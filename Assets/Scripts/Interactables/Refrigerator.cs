using System;
using System.Linq;
using UnityEngine;

public class Refrigerator : MonoBehaviour, IInteractable
{
    private MeshRenderer _meshRenderer;
    private Material[] _baseMaterials;
    private Material[] _outlineMaterials;
    public Material outlineMat;

    public Canvas ingredientPicker;
    public GameObject ingredientPrefab;

    private PlayerInteractionComponent _player;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        
        ingredientPicker.enabled = false;
        
        _baseMaterials = _meshRenderer.materials;

        _outlineMaterials = new Material[_baseMaterials.Length + 1];
        _baseMaterials.CopyTo(_outlineMaterials, 0);
        _outlineMaterials[_outlineMaterials.Length - 1] = outlineMat;
    }

    public void Interact(PlayerInteractionComponent player)
    {
        if (ingredientPicker.enabled) return;
        if (player.ingredientActions.HeldIngredient != null) return;

        _player = player;
        ingredientPicker.enabled = true;
    }

    public void SelectIngredient(IngredientData data)
    {
        ingredientPicker.enabled = false;

        if (_player == null) return;

        _player.ingredientActions.Pickup(ingredientPrefab, data);
        _player = null;
    }

    public void ShowBoundary()
    {
        _meshRenderer.materials = _outlineMaterials;
    }

    public void HideBoundary()
    {
        _meshRenderer.materials = _baseMaterials;
        if(ingredientPicker.enabled)
            ingredientPicker.enabled = false;
    }
}
