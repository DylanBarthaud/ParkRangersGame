using NUnit.Framework;
using System;
using Unity.Netcode;
using UnityEngine;

public enum MiniGameTypes { SnareTrap, FuseBox, SpringTrap }
[Serializable]
public struct Pair<Item1, Item2>
{
    public Item1 item1;
    public Item2 item2;
}

public class MiniGame : NetworkBehaviour, IInteractable
{
    [SerializeField] private MiniGameTypes game;
    [SerializeField] private ItemType neededItem = ItemType.None;

    [HideInInspector] private bool canInteract = true;

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
    }

    public bool CanInteract(Interactor interactor, ItemType itemUsed = ItemType.None)
    {
        if (canInteract)
        {
            if (itemUsed == neededItem) return true;
        }
        return false;
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnCompleteServerRpc(bool success)
    {
        SetCanInteractClientRpc(!success);
        if(success)
        {
            if (completesQuest.item1) SendQuestCompleteClientRpc(); 
            ActivateObjsClientRpc();
        }
    }
    [ClientRpc]
    private void SendQuestCompleteClientRpc()
    {
        EventManager.instance.OnQuestComplete(completesQuest.item2);
    }

    [ClientRpc]
    public void SetCanInteractClientRpc(bool canInteract)
    {
        this.canInteract = canInteract;
    }

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
 