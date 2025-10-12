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

        root = new Root("Test");

        Sequence patrolSeq = new Sequence("patrol");
        bool patrolable()
        {
            if (blackboard.TryGetValue(patrolKey, out bool patrolVal))
            {
                if (!patrolVal)
                {
                    patrolSeq.Reset();
                    return false;
                }
            }
            return true;
        }
        patrolSeq.AddChild(new Leaf("moveToPatrolPoints", new PatrolStrategy(transform, agent, waypoints), 10));

        Sequence runToSafetyseq = new Sequence("Run");
        runToSafetyseq.AddChild(new Leaf("canPatrol", new Condition(() => !patrolable())));
        runToSafetyseq.AddChild(new Leaf("moveToPatrolPointsTwo", new PatrolStrategy(transform, agent, waypointsTwo), 100));

        PrioritySelector actions = new PrioritySelector("actions");
        actions.AddChild(runToSafetyseq);
        actions.AddChild(patrolSeq);

        root.AddChild(actions);

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
