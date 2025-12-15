using System;
using Unity.Netcode;
using UnityEngine;

public class Interactor : NetworkBehaviour
{
    [SerializeField] private float interactRange;
    [SerializeField] private LayerMask interactMask;

    public void Interact(int tick = 0, ItemType itemUsed = ItemType.None)
    {
        Camera cam = Camera.main;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, interactRange, interactMask))
        {
            IInteractable interactable = hit.transform.GetComponent<IInteractable>();
            if (interactable != null)
            {
                if (tick == 0)
                {
                    if (!interactable.CanInteract(this, itemUsed)) return;
                    interactable.OnInteract(this, itemUsed);
                }
                else interactable.OnInteractHeld(this, tick, itemUsed);
            }
        }
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
                    if (!interactable.CanInteract(this, itemUsed)) continue;
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
}