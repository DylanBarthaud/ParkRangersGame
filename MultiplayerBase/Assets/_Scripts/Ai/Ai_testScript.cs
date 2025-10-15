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
    [SerializeField] List<Transform> waypoints;
    [SerializeField] List<Transform> waypointsTwo;

    private NavMeshAgent agent;

    Root root;

    [SerializeField] BlackboardController blackboardController;
    Blackboard blackboard;
    BlackboardKey patrolKey;

    bool canParolBool; 

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        blackboard = blackboardController.GetBlackboard();
        blackboardController.RegisterExpert(this);
        patrolKey = blackboard.GetOrRegisterKey("PatrolKey");

        #region BehaviourTree Logic

        root = new Root("Root");
        PrioritySelector chasePlayerOrPatrolSelector = new PrioritySelector("chasePlayerOrPatrolSelector");
        #region ChasePlayer Sequence
        PrioritySelector choosePlayerToChaseSelector = new PrioritySelector("ChoosePlayerToChaseSelector");
        bool CanChasePlayerOne()
        {
            BlackboardKey playerInfoKey = blackboard.GetOrRegisterKey("Player0InfoKey");

            if (blackboard.TryGetValue(playerInfoKey, out PlayerInfo playerInfo))
            { 
                if(playerInfo.canSeePlayer)
                {
                    Debug.Log("CAN CHASE");
                    return true;
                }
            }

            return false; 
        }
        IfGate canChasePlayerOne = new IfGate("CanChasePlayerOne", new Condition(() => CanChasePlayerOne()), 100);
        void ChasePlayer()
        {
            BlackboardKey playerOneInfoKey = blackboard.GetOrRegisterKey("Player0InfoKey");

            if (blackboard.TryGetValue(playerOneInfoKey, out PlayerInfo playerInfo))
            {
                agent.SetDestination(playerInfo.position);
            }
        }
        Leaf chasePlayer = new Leaf("ChasePlayer", new ActionStrategy(() => ChasePlayer()));

        canChasePlayerOne.AddChild(chasePlayer);
        choosePlayerToChaseSelector.AddChild(canChasePlayerOne);
        chasePlayerOrPatrolSelector.AddChild(choosePlayerToChaseSelector);

        #endregion

        root.AddChild(chasePlayerOrPatrolSelector);

        #endregion
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