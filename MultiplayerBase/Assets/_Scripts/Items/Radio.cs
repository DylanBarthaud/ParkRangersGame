using Discord;
using UnityEngine;

public class Radio : Item
{
    [SerializeField] GameObject onLight;
    VoiceInputController voiceController;

    public override void OnInteract(Interactor interactor, ItemType itemType = ItemType.None)
    {
        base.OnInteract(interactor, itemType);
        if (voiceController == null) voiceController = interactor.GetComponent<VoiceInputController>();
    }

    public override void UseItem(GameObject user)
    {
        voiceController.CanUseRadio = !voiceController.CanUseRadio;
        onLight.SetActive(!onLight.activeInHierarchy);
        SetUsingPowerServerRPC(voiceController.CanUseRadio); 
    }

    public override void DropItem(Vector3 newPos, Inventory inventory)
    {
        base.DropItem(newPos, inventory);
        voiceController.CanUseRadio = false;
    }

    public override bool RunOutOfPower()
    {
        if (!base.RunOutOfPower()) return false;
        voiceController.CanUseRadio = false; 
        SetUsingPowerServerRPC(false);
        return true;
    }

    public override void RemoveBattery(int index, Vector3 dropBatteryPos, Inventory inventory)
    {
        base.RemoveBattery(index, dropBatteryPos, inventory);
        if (Batteries.Count <= 0)
        {
            voiceController.CanUseRadio = false;
            SetUsingPowerServerRPC(false);
        }
    }
}
