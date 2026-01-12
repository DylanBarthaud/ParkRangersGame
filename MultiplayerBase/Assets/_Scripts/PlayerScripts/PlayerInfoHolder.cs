using BlackboardSystem;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerInfoHolder : NetworkBehaviour, IAiViewable, IHurtable
{
    [SerializeField] int playerHealth; 

    private BlackboardKey playerInfo_Key;
    private PlayerInfo playerInfo;

    private bool isDead = false; 

    public override void OnNetworkSpawn()
    {
        string clientIDString = OwnerClientId.ToString();
        playerInfo_Key = new BlackboardKey("Player" + clientIDString + "InfoKey");

        playerInfo = new PlayerInfo();
        playerInfo.position = transform.position;
        playerInfo.playerCamera = transform.GetChild(0).GetChild(0).GetComponent<Camera>();
        playerInfo.health = playerHealth;
        playerInfo.id = OwnerClientId;
        playerInfo.spectators = new List<BlackboardKey>();
        playerInfo.ravenCount = 0;
        playerInfo.maxRavens = 20; 
        UpdateInfo(false, 0);

        EventManager.instance.OnPlayerSpawned(playerInfo_Key);
        EventManager.instance.onTick_5 += OnTick_5;
    }

    int localRavenTick = 0; 
    private void OnTick_5(int tick)
    {
        playerInfo.position = transform.position; 
        BlackboardController.instance.GetBlackboard().SetValue(playerInfo_Key, playerInfo);

        localRavenTick++;
        if (playerInfo.ravenCount < playerInfo.maxRavens &&
            localRavenTick >= 20)
        {
            playerInfo.ravenCount++;
            Debug.Log("Raven count: " + playerInfo.ravenCount);
            localRavenTick = 0; 
        }
    }

    public void UpdateInfo(bool playerSeen, int importance = -1)
    {
        if (this == null || gameObject == null) return;
        if (!this) return;

        playerInfo.position = gameObject.transform.position;
        playerInfo.canSeePlayer = playerSeen;
        if (importance != -1) playerInfo.importance = importance;
    }

    private int GetImportance(Vector3 aiPos)
    {
        float distanceToAi = Vector3.Distance(transform.position, aiPos);

        int importance = 200 - (int)distanceToAi;

        return importance;
    }

    public int GetImportance(Ai_Senses caller)
    {
        if (caller == null || this == null || gameObject == null) return 0;

        Vector3 aiPos = caller.transform.position;
        return GetImportance(aiPos); 
    }

    public PlayerInfo GetPlayerInfo()
    {
        return playerInfo;
    }

    private void UpdateBlackboard(Blackboard blackboard, bool playerSeen, int importance)
    {
        UpdateInfo(playerSeen, importance);

        blackboard.AddAction(() =>
        {
            blackboard.SetValue(playerInfo_Key, playerInfo);
        });
    }

    #region IAiViewable implimentation
    public void OnSeen(Blackboard blackboard, Ai_Senses caller)
    {
        if(isDead) return;
        bool playerSeen = true;

        Vector3 aiPos = caller.transform.position;
        int importance = GetImportance(aiPos);

        UpdateBlackboard(blackboard, playerSeen, importance);
    }

    public void OnUnSeen(Blackboard blackboard, Ai_Senses caller)
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

    #region IHurtable implimentation
    public void IsHurt(string caller, int amount)
    {
        if (isDead) return;
        playerInfo.health -= amount;
        if (caller == "Monster_Ai") playerInfo.ravenCount += 50;

        Blackboard blackboard = BlackboardController.instance.GetBlackboard();
        blackboard.AddAction(() =>
        {
            blackboard.SetValue(playerInfo_Key, playerInfo);
        });

        if(playerHealth <= 0)
        {
            IsKilled(); 
        }
    }

    public void IsKilled()
    {
        if(isDead) return;
        EventManager.instance.OnPlayerKilled(playerInfo_Key);

        isDead = true;
        gameObject.GetComponent<Collider>().enabled = false;
        //disable regular graphics and enable corpse graphics

        var blackboard = BlackboardController.instance?.GetBlackboard();
        if (blackboard != null)
        {
            blackboard.Remove(playerInfo_Key);
        }

        EventManager.instance.onTick_5 -= OnTick_5;
    }
    #endregion
}