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
    private NavMeshAgent agent;
    Root root;
    readonly Blackboard blackboard = new Blackboard();
    BlackboardKey patrolKey; 

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        patrolKey = blackboard.GetOrRegisterKey("PatrolKey");
        blackboard.SetValue(patrolKey, false); 

        root = new Root("Test");

        Leaf canPatrol = new Leaf("canPatrol", new Condition(checkPatrolBool)); 
        Leaf moveToPatrolPoints = new Leaf("moveToPatrolPoints", new PatrolStrategy(transform, agent, waypoints), 100);
        Leaf moveToPos = new Leaf("moveToPoint", new ActionStrategy(() => agent.SetDestination(new Vector3(10, 1.1f, 100))), 10);

        Sequence move = new Sequence("patrol"); 
        move.AddChild(canPatrol);
        move.AddChild(moveToPatrolPoints);

        PrioritySelector actions = new PrioritySelector("actions");
        actions.AddChild(move);
        actions.AddChild(moveToPos);

        root.AddChild(actions);
    }

    private bool checkPatrolBool()
    {
        blackboard.TryGetValue(patrolKey, out bool patrolVal); 
        return patrolVal;
    }

    private void Update()
    {
        root.Process(); 

        if(Input.GetKeyDown(KeyCode.F))
        {
            if(blackboard.TryGetValue(patrolKey, out bool patrolVal))
            {
                blackboard.SetValue(patrolKey, !patrolVal);
                Debug.Log(patrolVal);
            }
        }
    }
}
