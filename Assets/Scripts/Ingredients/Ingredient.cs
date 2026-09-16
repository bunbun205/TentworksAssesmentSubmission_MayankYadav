using System;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    [SerializeField] private IngredientData data;
    private bool isPrepped;
    
    private MeshRenderer meshRenderer;
    
    public IngredientData Data => data;
    public bool IsPrepped => isPrepped;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Initialize(IngredientData ingredientData)
    {
        data = ingredientData;
        isPrepped = !data.PrepRequired;
        UpdateVisual();
    }
    
    public void MarkPrepped()
    {
        isPrepped = true;
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        if (meshRenderer == null) return;
        meshRenderer.material = isPrepped ? data.PreppedMat : data.UnpreppedMat;
    }
}
