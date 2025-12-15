using UnityEngine;

public class ScrewDriver : Item
{
    [Header("Screw Driver Settings:")]
    [SerializeField] float interactDistance = 5;
    [SerializeField] LayerMask layerMask; 

    public override void UseItem(GameObject player)
    {
        player.GetComponent<Interactor>().Interact(0, ItemType); 
    }
}
