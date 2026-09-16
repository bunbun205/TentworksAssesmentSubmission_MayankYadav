using TMPro;
using UnityEngine;

public class OrderFinisher : MonoBehaviour, IInteractable
{
    [Header("Refs")] 
    [SerializeField] private OrderGenerator _orderGenerator;
    
    [Header("Score")]
    [SerializeField] private TextMeshProUGUI scoreText;

    public void Interact(PlayerInteractionComponent player)
    {
        Ingredient held = player.ingredientActions.HeldIngredient;
        if (held == null) return;

        Order order = _orderGenerator.CurrentOrder;
        if (order == null) return;

        int slotIndex = FindMatchingUnfinishedSlot(order, held);
        if (slotIndex == -1) return;

        order.Fulfilled[slotIndex] = true;
        player.ingredientActions.ConsumeHeld();
        _orderGenerator.RefreshDisplay();

        if (order.IsComplete)
            CompleteOrder(order);
    }

    private int FindMatchingUnfinishedSlot(Order order, Ingredient held)
    {
        for (int i = 0; i < order.RequiredIngredients.Count; i++)
        {
            if(order.Fulfilled[i]) continue;
            if(order.RequiredIngredients[i].Type != held.Data.Type) continue;
            if(held.Data.PrepRequired && !held.IsPrepped) continue;

            return i;
        }
        return -1;
    }

    private void CompleteOrder(Order order)
    {
        int rawScore = 0;
        foreach (var ingredient in order.RequiredIngredients)
            rawScore += ingredient.Score;

        int secondsElapsed = Mathf.FloorToInt(Time.time - order.StartTime);
        int finalScore = rawScore - secondsElapsed;

        GameManager.Instance.AddScore(finalScore);
        ShowScorePopup(finalScore);
        
        _orderGenerator.ClearOrder();
    }

    private void ShowScorePopup(int score)
    {
        if (scoreText == null) return;

        scoreText.gameObject.SetActive(true);
        scoreText.text = score >= 0 ? $"+{score}" : score.ToString();
    }

    public void ShowBoundary()
    {
    }

    public void HideBoundary()
    {
    }
}