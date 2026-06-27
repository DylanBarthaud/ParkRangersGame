using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CheckInTable : MonoBehaviour, IInteractable
{
    [SerializeField] private Zones zone;
    public Zones Zone => zone;
    [SerializeField] private int tasksInZone = 5;
    public int TasksInZone => tasksInZone;
    [SerializeField] private CheckInManager checkInManager;
    [SerializeField] private int maxCheckedInPlayers = 2;
    private List<ulong> checkedInPlayers = new List<ulong>();
    public List<ulong> CheckedInPlayers => checkedInPlayers;
    private int tasksComplete = 0; 
    public int TasksComplete => tasksComplete;
    private bool zoneComplete = false;

    private void Awake()
    {
        EventManager.instance.onPuzzleComplete += OnPuzzleComplete;
        EventManager.instance.onCheckIn += CheckOut; 
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

        checkInManager.SetInteractor(interactor);
        checkInManager.SetTable(this); 
        checkInManager.OpenMenu(); 
    }

    public (bool, string) CanInteract(Interactor interactor, ItemType itemUsed = ItemType.None) 
    {
        if (zoneComplete) return (false, "Zone Complete");
        if (checkedInPlayers.Count >= maxCheckedInPlayers) return (false, "CkeckIn limit reached"); 
        return (true, ""); 
    }

    public bool RequiresZoneCheckIn() { return false; }

    public void CheckIn(Interactor interactor)
    {
        CheckInServerRPC(interactor.OwnerClientId); 
        interactor.SetZone(Zone);
    }

    [ServerRpc(RequireOwnership = false)]
    public void CheckInServerRPC(ulong playerID) => CheckInClientRPC(playerID);

    [ClientRpc]
    private void CheckInClientRPC(ulong playerID)
    {
        EventManager.instance.OnCheckIn(playerID);
        checkedInPlayers.Add(playerID);
    }

    public void CheckOut(ulong playerID)
    {
        Debug.Log("CHECKED OUT PLAYER"); 
        checkedInPlayers.Remove(playerID);
    }

    private void OnPuzzleComplete(bool sucess, IInteractable interactable)
    {
        if (!sucess) return;
        if (GameManager.instance.PlayerInSameZone(zone)) PuzzleCompleteServerRPC();
        //Debug.Log(tasksComplete + "Tasks complete in: " + zone); 
    }

    [ServerRpc(RequireOwnership = false)]
    private void PuzzleCompleteServerRPC() => PuzzleCompleteClientRPC();

    [ClientRpc]
    private void PuzzleCompleteClientRPC()
    {
        tasksComplete++; 
    }

    public void CompleteZone()
    {
        if(tasksComplete >= tasksInZone)
        {
            zoneComplete = true;
            EventManager.instance.OnZoneComplete(); 
        }
    }
}
