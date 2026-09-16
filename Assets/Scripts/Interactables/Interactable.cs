using UnityEngine;

public interface IInteractable
{
    void Interact(PlayerInteractionComponent player);
    void ShowBoundary();
    void HideBoundary();
}
