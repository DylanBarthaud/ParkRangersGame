using System;
using Unity.Netcode;
using UnityEngine;

public enum MiniGameTypes { SnareTrap, FuseBox, SpringTrap }
[Serializable]
public struct ActivatedObjs
{
    public GameObject obj;
    public bool setActive; 
}

public class MiniGame : NetworkBehaviour, IInteractable
{
    [SerializeField] private MiniGameTypes game;
    [SerializeField] private ItemType neededItem = ItemType.None;

    [HideInInspector] private bool canInteract = true;

    [SerializeField] ActivatedObjs[] setObjests;

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

    public void OnComplete(bool canInteract)
    {
        SetCanInteractClientRpc(canInteract);
        ActivateObjsClientRpc(); 
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
            GameObject obj = objWrapped.obj; 
            bool setActive = objWrapped.setActive;
            obj.SetActive(setActive);
        }
    }
}
 