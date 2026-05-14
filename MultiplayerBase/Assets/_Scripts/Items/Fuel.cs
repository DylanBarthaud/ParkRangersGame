using UnityEngine;

public class Fuel : Item
{
    [Header("Fuel Settings:")]
    [SerializeField] float interactDistance = 5;
    [SerializeField] LayerMask layerMask; 

    public override void UseItem(GameObject player)
    {
        player.GetComponent<Interactor>().Interact(0, ItemType); 
    }
}
