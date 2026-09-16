using System;
using UnityEngine;

public class IngredientActions : MonoBehaviour
{
    public Transform handSocket;
    public Ingredient HeldIngredient { get; private set; }

    public void Pickup(GameObject ingredientPrefab, IngredientData data)
    {
        if (HeldIngredient != null) return;

        var obj = Instantiate(ingredientPrefab);
        var ingredient = obj.GetComponent<Ingredient>();
        ingredient.Initialize(data);

        HeldIngredient = ingredient;
        ingredient.transform.SetParent(handSocket);
        ingredient.transform.localPosition = Vector3.zero;
        ingredient.transform.localRotation = Quaternion.identity;
    }
    
    public void Pickup(Ingredient ingredient)
    {
        if (HeldIngredient != null) return;
        
        HeldIngredient = ingredient;
        ingredient.transform.SetParent(handSocket);
        ingredient.transform.localPosition = Vector3.zero;
        ingredient.transform.localRotation = Quaternion.identity;
    }

    public Ingredient PlaceHeld(Transform destination)
    {
        Ingredient placed = HeldIngredient;
        if (placed == null) return null;

        placed.transform.SetParent(destination);
        HeldIngredient = null;
        return placed;
    }

    public void ConsumeHeld()
    {
        if (HeldIngredient == null) return;
        
        Destroy(HeldIngredient.gameObject);
        HeldIngredient = null;
    }

public void ClearHand()
    {
        HeldIngredient = null;
    }
}
