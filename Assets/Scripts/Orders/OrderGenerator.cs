using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderGenerator : MonoBehaviour
{
    [Header("Ingredient Pool")] 
    [SerializeField] private List<IngredientData> availableIngredients;

    [Header("Display")] 
    [SerializeField] private Canvas orderCanvas;
    [SerializeField] private List<Image> iconSlots;
    [SerializeField] private TextMeshProUGUI ageTimerText;
    
    public Order CurrentOrder { get; private set; }
    public bool HasOrder => CurrentOrder != null;

    private void Awake()
    {
        orderCanvas.enabled = false;
    }

    private void Update()
    {
        if (CurrentOrder == null || !ageTimerText) return;

        int age = Mathf.FloorToInt(Time.time - CurrentOrder.StartTime);
        ageTimerText.text = age.ToString();
    }
    
    public void GenerateOrder()
    {
        int ingredientCount = Random.value < 0.5 ? 2 : 3;
        List<IngredientData> required = new List<IngredientData>(ingredientCount);
        
        for(int i = 0; i < ingredientCount; i++)
            required.Add(availableIngredients[Random.Range(0, availableIngredients.Count)]);

        CurrentOrder = new Order(required);
        orderCanvas.enabled = true;
        RefreshDisplay();
    }

    public void ClearOrder()
    {
        CurrentOrder = null;
        orderCanvas.enabled = false;
    }

    public void RefreshDisplay()
    {
        for (int i = 0; i < iconSlots.Count; i++)
        {
            bool showSlot = CurrentOrder != null &&
                            i < CurrentOrder.RequiredIngredients.Count &&
                            !CurrentOrder.Fulfilled[i];
            iconSlots[i].enabled = showSlot;
            if (showSlot)
                iconSlots[i].sprite = CurrentOrder.RequiredIngredients[i].Icon;
        }
    }
}
