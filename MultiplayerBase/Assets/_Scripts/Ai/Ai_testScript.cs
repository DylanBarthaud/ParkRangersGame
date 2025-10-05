using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTrees; 

[RequireComponent(typeof(NavMeshAgent))]
public class Ai_testScript : NetworkBehaviour
{
    [SerializeField] List<Transform> waypoints; 
    private NavMeshAgent agent;
    Root root;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        root = new Root("Test"); 
        root.AddChild(new Leaf("Wander", new WanderStrategy(transform, agent, waypoints)));
    }

    private void Update()
    {
        root.Process(); 
    }
}
