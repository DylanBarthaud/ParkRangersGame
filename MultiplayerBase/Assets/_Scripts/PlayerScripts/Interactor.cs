using System;
using Unity.Netcode;
using UnityEngine;

public class Interactor : NetworkBehaviour
{
    [SerializeField] private float interactRange;
    [SerializeField] private LayerMask interactMask;

    private Zones currentZone = Zones.Null;
    public Zones CurrentZone => currentZone;

    public bool Interact(int tick = 0, ItemType itemUsed = ItemType.None)
    {
        Inventory interactorInventory = GetComponent<Inventory>();
        //if(interactorInventory.CarryingHeavy) return false;

        Camera cam = Camera.main;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, interactRange, interactMask))
        {
            if (hit.collider.gameObject.layer != 3) return false;


            IInteractable interactable = hit.transform.GetComponent<IInteractable>();
            if(interactable.RequiresZoneCheckIn() && !GameManager.instance.PlayerInSameZone(currentZone))
            {
                return false;
            }

            if (interactable != null)
            {
       
                if (tick == 0)
                {
                    bool canInteract;
                    string cantInteractReason;

                    (canInteract, cantInteractReason) = interactable.CanInteract(this, itemUsed);
                    if (!canInteract) return false;
                    interactable.OnInteract(this, itemUsed);
                    return true; 
                }
                else interactable.OnInteractHeld(this, tick, itemUsed);
            }
        }

        return false;
    }

    public void CircleInteract(int tick = 0, ItemType itemUsed = ItemType.None)
    {
        Collider[] seenObjColliders = Physics.OverlapSphere(transform.position, interactRange, interactMask);
        foreach (Collider collider in seenObjColliders)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                if (tick == 0)
                {
                    bool canInteract;
                    string cantInteractReason;

                    (canInteract, cantInteractReason) = interactable.CanInteract(this, itemUsed); 
                    if (!canInteract) continue;
                    interactable.OnInteract(this, itemUsed);
                }
                else interactable.OnInteractHeld(this, tick, itemUsed);
            }
        }
    }

    public void Release(int tick = 0)
    {
        if (tick > 0)
        {
            Collider[] seenObjColliders = Physics.OverlapSphere(transform.position, interactRange, interactMask);
            foreach (Collider collider in seenObjColliders)
            {
                IInteractable interactable = collider.GetComponent<IInteractable>();
                if (interactable != null) interactable.OnInteractReleased(this, tick);
            }
        }
    }

    public void SetZone(Zones zone) { currentZone = zone; }
}