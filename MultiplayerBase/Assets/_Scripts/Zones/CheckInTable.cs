using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CheckInTable : NetworkBehaviour, IInteractable
{
    [SerializeField] private CheckInManager checkInManager;
    [SerializeField] private ZoneInfo[] zoneInfoArr;

    [SerializeField] private bool isZoneController = false;

    private bool canInteract = true; 

    private void Awake()
    {
        EventManager.instance.onPuzzleComplete += OnPuzzleComplete;
        foreach(ZoneInfo zoneInfo in zoneInfoArr) zoneInfo.ResetZoneInfo();
    }

    public override void OnNetworkDespawn()
    {
        EventManager.instance.onPuzzleComplete -= OnPuzzleComplete;
    }

    public void OnInteract(Interactor interactor, ItemType itemUsed = ItemType.None)
    {
        SetCanInteractServerRPC(false);

        FirstPersonController playerController = interactor.GetComponent<FirstPersonController>();
        if (playerController != null)
        {
            playerController.DisableMovement();
            playerController.canInteract = false;
            interactor.GetComponent<Inventory>().DisableInv();
        }

        Cursor.lockState = CursorLockMode.Confined; 
        checkInManager.OpenMenu(this);

        foreach (ZoneInfo zoneInfo in zoneInfoArr) 
            checkInManager.AddZoneCard(zoneInfo, interactor);
    }

    public (bool, string) CanInteract(Interactor interactor, ItemType itemUsed = ItemType.None) => (canInteract, "");

    public bool RequiresZoneCheckIn() { return false; }

    private void OnPuzzleComplete(bool sucess, IInteractable interactable)
    {
        if (!sucess || !isZoneController) return;
        if (GameManager.instance.PlayerInSameZone(zoneInfoArr[0].Zone)) PuzzleCompleteServerRPC();
    }

    [ServerRpc(RequireOwnership = false)]
    private void PuzzleCompleteServerRPC() => PuzzleCompleteClientRPC();

    [ClientRpc]
    private void PuzzleCompleteClientRPC()
    {
        Debug.Log("Puzzle complete"); 
        zoneInfoArr[0].TasksComplete++; 
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetCanInteractServerRPC(bool canInteract) => SetCanInteractClientRPC(canInteract);

    [ClientRpc]
    private void SetCanInteractClientRPC(bool canInteract)
    {
        this.canInteract = canInteract;
    }
}
