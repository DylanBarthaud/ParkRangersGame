using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Splines;
using BlackboardSystem;
using System.Collections.Generic;
using Unity.Netcode;
using System;
using Steamworks.ServerList;
public class ObjectController : MonoBehaviour
{
    [SerializeField] private string itemName;

    [TextArea][SerializeField] private string itemextraInfo;

    public void ShowObjectName(InspectController inspectController)
    {
        inspectController.ShowName(itemName);
        Debug.Log(itemName);
    }

    public void HideObjectName(InspectController inspectController)
    {
        inspectController.HideName();
    }

    public void ShowExtraInfo(InspectController inspectController)
    {
        inspectController.ShowAdditionalInfo(itemextraInfo);
    }

}
