using Unity.Netcode;
using UnityEngine;

public enum MiniGameTypes { SnareTrap, FuseBox, SpringTrap }

public class MiniGame : NetworkBehaviour, IInteractable
{
    [SerializeField] private MiniGameTypes game;
    [SerializeField] private ItemType neededItem = ItemType.None;

    [HideInInspector] public bool canInteract = true;

    public void OnInteract(Interactor interactor, ItemType itemUsed = ItemType.None)
    {
        FirstPersonController playerController = interactor.GetComponent<FirstPersonController>();
        if (playerController != null)
        {
            playerController.DisableMovement();
            interactor.GetComponent<Inventory>().DisableInv();
            GameManager.instance.EnableMiniGame(game, gameObject);
        }
    }

    public bool CanInteract(Interactor interactor, ItemType itemUsed = ItemType.None)
    {
        if (canInteract)
        {
            if (itemUsed == neededItem) return true;
        }
        return false;
    }
}
 