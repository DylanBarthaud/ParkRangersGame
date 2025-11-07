using Unity.Netcode;
using UnityEngine;

public class Interactor : NetworkBehaviour
{
    [SerializeField] private float interactRange;
    [SerializeField] private LayerMask interactMask;

    private void Update()
    {
        if(!IsHost) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider[] seenObjColliders = Physics.OverlapSphere(transform.position, interactRange, interactMask);
            foreach(Collider collider in seenObjColliders)
            {
                IInteractable interactable = collider.GetComponent<IInteractable>(); 
                if (interactable != null)
                {
                    if(interactable.CanInteract(this) == false) continue;
                    interactable.OnInteract(this); 
                }
            }
        }
    }
}
