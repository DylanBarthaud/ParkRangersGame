using BlackboardSystem;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerInfoHolder : NetworkBehaviour, IAiViewable
{
    private BlackboardKey playerInfo_Key;
    private PlayerInfo playerInfo;

    public override void OnNetworkSpawn()
    {
        string clientIDString = OwnerClientId.ToString();
        playerInfo_Key = new BlackboardKey("Player" + clientIDString + "InfoKey");

        playerInfo = new PlayerInfo();
        playerInfo.id = OwnerClientId;
        playerInfo.key = playerInfo_Key;
        UpdateInfo(false, 0, 0);

        EventManager.instance.OnPlayerSpawned(playerInfo);
        EventManager.instance.onTick += OnTick;
    }

    private void OnTick(int tick)
    {
        if(playerInfo.fear > 0)
        {
            playerInfo.fear--;
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }

    public void UpdateInfo(bool playerSeen, int importance = -1, int fear = -1)
    {
        playerInfo.position = gameObject.transform.position;
        playerInfo.canSeePlayer = playerSeen;
        if (importance != -1) playerInfo.importance = importance;
        if (fear != -1) playerInfo.fear = fear;
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

    #region IAiViewable implimentation
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
    #endregion

    private void UpdateBlackboard(Blackboard blackboard, bool playerSeen, int importance)
    {
        UpdateInfo(playerSeen, importance);

        blackboard.AddAction(() =>
        {
            blackboard.SetValue(playerInfo_Key, playerInfo);
        });
    }
}