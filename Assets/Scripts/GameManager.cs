using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Order Windows")]
    [SerializeField] private List<OrderGenerator> orderWindows;
    [SerializeField] private float orderRespawnDelay = 5f;

    [Header("Game Cycle")]
    [SerializeField] private float gameDuration = 180f;
    private float _gameTimer;
    private bool _gameRunning;

    [Header("Start Countdown")]
    [SerializeField] private float startCountdown = 3f;
    [SerializeField] private TextMeshProUGUI startCountdownText;

    [Header("Game Timer UI")]
    [SerializeField] private TextMeshProUGUI gameTimerText;

    [Header("Game Over")]
    [SerializeField] private Canvas gameOverCanvas;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    public int Score { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameOverCanvas.enabled = false;

        UpdateGameTimerText(gameDuration);

        StartCoroutine(GameStartRoutine());
    }

    private IEnumerator GameStartRoutine()
    {
        startCountdownText.gameObject.SetActive(true);

        float t = startCountdown;
        while (t > 0f)
        {
            startCountdownText.text = Mathf.CeilToInt(t).ToString();
            yield return null;
            t -= Time.deltaTime;
        }

        startCountdownText.gameObject.SetActive(false);

        BeginGame();
    }

    private void BeginGame()
    {
        _gameTimer = gameDuration;
        _gameRunning = true;

        StartCoroutine(OrderRespawnRoutine());
    }

    private void Update()
    {
        if (!_gameRunning) return;

        _gameTimer -= Time.deltaTime;

        if (_gameTimer <= 0f)
        {
            _gameTimer = 0f;
            UpdateGameTimerText(_gameTimer);
            EndGame();
            return;
        }

        UpdateGameTimerText(_gameTimer);
    }

    private void UpdateGameTimerText(float time)
    {
        if (gameTimerText == null) return;

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        gameTimerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void AddScore(int amount)
    {
        Score += amount;
    }

    public IEnumerator OrderRespawnRoutine()
    {
        while (true)
        {
            OrderGenerator emptyWindow = FindNextEmptyWindow();

            if (!emptyWindow)
            {
                yield return null;
                continue;
            }

            yield return new WaitForSeconds(orderRespawnDelay);

            if (!emptyWindow.HasOrder)
                emptyWindow.GenerateOrder();
        }
    }

    private OrderGenerator FindNextEmptyWindow()
    {
        foreach (var window in orderWindows)
        {
            if (!window.HasOrder) return window;
        }

        return null;
    }

    private void EndGame()
    {
        _gameRunning = false;
        StopAllCoroutines();

        if (finalScoreText)
            finalScoreText.text = $"{Score}";

        gameOverCanvas.enabled = true;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}