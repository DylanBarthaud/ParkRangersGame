using BlackboardSystem;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInfoHolder : NetworkBehaviour, IAiViewable
{
    private BlackboardKey playerInfo_Key;
    private PlayerInfo playerInfo;

    public override void OnNetworkSpawn()
    {
        UpdateInfo(false, 0);

        EventManager.instance.OnPlayerSpawned(playerInfo); 

        string clientIDString = OwnerClientId.ToString();
        playerInfo_Key = new BlackboardKey("Player" + clientIDString + "InfoKey");
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        playerInfo_Key = new BlackboardKey("NULL"); 
    }

    public void UpdateInfo(bool playerSeen, int importance)
    {
        playerInfo = new PlayerInfo() 
        { 
            position = gameObject.transform.position,
            canSeePlayer = playerSeen,
            importance = importance
        };
    }

    private int GetImportance(Vector3 aiPos)
    {
        float distanceToAi = Vector3.Distance(transform.position, aiPos);

        int importance = 100 - (int)distanceToAi;

        return importance;
    }

    public int GetImportance(Ai_Eyes caller)
    {
        Vector3 aiPos = caller.transform.position;
        return GetImportance(aiPos); 
    }

    public void OnSeen(Blackboard blackboard, Ai_Eyes caller)
    {
        bool playerSeen = true;

        Vector3 aiPos = caller.transform.position;
        int importance = GetImportance(aiPos);

        UpdateBlackboard(blackboard, playerSeen, importance);
    }

    public void OnUnSeen(Blackboard blackboard, Ai_Eyes caller)
    {
        bool playerSeen = false;

        int importance = 0; 
        if(blackboard.TryGetValue(playerInfo_Key, out PlayerInfo playerBlackboardInfo))
        {
            if(playerBlackboardInfo.canSeePlayer != playerSeen)
            {
                importance = 200;
            }
            else importance = 0;
        }

        UpdateBlackboard(blackboard, playerSeen, importance); 
    }

    private void UpdateBlackboard(Blackboard blackboard, bool playerSeen, int importance)
    {
        UpdateInfo(playerSeen, importance);

        blackboard.AddAction(() =>
        {
            blackboard.SetValue(playerInfo_Key, playerInfo);
        });
    }
}