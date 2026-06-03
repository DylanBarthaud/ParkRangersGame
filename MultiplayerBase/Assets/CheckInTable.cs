using System;
using UnityEngine;

public class CheckInTable : MonoBehaviour, IInteractable
{
    [SerializeField] private Zones zone;
    public Zones Zone => zone;
    [SerializeField] private int tasksInZone = 5;
    [SerializeField] private CheckInManager checkInManager;
    private int tasksComplete = 0; 
    private bool zoneComplete = false;
    Interactor interactor;

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
        Cursor.visible = true;

        checkInManager.SetInteractor(interactor);
        checkInManager.SetTable(this); 
        checkInManager.OpenMenu(); 
    }

    public bool RequiresZoneCheckIn() { return false; }

    private void OnPuzzleComplete(bool sucess, IInteractable interactable)
    {
        if (!sucess) return;
        if (GameManager.instance.PlayerInSameZone(zone)) tasksComplete++;
        Debug.Log(tasksComplete + "Tasks complete in: " + zone); 
    }

    public void CompleteZone()
    {
        if(tasksComplete >= tasksInZone) zoneComplete = true;
    }
}
