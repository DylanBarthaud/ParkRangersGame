using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class WinButton : NetworkBehaviour, IInteractable
{
    public bool CanInteract(Interactor interactor)
    {
        return true; 
    }

    public void OnInteract(Interactor interactor)
    {
    }

    public void OnInteractHeld(Interactor interactor, int tick)
    {
        EventManager.instance.OnButtonHeld(tick, interactor); 
        if(tick == 30)
        {
            EventManager.instance.OnButtonPressed();
            DeleteButtonServerRpc();
        }
    }
    public void OnInteractReleased(Interactor interactor, int tick)
    {
        EventManager.instance.OnButtonReleased(); 
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeleteButtonServerRpc()
    {
        gameObject.GetComponent<NetworkObject>().Despawn();
    }
}
