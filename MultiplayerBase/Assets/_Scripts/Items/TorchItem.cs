using UnityEngine;

public class TorchItem : Item
{
    private GameObject playerTorch;

    public override void OnInteract(Interactor interactor, ItemType itemType = ItemType.None)
    {
        base.OnInteract(interactor, itemType);
        if (playerTorch == null) playerTorch = interactor.GetComponent<PlayerInfoHolder>().PlayerTorch;
        playerTorch.SetActive(true);
    }

    public override void UseItem(GameObject user)
    {
        GameObject playerTorchLight = playerTorch.transform.GetChild(0).gameObject;
        playerTorchLight.SetActive(!playerTorchLight.activeInHierarchy);
    }

    public override void DropItem(Vector3 newPos)
    {
        playerTorch.SetActive(false);
        playerTorch.transform.GetChild(0).gameObject.SetActive(false);
        base.DropItem(newPos);
    }
}
