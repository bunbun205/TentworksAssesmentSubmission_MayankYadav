using UnityEngine;
using UnityEngine.UI;

public class Table : MonoBehaviour, IInteractable
{
    private MeshRenderer _meshRenderer;
    private Material[] _baseMaterials;
    private Material[] _outlineMaterials;
    public Material outlineMat;

    [Header("Chopping")] [SerializeField] private Transform slot;

    [Header("UI")] [SerializeField] private Slider chopSlider;

    private Ingredient _occupant;
    private float _timer;
    private float _totalPrepTime;
    private bool _isChopping;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();

        _baseMaterials = _meshRenderer.materials;

        _outlineMaterials = new Material[_baseMaterials.Length + 1];
        _baseMaterials.CopyTo(_outlineMaterials, 0);
        _outlineMaterials[_outlineMaterials.Length - 1] = outlineMat;

        if (chopSlider != null)
            chopSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!_isChopping) return;

        _timer -= Time.deltaTime;

        if (chopSlider != null)
            chopSlider.value = 1f - (_timer / _totalPrepTime); // fills up as it chops

        if (_timer <= 0f)
        {
            _isChopping = false;
            _occupant.MarkPrepped();

            if (chopSlider != null)
                chopSlider.gameObject.SetActive(false);
        }
    }

    public void Interact(PlayerInteractionComponent player)
    {
        Ingredient held = player.ingredientActions.HeldIngredient;

        if (held != null && _occupant == null)
        {
            bool isChoppable = held.Data.PrepRequired &&
                               held.Data.PrepType == PrepType.Chop &&
                               !held.IsPrepped;

            if (!isChoppable) return;

            _occupant = player.ingredientActions.PlaceHeld(slot);
            _occupant.transform.localPosition = Vector3.zero;
            _occupant.transform.localRotation = Quaternion.identity;

            _totalPrepTime = _occupant.Data.PrepTime;
            _timer = _totalPrepTime;
            _isChopping = true;

            if (chopSlider != null)
            {
                chopSlider.value = 0f;
                chopSlider.gameObject.SetActive(true);
            }
            return;
        }

        if (held == null && _occupant != null && !_isChopping)
        {
            player.ingredientActions.Pickup(_occupant);
            _occupant = null;
        }
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