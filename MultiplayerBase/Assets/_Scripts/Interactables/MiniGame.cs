using NUnit.Framework;
using System;
using Unity.Netcode;
using UnityEngine;

public enum MiniGameTypes { SnareTrap, FuseBox, SpringTrap, Keypad, Memory }
[Serializable]
public struct Pair<Key, Value>
{
    public Key item1;
    public Value item2;
}

public class MiniGame : NetworkBehaviour, IInteractable
{
    [SerializeField] private MiniGameTypes game;
    [SerializeField] private ItemType neededItem = ItemType.None;

    private bool canInteract = true;

    [SerializeField] Pair<GameObject, bool>[] setObjests;
    [SerializeField] Pair<bool, string> completesQuest; 

    public void OnInteract(Interactor interactor, ItemType itemUsed = ItemType.None)
    {
        FirstPersonController playerController = interactor.GetComponent<FirstPersonController>();
        if (playerController != null)
        {
            playerController.DisableMovement();
            interactor.GetComponent<Inventory>().DisableInv();
            GameManager.instance.EnableMiniGame(game, gameObject);
        }

        SetCanInteractServerRpc(false);
    }

    public (bool, string) CanInteract(Interactor interactor, ItemType itemUsed = ItemType.None)
    {
        if (canInteract)
        {
            if (itemUsed == neededItem) return (true, "");
            return (false, $"Requires item: {neededItem}");
        }
        return (false, "");
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnCompleteServerRpc(bool success)
    {
        //Debug.Log(success); 
        SetCanInteractServerRpc(!success);
        if(success)
        {
            //if (completesQuest.item1) SendQuestCompleteClientRpc(); 
            ActivateObjsClientRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetCanInteractServerRpc(bool canInteract) => SetCanInteractClientRpc(canInteract);

    [ClientRpc]
    private void SetCanInteractClientRpc(bool canInteract) => this.canInteract = canInteract;
    

    [ClientRpc]
    public void ActivateObjsClientRpc()
    {
        if(setObjests.Length <= 0) return;
        
        foreach (var objWrapped in setObjests)
        {
            GameObject obj = objWrapped.item1; 
            bool setActive = objWrapped.item2;
            obj.SetActive(setActive);
        }
    }
}
 