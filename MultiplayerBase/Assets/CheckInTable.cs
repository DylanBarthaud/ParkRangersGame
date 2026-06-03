using UnityEngine;

public class CheckInTable : MonoBehaviour, IInteractable
{
    [SerializeField] private Zones zone; 

    public void OnInteract(Interactor interactor, ItemType itemUsed = ItemType.None)
    {
        interactor.SetZone(zone);
        Debug.Log($"set to zone {zone}"); 
    }

    public bool RequiresZoneCheckIn() { return false; }
}
