using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Ai_testScript : NetworkBehaviour
{
    private Transform targetPos;

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        EventManager.instance.onPlayerSpawned += OnPlayerSpawned; 
    }

    private void OnPlayerSpawned(GameObject player)
    {
        targetPos = player.transform; 
    }

    void Update()
    {
        if (!IsHost) { return; }
        if (targetPos == null) { return; }

        agent.destination = targetPos.position;
    }
}
