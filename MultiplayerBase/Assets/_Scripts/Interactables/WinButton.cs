using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class WinButton : NetworkBehaviour, IInteractable
{
    private NetworkVariable<bool> isBeingPressed = new NetworkVariable<bool>();

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            isBeingPressed.Value = false; 
        }
    }

    public bool CanInteract(Interactor interactor, ItemType itemUsed)
    {
        return !isBeingPressed.Value; 
    }

    public void OnInteract(Interactor interactor, ItemType itemUsed = ItemType.None)
    {
        Debug.Log("Interact pressed");

        isBeingPressed.Value = true;
    }

    public void OnInteractHeld(Interactor interactor, int tick, ItemType itemUsed)
    {
        Debug.Log("Interact held"); 

        if (tick == 30)
        {
            EventManager.instance.OnPuzzleComplete();
            DeleteButtonServerRpc();
        }
    }
    public void OnInteractReleased(Interactor interactor, int tick, ItemType itemUsed)
    {
        EventManager.instance.OnButtonReleased();
        isBeingPressed.Value = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeleteButtonServerRpc()
    {
        gameObject.GetComponent<NetworkObject>().Despawn();
    }
}
