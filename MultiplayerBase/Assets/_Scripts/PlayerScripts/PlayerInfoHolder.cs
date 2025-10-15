using BlackboardSystem;
using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerInfoHolder : NetworkBehaviour, IAiViewable
{
    private BlackboardKey playerInfo_Key;
    private PlayerInfo playerInfo;

    public override void OnNetworkSpawn()
    {
        string clientIDString = OwnerClientId.ToString();
        Debug.Log("Player" + clientIDString + "InfoKey"); 
        playerInfo_Key = new BlackboardKey("Player" + clientIDString + "InfoKey");
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        playerInfo_Key = new BlackboardKey("NULL"); 
    }

    public void UpdateInfo(bool playerSeen)
    {
        playerInfo = new PlayerInfo() 
        { 
            position = gameObject.transform.position,
            canSeePlayer = playerSeen
        };
    }

    public int GetImportance(Vector3 aiPosition)
    {
        float distanceToAi = Vector3.Distance(transform.position, aiPosition);

        int importance = 100 - (int)distanceToAi;
        Debug.Log(importance); 

        return importance; 
    }

    public void OnSeen(Blackboard blackboard)
    {
        UpdateInfo(true);

        blackboard.AddAction(() =>
        {
            blackboard.SetValue(playerInfo_Key, playerInfo);
        });
    }
}