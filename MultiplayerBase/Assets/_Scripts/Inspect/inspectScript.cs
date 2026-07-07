using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class inspectScript : MonoBehaviour
{
    [SerializeField] private int rayLength = 5;
    [SerializeField] private LayerMask layerMaskInteract;
    private ObjectController raycastedObj;

    [SerializeField] private Image crosshair;
    private bool isCrosshairActive;
    private bool doOnce;

    [SerializeField] private InspectController inspectController;

    private void Update()
    {
        RaycastHit hit;
        Vector3 fwd = Camera.main.transform.TransformDirection(Vector3.forward);

        if (Physics.Raycast(Camera.main.transform.position, fwd, out hit, rayLength, layerMaskInteract.value))
        {
            if (hit.collider.CompareTag("InteractObject"))
            {
                IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();
                raycastedObj = hit.collider.gameObject.GetComponent<ObjectController>();

                bool canInteract;
                string cantInteractReason;

                Interactor interactor = gameObject.GetComponent<Interactor>();
                Inventory Inv = gameObject.GetComponent<Inventory>();
                ItemType typeInInv;
                if (Inv.Items.Count > 0)
                {
                    Item itemInInv;
                    if (Inv.CarryingHeavy) itemInInv = Inv.HeavyItem;
                    else itemInInv = Inv.Items[Inv.SelectedItemSlot];
                    typeInInv = itemInInv.ItemType;
                }
                else typeInInv = ItemType.None;

                (canInteract, cantInteractReason) = interactable.CanInteract(interactor, typeInInv);
                bool skipCanInteractCheck = false;
                if (interactable.RequiresZoneCheckIn())
                {
                    GameManager gameManager = GameManager.instance;
                    Zones objZone = gameManager.GridPosZoneDictonary[gameManager.mapHandler.GetGridLocation(hit.collider.transform.position)];
                    if (!gameManager.PlayerInSameZone(interactor.CurrentZone))
                    {
                        skipCanInteractCheck = true;
                        inspectController.ShowName($"Requires {objZone} CheckIn", Color.red);
                    }
                }
                if (!skipCanInteractCheck && !canInteract) inspectController.ShowName(cantInteractReason, Color.yellow);

                if (raycastedObj != null) raycastedObj.ShowObjectName(inspectController);
                CrosshairChange(true);

                if (!doOnce)
                {

                }

                isCrosshairActive = true;
                doOnce = true;

                if (Input.GetMouseButtonDown(0) && raycastedObj != null)
                {
                    raycastedObj.ShowExtraInfo(inspectController);
                }

                return;
            }
        }

        if (isCrosshairActive)
        {
            inspectController.HideName();
            CrosshairChange(false);
            doOnce = false;
        }
    }
    void CrosshairChange(bool on)
    {
        if (on && !doOnce)
        {
            crosshair.color = Color.red;
        }
        else
        {
            crosshair.color = Color.white;
            isCrosshairActive = false;
        }
    }
}