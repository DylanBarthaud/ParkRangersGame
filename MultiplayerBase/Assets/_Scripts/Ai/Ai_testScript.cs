using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTrees;
using BlackboardSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class Ai_testScript : NetworkBehaviour, IExpert
{
    private NavMeshAgent agent;

    Root root;

    [SerializeField] BlackboardController blackboardController;
    Blackboard blackboard;
    BlackboardKey AiMonsterKey;
    AiInfo aiInfo;

    bool canParolBool;

    int playerAmount = 0; 

    private void Awake()
    {
        EventManager.instance.onPlayerSpawned += OnPlayerSpawned;
        EventManager.instance.onTick_5 += OnTick_5;

        agent = GetComponent<NavMeshAgent>();

        aiInfo = new AiInfo()
        {
            position = transform.position,
        };

        blackboard = blackboardController.GetBlackboard();
        blackboardController.RegisterExpert(this);
        AiMonsterKey = blackboard.GetOrRegisterKey("AiMonsterKey");
        blackboard.SetValue(AiMonsterKey, aiInfo); 

        #region Behaviour Tree

        root = new Root("Root");

        PrioritySelector prioritySelector = new PrioritySelector("PrioritySelector"); 

        #region ChasePlayer Sequence
        PlayerInfo PlayerInfo() 
        {
            List<PlayerInfo> seenPlayers = new List<PlayerInfo>(); 
            for(int i = 0; i < playerAmount; i++)
            {
                string playerIdToString = i.ToString();
                BlackboardKey key = blackboard.GetOrRegisterKey("Player" + playerIdToString + "InfoKey");

                if(blackboard.TryGetValue(key, out PlayerInfo info))
                {
                    if (info.canSeePlayer) seenPlayers.Add(info);
                }
            }

            if (seenPlayers.Count == 0)
            {
                PlayerInfo placeHolderInfo = new PlayerInfo()
                {
                    canSeePlayer = false,
                    importance = 0,
                    position = Vector3.zero 
                };

                return placeHolderInfo;
            }

            PlayerInfo playerInfo = new PlayerInfo(); 
            foreach(PlayerInfo player in seenPlayers)
            {
                if(player.importance > playerInfo.importance)
                {
                    playerInfo = player;
                }
            }

            return playerInfo;
        }
        Leaf chasePlayer = new Leaf("ChasePlayer", new ChasePlayerStrategy(PlayerInfo, agent), 100);
        #endregion

        Leaf moveToPos0 = new Leaf("MoveToPos", new ActionStrategy(() => { agent.SetDestination(Vector3.zero); }), 50);
        void MoveToGridPos()
        {
            BlackboardKey overlordKey = blackboard.GetOrRegisterKey("AiOverlordKey");

            if (blackboard.TryGetValue(overlordKey, out OverlordGivenInfo overlordInfo))
            {
                agent.SetDestination(overlordInfo.playerPositionHint);
            }
        }
        Leaf moveToPos = new Leaf("MoveToPos", new ActionStrategy(MoveToGridPos), 50);

        prioritySelector.AddChild(chasePlayer);
        prioritySelector.AddChild(moveToPos);

        root.AddChild(prioritySelector);

        #endregion
    }

    private void OnTick_5(int obj)
    {
        aiInfo = new AiInfo()
        {
            position = transform.position,
        };

        blackboard.SetValue(AiMonsterKey, aiInfo);
    }

    private void Update()
    {
        if (!IsHost) return;
        root.Process();
    }

    private void OnPlayerSpawned(BlackboardKey playerInfo)
    {
        playerAmount++; 
    }

    #region IExpert implimentation

    public int GetInsistence(Blackboard blackboard)
    {
       int insistence = 0;
       return insistence;
    }

    public void Execute(Blackboard blackboard)
    {
    }

    #endregion
}