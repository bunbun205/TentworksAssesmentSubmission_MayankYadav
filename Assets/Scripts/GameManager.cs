using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private const string HighScoreKey = "YesChef_HighScore";

    [Header("Order Windows")]
    [SerializeField] private List<OrderGenerator> orderWindows;

    [Header("Game Cycle")]
    [SerializeField] private float gameDuration = 180f;
    private float _gameTimer;
    private bool _gameRunning;
    private bool _isPaused;

    [Header("Start Screen")]
    [SerializeField] private Canvas startScreenCanvas;
    [SerializeField] private Button beginButton;

    [Header("Start Countdown")]
    [SerializeField] private float startCountdown = 3f;
    [SerializeField] private TextMeshProUGUI startCountdownText;

    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI gameTimerText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("Pause")]
    [SerializeField] private Canvas pauseCanvas;
    [SerializeField] private Button pauseButton;

    [Header("Game Over")]
    [SerializeField] private Canvas gameOverCanvas;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private GameObject newHighScoreLabel;

    public int Score { get; private set; }
    public int HighScore { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1f;

        gameOverCanvas.enabled = false;
        if (pauseCanvas != null) pauseCanvas.enabled = false;
        startCountdownText.gameObject.SetActive(false);

        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        UpdateScoreText();
        UpdateGameTimerText(gameDuration);

        if (beginButton != null)
            beginButton.onClick.AddListener(OnBeginPressed);

        if (pauseButton != null)
            pauseButton.onClick.AddListener(TogglePause);

        startScreenCanvas.enabled = true;
    }

    private void OnBeginPressed()
    {
        startScreenCanvas.enabled = false;
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

        foreach (var window in orderWindows)
            window.GenerateOrder(); 
    }

    private void Update()
    {
        if (!_gameRunning || _isPaused) return;

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
        if (!gameTimerText) return;

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        gameTimerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void AddScore(int amount)
    {
        Score += amount;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText)
            scoreText.text = $"{Score}";
        if (highScoreText)
            highScoreText.text = $"{HighScore}";
    }

    public void TogglePause()
    {
        if (!_gameRunning) return;

        _isPaused = !_isPaused;
        Time.timeScale = _isPaused ? 0f : 1f;

        if (pauseCanvas != null)
            pauseCanvas.enabled = _isPaused;
    }

    private void EndGame()
    {
        _gameRunning = false;
        Time.timeScale = 0f; 

        bool isNewHighScore = Score > HighScore;
        if (isNewHighScore)
        {
            HighScore = Score;
            PlayerPrefs.SetInt(HighScoreKey, HighScore);
            PlayerPrefs.Save();
        }

        UpdateScoreText();

        if (finalScoreText)
            finalScoreText.text = $"{Score}";

        if (newHighScoreLabel)
            newHighScoreLabel.SetActive(isNewHighScore);

        gameOverCanvas.enabled = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
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