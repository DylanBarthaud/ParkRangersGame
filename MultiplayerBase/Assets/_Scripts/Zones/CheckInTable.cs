using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CheckInTable : MonoBehaviour, IInteractable
{
    [SerializeField] private CheckInManager checkInManager;
    [SerializeField] private ZoneInfo[] zoneInfoArr;

    [SerializeField] private bool isZoneController = false;

    private void Awake()
    {
        EventManager.instance.onPuzzleComplete += OnPuzzleComplete;
    }

    public void OnInteract(Interactor interactor, ItemType itemUsed = ItemType.None)
    {
        FirstPersonController playerController = interactor.GetComponent<FirstPersonController>();
        if (playerController != null)
        {
            playerController.DisableMovement();
            interactor.GetComponent<Inventory>().DisableInv();
        }

        Cursor.lockState = CursorLockMode.Confined; 
        checkInManager.OpenMenu();

        foreach (ZoneInfo zoneInfo in zoneInfoArr) 
            checkInManager.AddZoneCard(zoneInfo, interactor);
    }

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
        zoneInfoArr[0].TasksComplete++; 
    }
}
