using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum AiState { NULL, Neutral, Searching, Investigating, Hunting, Chasing}

[RequireComponent(typeof(Ai_Controller))]
public abstract class Ai_Brain : MonoBehaviour
{
    private List<GameObject> playerList;
    private GameObject focusedPlayer;

    private Ai_Controller controller;

    private AiState state; 

    #region stat_vars
    [SerializeField] private float senseRange;
    [SerializeField] private float intelligence;
    [SerializeField] private float aggrestion;
    #endregion

    private void Start()
    {
        controller = GetComponent<Ai_Controller>();

        EventManager.instance.onPlayerSpawned += AddPlayerToList;
    }

    private void AddPlayerToList(GameObject player)
    {
        playerList.Add(player); 
    }

    private void Update()
    {
        switch (state)
        {
            case AiState.NULL:
                Debug.LogWarning(gameObject.name + " AiState"); 
                break;
            case AiState.Neutral:
                Neutral(); 
                break;
            case AiState.Searching:
                Searching();
                break;
            case AiState.Investigating:
                Investigate(); 
                break; 
            case AiState.Hunting:
                Hunt();
                break; 
            case AiState.Chasing:
                ChasePlayer();
                break; 
        }
    }

    protected abstract void ChasePlayer();
    protected abstract void Hunt();
    protected abstract void Investigate();
    protected abstract void Searching();
    protected abstract void Neutral();
}