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
    private bool canInteract = true; 

    public (bool, string) CanInteract(Interactor interactor, ItemType itemUsed) => (canInteract, "Already being used");

    public void OnInteract(Interactor interactor, ItemType itemUsed = ItemType.None)
    {
        //Debug.Log("Interact pressed");

        interactor.GetComponent<FirstPersonController>().DisableMovement();
        progressBar.maxValue = secondsToComplete;
        SetCanInteractServerRpc(false); 
        ui.SetActive(true);
    }

    public void OnInteractHeld(Interactor interactor, int tick, ItemType itemUsed)
    {
        //Debug.Log("Interact held");
        progressBar.value = tick; 

        if (tick == secondsToComplete)
        {
            Debug.Log("Rubbish pile complete puzzle"); 
            EventManager.instance.OnPuzzleComplete();
            DeleteButtonServerRpc();
        }
    }
    public void OnInteractReleased(Interactor interactor, int tick, ItemType itemUsed)
    {
        EventManager.instance.OnButtonReleased();
        SetCanInteractServerRpc(true);
        progressBar.value = 0;
        ui.SetActive(false); 
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeleteButtonServerRpc() => gameObject.GetComponent<NetworkObject>().Despawn();

    [ServerRpc(RequireOwnership = false)]
    public void SetCanInteractServerRpc(bool canInteract) => SetCanInteractClientRpc(canInteract);

    [ClientRpc]
    private void SetCanInteractClientRpc(bool canInteract)
    {
        Debug.Log(canInteract);
        this.canInteract = canInteract;
    }
}