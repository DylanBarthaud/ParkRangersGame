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
                audioHandler.PlaySound("BadgerShout"); 
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
                audioHandler.PlaySound("BadgerShout");
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
        }

        public Node.Status Process()
        {
            if(targetPosition == Vector3.zero
                || !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                GridPosition cell = getCellFunc();
                //Debug.Log(cell.x + ", " + cell.z);
                targetPosition = GameManager.instance.mapHandler.GetRandomLocationInGridPosition(cell);
            }

            agent.SetDestination(targetPosition);
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
        readonly float moveSpeed;
        private bool destinationSet = false;

        public MoveToPositionInCellStrategy(Func<GridPosition> getCellFunc, NavMeshAgent agent, float moveSpeed)
        {
            this.getCellFunc = getCellFunc;
            this.agent = agent;
            this.moveSpeed = moveSpeed;
        }

        public Node.Status Process()
        {
            if (!destinationSet)
            {
                GridPosition cell = getCellFunc();
                //Debug.Log(cell.x + ", " + cell.z);

                Vector3 targetPos = GameManager.instance.mapHandler.GetRandomLocationInGridPosition(cell);

                agent.speed = moveSpeed;
                agent.SetDestination(targetPos);

                destinationSet = true;
            }

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                Debug.Log("Arrived");
                return Node.Status.Success;
            }

            return Node.Status.Running;
        }

        public void Reset()
        {
            destinationSet = false;
        }
    }
}
