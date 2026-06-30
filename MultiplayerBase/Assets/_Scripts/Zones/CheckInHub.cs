using UnityEngine;

public class CheckInHub : MonoBehaviour, IInteractable
{
    [SerializeField] CheckInManager manager;
    public void OnInteract(Interactor interactor, ItemType itemUsed = ItemType.None)
    {

    }
}
