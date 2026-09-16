using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionComponent : MonoBehaviour
{
    [Header("Input")] public InputActionReference interact;

    [Header("Interaction")] 
    public Transform interactionSphere;
    public float interactionRadius;
    public LayerMask interactableLayer;
    private IInteractable _target;

    public IngredientActions ingredientActions;

    private void OnEnable()
    {
        interact.action.Enable();

        interact.action.started += OnInteract;
    }

    private void OnDisable()
    {
        interact.action.started -= OnInteract;
        
        interact.action.Disable();
    }

    private void Awake()
    {
        ingredientActions = GetComponent<IngredientActions>();
        ingredientActions.ClearHand();
    }

    private void Update()
    {
        IInteractable nearest = UpdateNearestInteractable();

        if (nearest != _target)
        {
            _target?.HideBoundary();
            nearest?.ShowBoundary();
            _target = nearest;
        }
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        _target?.Interact(this);
    }

    private IInteractable UpdateNearestInteractable()
    {
        Collider[] hits = Physics.OverlapSphere(interactionSphere.position, interactionRadius, interactableLayer);

        IInteractable nearest = null;
        float nearestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent<IInteractable>(out var interactable)) continue;

            float dist = (hit.transform.position - interactionSphere.position).sqrMagnitude;
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = interactable;
            }
        }

        return nearest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(interactionSphere.position, interactionRadius);
    }
}
