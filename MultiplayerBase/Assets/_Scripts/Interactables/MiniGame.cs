using Unity.Netcode;
using UnityEngine;

public enum MiniGameTypes { SnareTrap, FuseBox }

public class MiniGame : NetworkBehaviour, IInteractable
{
    [SerializeField] private MiniGameTypes game;  

    [HideInInspector] public bool canInteract = true;

    public void OnInteract(Interactor interactor)
    {
        FirstPersonController playerController = interactor.GetComponent<FirstPersonController>();
        if (playerController != null)
        {
            playerController.DisableMovement();
            interactor.GetComponent<Inventory>().DisableInv();


            GameManager.instance.EnableMiniGame(game, gameObject); 
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
