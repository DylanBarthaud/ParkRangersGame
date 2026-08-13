using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Switch : NetworkBehaviour, IInteractable
{
    public UnityEvent onButtonClick;

    public void OnInteract(Interactor interactor, ItemType itemUsed = ItemType.None) => OnInteractServerRpc();
    [ServerRpc(RequireOwnership = false)] private void OnInteractServerRpc() => OnInteractClientRpc();
    [ClientRpc] private void OnInteractClientRpc() => onButtonClick.Invoke();
}
