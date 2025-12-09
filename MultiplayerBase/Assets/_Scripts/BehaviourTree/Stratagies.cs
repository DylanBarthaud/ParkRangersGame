using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTrees
{
    public interface IStrategy
    {
        Node.Status Process();
        void Reset() { }
    }

    public class Condition : IStrategy {
        readonly Func<bool> predicate;
        
        public Condition(Func<bool> predicate) {
            this.predicate = predicate;
        }

        public Node.Status Process()
        {
            Node.Status status = predicate() ? Node.Status.Success : Node.Status.Failure;
            return status;
        }
    }

    public class ActionStrategy : IStrategy 
    {
        readonly Action action;

        public ActionStrategy(Action action)
        {
            this.action = action;
        }

        public Node.Status Process()
        {
            action();
            return Node.Status.Success;
        }
    }

    public class PatrolStrategy : IStrategy
    {
        readonly Transform entity;
        readonly NavMeshAgent agent;
        readonly List<Transform> patrolPoints;
        int currentIndex;
        bool isPathCalculated; 

        public PatrolStrategy(Transform entity, NavMeshAgent agent, List<Transform> patrolPoints)
        {
            this.entity = entity;
            this.agent = agent;
            this.patrolPoints = patrolPoints;
        }

        public Node.Status Process()
        {
            if (currentIndex == patrolPoints.Count)
            {
                Reset();
                return Node.Status.Success;
            }

            var target = patrolPoints[currentIndex];
            agent.SetDestination(target.position); 

            if(isPathCalculated && agent.remainingDistance < 0.5f)
            {
                currentIndex++;
                isPathCalculated = false;
            }

            if (agent.pathStatus != NavMeshPathStatus.PathInvalid)
            {
                isPathCalculated = true;
            }

            return Node.Status.Running;
        }

        public void Reset()
        {
            currentIndex = 0;
        }
    }

    public class ChasePlayerStrategy : IStrategy 
    {
        readonly Func<PlayerInfo> playerInfoFunc;
        readonly NavMeshAgent agent;
        readonly float chaseSpeed; 

        public ChasePlayerStrategy(Func<PlayerInfo> playerInfo, NavMeshAgent agent, float chaseSpeed)
        {
            this.playerInfoFunc = playerInfo;
            this.agent = agent;
            this.chaseSpeed = chaseSpeed;
        }

        public Node.Status Process()
        {
            PlayerInfo playerInfo = playerInfoFunc();

            if (!playerInfo.canSeePlayer)
            {
                return Node.Status.Failure;
            }

            agent.SetDestination(playerInfo.position);
            agent.speed = chaseSpeed;  
            return Node.Status.Running;
        }
    }

    public class StalkPlayerStrategy : IStrategy
    {
        readonly Func<PlayerInfo> playerInfoFunc;
        readonly NavMeshAgent agent;
        readonly float stalkSpeed;
        readonly float stalkMinDistance; 
        readonly float stalkMaxDistance; 
        readonly float maxStalkTime;
        readonly AudioHandler audioHandler;
        private float stalkTime;

        public StalkPlayerStrategy(Func<PlayerInfo> playerInfoFunc, NavMeshAgent agent, float stalkSpeed, float maxStalkTime, float stalkMinDistance, float stalkMaxDistance, AudioHandler audioHandler)
        {
            this.playerInfoFunc = playerInfoFunc;
            this.agent = agent;
            this.stalkSpeed = stalkSpeed;
            this.maxStalkTime = maxStalkTime;
            this.stalkMinDistance = stalkMinDistance;
            this.stalkMaxDistance = stalkMaxDistance;
            this.audioHandler = audioHandler;
        }

        public Node.Status Process()
        {
            if (stalkTime >= maxStalkTime)
            {
                agent.isStopped = false;
                audioHandler.PlaySoundClientRpc("BadgerShout"); 
                return Node.Status.Success;
            }
            PlayerInfo playerInfo = playerInfoFunc();
            if (!playerInfo.canSeePlayer)
            {
                agent.isStopped = false;
                return Node.Status.Failure;
            }

            agent.SetDestination(playerInfo.position);
            float distanceFromPlayer = Vector3.Distance(agent.transform.position, playerInfo.position);
            if(distanceFromPlayer >= stalkMinDistance
                && distanceFromPlayer <= stalkMaxDistance)
            {
                agent.isStopped = true; 
            }
            else if(distanceFromPlayer <= stalkMinDistance)
            {
                agent.isStopped = false;
                return Node.Status.Success;
            }
            else agent.isStopped = false;

            stalkTime += Time.deltaTime; 

            return Node.Status.Running;
        }

        public void Reset()
        {
            stalkTime = 0;
        }
    }

    public class SearchCellStrategy : IStrategy
    {
        readonly Func<GridPosition> getCellFunc;
        readonly NavMeshAgent agent; 
        Vector3 targetPosition; 

        public SearchCellStrategy(Func<GridPosition> getCellFunc, NavMeshAgent agent)
        {
            this.getCellFunc = getCellFunc;
            this.agent = agent;
            targetPosition = Vector3.zero; 
        }

        public Node.Status Process()
        {
            if(targetPosition == Vector3.zero)
            {
                GridPosition cell = getCellFunc();
                //Debug.Log(cell.x + ", " + cell.z);
                targetPosition = GameManager.instance.mapHandler.GetRandomLocationInGridPosition(cell);
            }

            agent.SetDestination(targetPosition);

            if(!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                return Node.Status.Success;
            }

            return Node.Status.Running;
        }

        public void Reset()
        {
            targetPosition = Vector3.zero;
        }
    }

    public class MoveToPositionInCellStrategy : IStrategy
    {
        readonly Func<GridPosition> getCellFunc;
        readonly NavMeshAgent agent;
        readonly float? moveSpeed;
        private bool destinationSet = false;
        private Vector3 targetPos; 

        public MoveToPositionInCellStrategy(Func<GridPosition> getCellFunc, NavMeshAgent agent, float? moveSpeed)
        {
            this.getCellFunc = getCellFunc;
            this.agent = agent;
            this.moveSpeed = moveSpeed;
        }

        public Node.Status Process()
        {
            MapHandler mapHandler = GameManager.instance.mapHandler;

            if (!destinationSet)
            {
                GridPosition cell = getCellFunc();
                //Debug.Log(cell.x + ", " + cell.z);

                targetPos = mapHandler.GetRandomLocationInGridPosition(cell);

                if(moveSpeed != null) agent.speed = (float)moveSpeed;
                agent.SetDestination(targetPos);

                destinationSet = true;
            }

            GridPosition agentCellPos = mapHandler.GetGridLocation(agent.transform.position);
            GridPosition targetCell = mapHandler.GetGridLocation(targetPos); 

            if(agentCellPos == targetCell)
            {
                return Node.Status.Success;
            }

            return Node.Status.Running;
        }

        public void Reset()
        {
            destinationSet = false;
        }
    }

    public class BurrowStrategy : IStrategy
    {
        readonly NavMeshAgent agent;
        readonly float moveSpeed;
        readonly float burrowTime;
        readonly Func<bool> isBurrowedFunc;
        readonly GFXHandler gfxHandler;
        readonly AudioHandler audioHandler;

        public BurrowStrategy(NavMeshAgent agent, float moveSpeed, float burrowTime, Func<bool> isBurrowedFunc, GFXHandler gFXHandler, AudioHandler audioHandler)
        {
            this.agent = agent;
            this.moveSpeed = moveSpeed;
            this.burrowTime = burrowTime;
            this.isBurrowedFunc = isBurrowedFunc;
            gfxHandler = gFXHandler;
            this.audioHandler = audioHandler;
        }

        public Node.Status Process()
        { 
            if(isBurrowedFunc()) return Node.Status.Success;

            EventManager.instance.OnBurrow(agent.transform.position); 

            //play digging animation 
            //wait "burrow time" amout of seconds
            gfxHandler.DisableGFXClientRpc("MonsterGFX");
            gfxHandler.EnableGFXClientRpc("BurrowedGFX");

            audioHandler.StopPlayingClipSoundClientRpc("BadgerWalking");
            audioHandler.PlaySoundClientRpc("BadgerDigging", true, 0.5f, default, 20); 

            agent.gameObject.GetComponent<Collider>().enabled = false;

            agent.speed = moveSpeed;

            return Node.Status.Success;
        }
    }

    public class UnBurrowStrategy : IStrategy
    {
        readonly NavMeshAgent agent;
        readonly float moveSpeed;
        readonly float unBurrowTime;
        readonly Func<bool> isBurrowedFunc;
        readonly GFXHandler gfxHandler;
        readonly AudioHandler audioHandler;

        public UnBurrowStrategy(NavMeshAgent agent, float moveSpeed, float unBurrowTime, Func<bool> isBurrowedFunc, GFXHandler gFXHandler, AudioHandler audioHandler)
        {
            this.agent = agent;
            this.moveSpeed = moveSpeed;
            this.unBurrowTime = unBurrowTime;
            this.isBurrowedFunc = isBurrowedFunc;
            gfxHandler = gFXHandler;
            this.audioHandler = audioHandler;
        }

        public Node.Status Process()
        {
            if (!isBurrowedFunc()) return Node.Status.Success;

            EventManager.instance.OnUnBurrow(agent.transform.position);

            //play digging animation 
            //wait "unburrow time" amout of seconds
            gfxHandler.DisableGFXClientRpc("BurrowedGFX");
            gfxHandler.EnableGFXClientRpc("MonsterGFX");

            //audioHandler.PlaySound("UnBurrowSound"); 
            //audioHandler.StopPlayingClipSoundClientRpc("BadgerDigging");
            audioHandler.PlaySoundClientRpc("BadgerWalking", true, 1, 5, 15);

            agent.gameObject.GetComponent<Collider>().enabled = true;

            agent.speed = moveSpeed;

            return Node.Status.Success;
        }
    }
}