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
        EventManager.instance.OnButtonPressed();
        DeleteButtonRpc(); 
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeleteButtonRpc()
    {
        gameObject.GetComponent<NetworkObject>().Despawn();
    }
}
