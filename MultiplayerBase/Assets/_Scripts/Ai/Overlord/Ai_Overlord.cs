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

    [SerializeField] private BlackboardController blackBoardController;
    private BlackboardKey key; 
    private List<BlackboardKey> playerBlackboardKeys;
    private OverlordGivenInfo infoPackage; 

    Root root;

    private int tick; 

    private void Awake()
    {
        blackBoardController.RegisterExpert(this); 
        Blackboard blackboard = blackBoardController.GetBlackboard();

        playerBlackboardKeys = new List<BlackboardKey>();
        key = blackboard.GetOrRegisterKey(keyName); 

        EventManager.instance.onPlayerSpawned += OnPlayerSpawned;
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

        //TODO
        bool MonsterIsFarFromPlayers()
        {
            return true; 
        }
        IfGate monsterFarFromPlayers = new IfGate("MonsterIsFarFromPlayersGate", new Condition(MonsterIsFarFromPlayers));

        PlayerInfo GetLowestFearedPlayerInfo()
        {
            PlayerInfo currentPlayerInfo = new PlayerInfo { fear = 0 };

            foreach(BlackboardKey key in playerBlackboardKeys)
            {
                if(blackboard.TryGetValue(key, out PlayerInfo playerInfo))
                {
                    if(playerInfo.fear > currentPlayerInfo.fear) currentPlayerInfo = playerInfo;
                }
            }

            return currentPlayerInfo;
        }
        //TODO
        Vector3 GetPositionHint(PlayerInfo playerInfo)
        {
            return Vector3.back;
        }
        Leaf givePlayerPositionHint = new Leaf("GivePlayerPositionHintLeaf", new ActionStrategy(() => { infoPackage.playerPositionHint = GetPositionHint(GetLowestFearedPlayerInfo()); }));

        #endregion
    }

    #region Events
    private void OnTick(int tick)
    {
        this.tick = tick;
    }

    private void OnPlayerSpawned(PlayerInfo info)
    {
        playerBlackboardKeys[(int)info.id] = info.key;
    }
    #endregion

    #region Expert implimentation
    public void Execute(Blackboard blackboard)
    {
        blackboard.AddAction(() =>
        {
            blackboard.SetValue(key, infoPackage);
        });
    }

    public int GetInsistence(Blackboard blackboard)
    {
        return 0; 
    }
    #endregion
}