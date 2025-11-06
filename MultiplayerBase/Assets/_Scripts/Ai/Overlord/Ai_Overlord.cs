using BehaviourTrees;
using BlackboardSystem;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Ai_Overlord : MonoBehaviour, IExpert
{
    #region Time Consts
    private const int ONE_MINUTE = 60; 
    private const int TEN_MINUTES = 600; 
    private const int ONE_HOUR = 3600;
    #endregion

    [SerializeField] private string keyName; 

    private BlackboardKey key; 
    private OverlordGivenInfo infoPackage;

    [SerializeField] private float distanceToGiveHint; 
    private int insistance = 0; 

    Root root;

    private int tick; 

    private void Awake()
    {
        BlackboardController.instance.RegisterExpert(this); 
        Blackboard blackboard = BlackboardController.instance.GetBlackboard();

        key = blackboard.GetOrRegisterKey(keyName); 

        EventManager.instance.onTick_5 += OnTick;

        #region Behaviour Tree

        root = new Root("Overlord_Root");
        PrioritySelector primaryOverlordSelector = new PrioritySelector("Primary_Overlord_Selector"); 

        bool isPastGivenTime(int time)
        {
            if(tick >= time) return true;
            return false;
        }
        IfGate pastFiveMins = new IfGate("PastFiveMinsGate", new Condition(() => isPastGivenTime(ONE_MINUTE * 5)), 5); 
        IfGate pastTenMins = new IfGate("PastTenMinsGate", new Condition(() => isPastGivenTime(TEN_MINUTES)), 10); 
        IfGate pastTwentyMins = new IfGate("PastTwentyMinsGate", new Condition(() => isPastGivenTime(TEN_MINUTES * 2)), 20);
        IfGate pastOneHour = new IfGate("PastOneHourGate", new Condition(() => isPastGivenTime(ONE_HOUR)), 60);

        bool MonsterIsFarFromPlayers()
        {
            MapHandler mapHandler = GameManager.instance.mapHandler; 

            GridPosition aiPosition = new GridPosition { x = 0, z = 0 }; 
            BlackboardKey monsterKey = blackboard.GetOrRegisterKey("AiMonsterKey");
            if (blackboard.TryGetValue(monsterKey, out AiInfo monsterInfo))
            {
                aiPosition = mapHandler.GetGridLocation(monsterInfo.position);
            }

            float shortestDistance = 1000; 
            foreach (BlackboardKey playerKey in GameManager.instance.playerBlackboardKeys)
            {
                if (blackboard.TryGetValue(playerKey, out PlayerInfo playerInfo))
                {
                    GridPosition playerPosition = mapHandler.GetGridLocation(playerInfo.position);
                    float distance = mapHandler.GetDistanceBetweenGrids(aiPosition, playerPosition);

                    if(distance < shortestDistance) shortestDistance = distance;
                }
            }

            float distanceToGiveHintSq = distanceToGiveHint * distanceToGiveHint; 
            bool isFarAway = shortestDistance > distanceToGiveHintSq ? true : false;

            //Debug.Log(shortestDistance + " , " + isFarAway);
            return isFarAway; 
        }
        IfGate monsterFarFromPlayers = new IfGate("MonsterIsFarFromPlayersGate", new Condition(MonsterIsFarFromPlayers));

        PlayerInfo GetLowestFearedPlayerInfo()
        {
            PlayerInfo currentPlayerInfo = new PlayerInfo { fear = -1 };

            foreach(BlackboardKey key in GameManager.instance.playerBlackboardKeys)
            {
                if(blackboard.TryGetValue(key, out PlayerInfo playerInfo))
                {
                    if(playerInfo.fear > currentPlayerInfo.fear) currentPlayerInfo = playerInfo;
                }
            }

            return currentPlayerInfo;
        }
        Vector3 GetPositionHint(PlayerInfo playerInfo)
        {
            MapHandler mapHandler = GameManager.instance.mapHandler;

            GridPosition playerGridPosition = mapHandler.GetGridLocation(playerInfo.position); 
            //Debug.Log(playerGridPosition.x + "," + playerGridPosition.z);
            return mapHandler.GetRandomLocationInGridPosition(playerGridPosition);
        }
        Leaf givePlayerPositionHint = new Leaf("GivePlayerPositionHintLeaf", new ActionStrategy(() => 
        {
            infoPackage.playerPositionHint = GetPositionHint(GetLowestFearedPlayerInfo());
            insistance = 90; 
        }));

        root.AddChild(primaryOverlordSelector); 

        primaryOverlordSelector.AddChild(monsterFarFromPlayers);
        monsterFarFromPlayers.AddChild(givePlayerPositionHint);
        #endregion
    }

    #region Events
    private void OnTick(int tick)
    {
        this.tick = tick;
        root.Process(); 
    }
    #endregion

    #region Expert implimentation
    public void Execute(Blackboard blackboard)
    {
        blackboard.AddAction(() =>
        {
            blackboard.SetValue(key, infoPackage);
        });

        insistance = 0; 
    }

    public int GetInsistence(Blackboard blackboard)
    {
        return insistance; 
    }
    #endregion
}