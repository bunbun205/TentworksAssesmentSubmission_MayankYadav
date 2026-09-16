using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngredientPicker : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private List<Transform> buttonPositions;

    private Refrigerator _refrigerator;
    private readonly List<GameObject> spawnedButtons = new();

    private void Awake()
    {
        _refrigerator = transform.parent.GetComponent<Refrigerator>();
        AddButtons();
    }

    private void AddButtons()
    {
        IngredientData[] allIngredients = Resources.LoadAll<IngredientData>("");

        int i = 0;
        foreach (var data in allIngredients)
        {
            GameObject buttonObj = Instantiate(buttonPrefab, buttonPositions[i]);

            Image image = buttonObj.GetComponent<Image>();
            if (image != null)
                image.sprite = data.Icon;

            Button button = buttonObj.GetComponent<Button>();
            IngredientData captured = data;
            button.onClick.AddListener(() => _refrigerator.SelectIngredient(captured));
            
            spawnedButtons.Add(buttonObj);
            ++i;
        }
    }
}
