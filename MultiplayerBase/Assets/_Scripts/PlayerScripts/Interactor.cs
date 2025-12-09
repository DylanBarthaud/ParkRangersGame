using System;
using Unity.Netcode;
using UnityEngine;

public class Interactor : NetworkBehaviour
{
    [SerializeField] private float interactRange;
    [SerializeField] private LayerMask interactMask;

    public void Interact(int tick = 0)
    {
        Collider[] seenObjColliders = Physics.OverlapSphere(transform.position, interactRange, interactMask);
        foreach (Collider collider in seenObjColliders)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                if (tick == 0)
                {
                    if (!interactable.CanInteract(this)) continue;
                    interactable.OnInteract(this);
                }
                else interactable.OnInteractHeld(this, tick);
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
