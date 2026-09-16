using System.Collections.Generic;
using UnityEngine;

public class Order
{
    public List<IngredientData> RequiredIngredients { get; }
    public List<bool> Fulfilled { get; }
    public float StartTime { get; }

    public Order(List<IngredientData> requiredIngredients)
    {
        RequiredIngredients = requiredIngredients;
        Fulfilled = new List<bool>(new bool[requiredIngredients.Count]);
        StartTime = Time.time;
    }

    public bool IsComplete => Fulfilled.TrueForAll(f => f);
}
