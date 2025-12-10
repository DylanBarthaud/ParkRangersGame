using System;
using Unity.Netcode;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

public class SnareTrap : NetworkBehaviour, IInteractable
{
    [HideInInspector] public bool canInteract = true; 

    public void OnInteract(Interactor interactor)
    {
        FirstPersonController playerController = interactor.GetComponent<FirstPersonController>(); 
        if (playerController != null)
        {
            playerController.DisableMovement();
            GameManager.instance.uiManager.EnableSnareGameUi(this); 
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
