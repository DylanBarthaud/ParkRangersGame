using NUnit.Framework;
using System;
using System.Collections.Generic;
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
}
