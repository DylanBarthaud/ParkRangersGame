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
    BlackboardKey patrolKey;

    bool canParolBool;

    int playerAmount = 0; 

    private void Awake()
    {
        EventManager.instance.onPlayerSpawned += OnPlayerSpawned;

        agent = GetComponent<NavMeshAgent>();

        blackboard = blackboardController.GetBlackboard();
        blackboardController.RegisterExpert(this);
        patrolKey = blackboard.GetOrRegisterKey("PatrolKey");

        #region BehaviourTree Logic

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
                    if(info.canSeePlayer) seenPlayers.Add(info);
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
        Leaf chasePlayer = new Leaf("ChasePlayer", new ChasePlayerStrategy(() => PlayerInfo(), agent), 100);
        #endregion

        Leaf moveToPos = new Leaf("MoveToPos", new ActionStrategy(() => { agent.SetDestination(Vector3.zero); }), 50);

        prioritySelector.AddChild(chasePlayer);
        prioritySelector.AddChild(moveToPos);

        root.AddChild(prioritySelector);

        #endregion
    }

    private void OnPlayerSpawned(PlayerInfo playerInfo)
    {
        playerAmount++; 
    }

    private void Update()
    {
        if (!IsHost) return;
        root.Process(); 

        if(Input.GetKeyDown(KeyCode.F))
        {
            canParolBool = !canParolBool;
        }
    }

    #region IExpert implimentation

    public int GetInsistence(Blackboard blackboard)
    {
       int insistence = 0;
        
       if(blackboard.TryGetValue(patrolKey, out bool patrolVal))
       {
            if (patrolVal != canParolBool)
            {
                if (canParolBool) insistence = 100;
                else insistence = 10; 
            }
       }
       else
       {
           insistence = 0; 
       }

       return insistence;
    }

    public void Execute(Blackboard blackboard)
    {
        blackboard.AddAction(() =>
        {
            blackboard.SetValue(patrolKey, canParolBool);
        });
    }

    #endregion
}