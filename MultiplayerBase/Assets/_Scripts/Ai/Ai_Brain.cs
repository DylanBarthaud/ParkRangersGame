using BehaviourTrees;
using BlackboardSystem;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AiState { NULL, Neutral, Searching, Investigating, Hunting, Chasing}

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Ai_Brain : MonoBehaviour, IExpert
{
    [SerializeField] protected BlackboardController blackboardController;
    protected Blackboard blackboard;

    protected NavMeshAgent agent;

    protected Root neutral_Root;
    protected Root searching_Root;
    protected Root hunt_Root;
    protected Root chasePlayer_Root;

    private AiState state; 

    #region stat_vars
    [SerializeField] protected float intelligence;
    [SerializeField] protected float aggrestion;
    #endregion

    protected void BaseAwake()
    {
        agent = GetComponent<NavMeshAgent>();

        blackboard = blackboardController.GetBlackboard();
        blackboardController.RegisterExpert(this);
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
    protected abstract void Searching();
    protected abstract void Neutral();

    protected void ChangeState(AiState state)
    {
        this.state = state;
    }

    #region IExpert Implimentation
    public int GetInsistence(Blackboard blackboard)
    {
        return 0; 
    }

    public void Execute(Blackboard blackboard)
    {

    }
    #endregion
}