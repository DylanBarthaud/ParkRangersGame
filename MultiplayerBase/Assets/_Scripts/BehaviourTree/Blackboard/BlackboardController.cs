using System;
using System.Collections.Generic; 
using UnityEngine;

namespace BlackboardSystem
{
    public class BlackboardController : MonoBehaviour
    {
        public static BlackboardController instance;

        [SerializeField] BlackboardData blackboardData;
        readonly Blackboard blackboard = new Blackboard();
        readonly Arbiter arbiter = new Arbiter();

        private void Awake()
        {
            if(instance == null) instance = this; 

            blackboardData.SetValuesOnBlackboard(blackboard);
        }

        public Blackboard GetBlackboard() => blackboard;

        public void RegisterExpert(IExpert expert) => arbiter.RegisterExpert(expert);

        private void Update()
        {
            IterateBlackboard(); 
        }

        private void IterateBlackboard()
        {
            foreach (var action in arbiter.BlackboardIteration(blackboard))
            {
                action();
            }

            blackboard.ClearActions();
        }
    }
}
