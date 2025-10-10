using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTrees;
using BlackboardSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class Ai_testScript : NetworkBehaviour
{
    [SerializeField] List<Transform> waypoints;
    [SerializeField] List<Transform> waypointsTwo;

    [SerializeField] BlackboardData blackboardData;

    private NavMeshAgent agent;

    Root root;
    readonly Blackboard blackboard = new Blackboard();
    BlackboardKey patrolKey; 

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        //blackboardData.SetValuesOnBlackboard(blackboard);
        patrolKey = blackboard.GetOrRegisterKey("PatrolKey");
        blackboard.SetValue(patrolKey, false);

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
    }
     
    private void Update()
    {
        root.Process(); 

        if(Input.GetKeyDown(KeyCode.F))
        {
            if(blackboard.TryGetValue(patrolKey, out bool patrolVal))
            {
                blackboard.SetValue(patrolKey, !patrolVal);
                Debug.Log(!patrolVal);
            }
        }
    }
}
