using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class GeneratorFuel : NetworkBehaviour, IInteractable
{
    private NetworkVariable<bool> isBeingPressed = new NetworkVariable<bool>();

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            isBeingPressed.Value = false;
        }
    }

    public bool CanInteract(Interactor interactor)
    {
        return !isBeingPressed.Value;
    }

    public void OnInteract(Interactor interactor)
    {
        isBeingPressed.Value = true;
    }

    public void OnInteractHeld(Interactor interactor, int tick)
    {
        EventManager.instance.OnButtonHeld(tick, interactor);

        if (tick == 30)
        {
            EventManager.instance.OnButtonPressed();
        }
    }
    public void OnInteractReleased(Interactor interactor, int tick)
    {
        EventManager.instance.OnButtonReleased();
        isBeingPressed.Value = false;
    }
}
