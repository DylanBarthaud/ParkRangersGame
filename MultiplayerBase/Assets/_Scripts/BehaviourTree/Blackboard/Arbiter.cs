using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlackboardSystem
{
    public class Arbiter
    {
        readonly List<IExpert> experts = new();

        public void RegisterExpert(IExpert expert)
        {
            experts.Add(expert);
        }

        public List<Action> BlackboardIteration(Blackboard blackboard)
        {
            IExpert bestExpert = null;
            int highestInsistence = 0; 

            foreach (var expert in experts)
            {
                int insistence = expert.GetInsistence(blackboard);

                if (insistence > highestInsistence)
                {
                    highestInsistence = insistence;
                    bestExpert = expert;
                }
            }

            bestExpert?.Execute(blackboard); 

            var actions = blackboard.passedActions;

            return actions;
        }
    }
}
