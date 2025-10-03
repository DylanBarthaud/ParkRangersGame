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

    private void ChasePlayer()
    {
        //when close enough
        //runs towards targeted player
        //attempts to predict where player will run and use shortcuts??? 
        //indicates player with loud noise 

        throw new NotImplementedException();
    }

    private void Hunt()
    {
        //when a player is within sense range of the ai the ai begins to hunt them 
        //The ai stalks them and attempts to lure them in

        throw new NotImplementedException();
    }

    private void Investigate()
    {
        //after monster finds clue it is able to start moving towards the player who left clue
        //Indicates player that a clue has been found (through howling noise or similar)
        //finding move clues gives ai more precise and recent location of player 

        throw new NotImplementedException();
    }

    private void Searching()
    {
        //Look around for signs of recent player activity
        //(footprints , tasks done , doors opened)
        //If monster cant find players for x amount of time (lowered with higher aggrestion) ai is steered towards players

        throw new NotImplementedException();
    }

    private void Neutral()
    {
        throw new NotImplementedException();
    }
}