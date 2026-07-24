using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RubbishPile : NetworkBehaviour, IInteractable
{
    [SerializeField] private GameObject ui;
    [SerializeField] private Slider progressBar;
    [Header("Settings")]
    [SerializeField] private int secondsToComplete = 30;
    private NetworkVariable<bool> isBeingPressed = new NetworkVariable<bool>();

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            isBeingPressed.Value = false; 
        }
    }

    public (bool, string) CanInteract(Interactor interactor, ItemType itemUsed)
    {
        return (!isBeingPressed.Value, "Already being used"); 
    }

    public void OnInteract(Interactor interactor, ItemType itemUsed = ItemType.None)
    {
        Debug.Log("Interact pressed");

        interactor.GetComponent<FirstPersonController>().DisableMovement();
        progressBar.maxValue = secondsToComplete; 
        isBeingPressed.Value = true;
        ui.SetActive(true);
    }

    public void OnInteractHeld(Interactor interactor, int tick, ItemType itemUsed)
    {
        Debug.Log("Interact held");
        progressBar.value = tick; 

        if (tick == secondsToComplete)
        {
            EventManager.instance.OnPuzzleComplete();
            DeleteButtonServerRpc();
        }
    }
    public void OnInteractReleased(Interactor interactor, int tick, ItemType itemUsed)
    {
        EventManager.instance.OnButtonReleased();
        isBeingPressed.Value = false;
        progressBar.value = 0;
        ui.SetActive(false); 
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeleteButtonServerRpc()
    {
        gameObject.GetComponent<NetworkObject>().Despawn();
    }
}
