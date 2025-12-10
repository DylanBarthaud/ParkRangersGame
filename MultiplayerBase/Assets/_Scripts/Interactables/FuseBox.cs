using Unity.Netcode;
using UnityEngine;

public class FuseBox : NetworkBehaviour, IInteractable
{
    [HideInInspector] public bool canInteract = true;

    public void OnInteract(Interactor interactor)
    {
        FirstPersonController playerController = interactor.GetComponent<FirstPersonController>();
        if (playerController != null)
        {
            playerController.DisableMovement();
            interactor.GetComponent<Inventory>().DisableInv();

            GameManager.instance.uiManager.EnableFuseGameUi(this);
        }
    }

    public bool CanInteract(Interactor interactor)
    {
        if (interactor.gameObject.CompareTag("Player"))
        {
            return canInteract;
        }
        return false;
    }
}
