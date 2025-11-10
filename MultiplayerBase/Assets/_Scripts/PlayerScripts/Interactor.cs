using System;
using Unity.Netcode;
using UnityEngine;

public class Interactor : NetworkBehaviour
{
    [SerializeField] private float interactRange;
    [SerializeField] private LayerMask interactMask;

    private void Awake()
    {
        EventManager.instance.onTick += OnTick;
    }

    int localTick = 0; 
    private void OnTick(int tick)
    {
        if (!IsOwner) return;

        Collider[] seenObjColliders = Physics.OverlapSphere(transform.position, interactRange, interactMask);
        if (Input.GetKey(KeyCode.E))
        {
            foreach (Collider collider in seenObjColliders)
            {
                IInteractable interactable = collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    if (localTick == 0)
                    {
                        if (interactable.CanInteract(this) == false) continue;
                        interactable.OnInteract(this);
                    }
                    else interactable.OnInteractHeld(this, localTick);

                    localTick++;
                }
            }
        }

        else
        {
            if(localTick > 0)
            {
                foreach (Collider collider in seenObjColliders)
                {
                    IInteractable interactable = collider.GetComponent<IInteractable>();
                    if(interactable != null) interactable.OnInteractReleased(this, localTick);
                }
            }

            localTick = 0;
        }

    }
}
