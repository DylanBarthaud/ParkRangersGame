using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTrees
{
    public interface IStrategy
    {
        Node.Status Process();
        void Reset();
    }

    public class WanderStrategy : IStrategy
    {
        readonly Transform entity;
        readonly NavMeshAgent agent;
        readonly List<Transform> patrolPoints;
        int currentIndex;
        bool isPathCalculated; 

        public WanderStrategy(Transform entity, NavMeshAgent agent, List<Transform> patrolPoints)
        {
            this.entity = entity;
            this.agent = agent;
            this.patrolPoints = patrolPoints;
        }

        public Node.Status Process()
        {
            if (currentIndex == patrolPoints.Count)
            {
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
