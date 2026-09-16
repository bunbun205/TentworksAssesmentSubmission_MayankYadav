using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stove : MonoBehaviour, IInteractable
{
    private MeshRenderer _meshRenderer;
    private Material[] _baseMaterials;
    private Material[] _outlineMaterials;
    public Material outlineMat;

    [Header("Cooking")] [SerializeField] private List<Transform> slots;
    [Header("UI")] [SerializeField] private List<Slider> cookSliders; // must match slots order/count

    private class SlotState
    {
        public Ingredient occupant;
        public float timer;
        public float totalPrepTime;
        public bool cooking;
    }

    private List<SlotState> _slotStates;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();

        _baseMaterials = _meshRenderer.materials;

        _outlineMaterials = new Material[_baseMaterials.Length + 1];
        _baseMaterials.CopyTo(_outlineMaterials, 0);
        _outlineMaterials[_outlineMaterials.Length - 1] = outlineMat;

        _slotStates = new List<SlotState>(slots.Count);
        for (int i = 0; i < slots.Count; i++)
            _slotStates.Add(new SlotState());

        foreach (var slider in cookSliders)
            if (slider != null)
                slider.gameObject.SetActive(false);
    }

    private void Update()
    {
        for (int i = 0; i < _slotStates.Count; i++)
        {
            var slot = _slotStates[i];
            if (!slot.cooking) continue;

            slot.timer -= Time.deltaTime;

            if (i < cookSliders.Count && cookSliders[i] != null)
                cookSliders[i].value = 1f - (slot.timer / slot.totalPrepTime);

            if (slot.timer <= 0f)
            {
                slot.cooking = false;
                slot.occupant.MarkPrepped();

                if (i < cookSliders.Count && cookSliders[i] != null)
                    cookSliders[i].gameObject.SetActive(false);
            }
        }
    }

    public void Interact(PlayerInteractionComponent player)
    {
        Ingredient held = player.ingredientActions.HeldIngredient;

        if (held != null)
        {
            bool isCookable = held.Data.PrepRequired &&
                               held.Data.PrepType == PrepType.Cook &&
                               !held.IsPrepped;

            if (!isCookable) return;

            int freeIndex = GetFreeSlot();
            if (freeIndex == -1) return;

            SlotState slot = _slotStates[freeIndex];
            slot.occupant = player.ingredientActions.PlaceHeld(slots[freeIndex]);
            slot.occupant.transform.localPosition = Vector3.zero;
            slot.occupant.transform.localRotation = Quaternion.identity;

            slot.totalPrepTime = slot.occupant.Data.PrepTime;
            slot.timer = slot.totalPrepTime;
            slot.cooking = true;

            if (freeIndex < cookSliders.Count && cookSliders[freeIndex] != null)
            {
                cookSliders[freeIndex].value = 0f;
                cookSliders[freeIndex].gameObject.SetActive(true);
            }
            return;
        }

        int readyIndex = GetFreeableSlot();
        if (readyIndex != -1)
        {
            SlotState slot = _slotStates[readyIndex];
            player.ingredientActions.Pickup(slot.occupant);
            slot.occupant = null;
        }
    }

    private int GetFreeSlot()
    {
        for (int i = 0; i < _slotStates.Count; i++)
            if (_slotStates[i].occupant == null)
                return i;
        return -1;
    }

    private int GetFreeableSlot()
    {
        for (int i = 0; i < _slotStates.Count; i++)
            if (_slotStates[i].occupant != null && !_slotStates[i].cooking)
                return i;
        return -1;
    }

    public float GetSlotTimeRemaining(int index) => Mathf.Max(0f, _slotStates[index].timer);
    public bool IsSlotCooking(int index) => _slotStates[index].cooking;
    public bool IsSlotOccupied(int index) => _slotStates[index].occupant != null;

    public void ShowBoundary()
    {
        _meshRenderer.materials = _outlineMaterials;
    }

    public void HideBoundary()
    {
        _meshRenderer.materials = _baseMaterials;
    }
}