using System;
using System.Collections.Generic; 
using UnityEngine;

namespace BlackboardSystem
{
    public class BlackboardController : MonoBehaviour
    {
        [SerializeField] BlackboardData blackboardData;
        readonly Blackboard blackboard = new Blackboard();
        readonly Arbiter arbiter = new Arbiter();

        private void Awake()
        {
            blackboardData.SetValuesOnBlackboard(blackboard);
        }

        public Blackboard GetBlackboard() => blackboard;

        public void RegisterExpert(IExpert expert) => arbiter.RegisterExpert(expert);

        private void Update()
        {
            foreach(var action in arbiter.BlackboardIteration(blackboard))
            {
                action();  
            }
        }
    }
}
