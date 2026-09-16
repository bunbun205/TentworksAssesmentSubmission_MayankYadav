using UnityEngine;

public class Trash : MonoBehaviour, IInteractable
{
    private MeshRenderer _meshRenderer;
    private Material[] _baseMaterials;
    private Material[] _outlineMaterials;
    public Material outlineMat;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        
        _baseMaterials = _meshRenderer.materials;

        _outlineMaterials = new Material[_baseMaterials.Length + 1];
        _baseMaterials.CopyTo(_outlineMaterials, 0);
        _outlineMaterials[_outlineMaterials.Length - 1] = outlineMat;
    }

    public void Interact(PlayerInteractionComponent player)
    {
        if (player.ingredientActions.HeldIngredient == null) return;
        player.ingredientActions.ConsumeHeld();
    }

    public void ShowBoundary()
    {
        _meshRenderer.materials = _outlineMaterials;
    }

    public void HideBoundary()
    {
        _meshRenderer.materials = _baseMaterials;
    }
}
