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

    [SerializeField] private InspectController inspectController;

    private void Awake()
    {
        EventManager.instance.playerSpawningComplete += SetPlayer;
    }

    private void SetPlayer()
    {
        Blackboard blackboard = BlackboardController.instance.GetBlackboard();
        List<BlackboardKey> playerKeys = GameManager.instance.playerBlackboardKeys;
        foreach (BlackboardKey key in playerKeys)
        {
            if (blackboard.TryGetValue(key, out PlayerInfo playerInfo))
            {
                if (playerInfo.id == NetworkManager.Singleton.LocalClientId)
                {
                    Debug.Log(playerInfo.id); 
                    inspectController = playerInfo.inspectController;
                    break; 
                }
            }
        }
    }
    public void ShowObjectName()
    {
        inspectController.ShowName(itemName);
        Debug.Log(itemName);
    }

    public void HideObjectName()
    {
        inspectController.HideName();
    }

    public void ShowExtraInfo()
    {
        inspectController.ShowAdditionalInfo(itemextraInfo);
    }

}
