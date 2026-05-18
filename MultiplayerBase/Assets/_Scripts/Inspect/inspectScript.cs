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
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        if (Physics.Raycast(transform.position, fwd, out hit, rayLength, layerMaskInteract.value))
        {
            if (hit.collider.CompareTag("InteractObject"))
            {
                if (!doOnce)
                {
                    raycastedObj = hit.collider.gameObject.GetComponent<ObjectController>();
                    raycastedObj.ShowObjectName(inspectController);
                    CrosshairChange(true);
                }

                isCrosshairActive = true;
                doOnce = true;

                if (Input.GetMouseButtonDown(0))
                {
                    raycastedObj.ShowExtraInfo(inspectController);
                }
            }
        }
        else
        {
            if(isCrosshairActive)
            {
                raycastedObj.HideObjectName(inspectController);
                CrosshairChange(false);
                doOnce = false;
            }
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

