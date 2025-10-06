using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace BehaviourTrees
{
    public class PrioritySelector : Node
    {
        public PrioritySelector(string name, int priority = 0) : base(name, priority)
        {
        }

        List<Node> sortedChildren;
        List<Node> SortedChildren => sortedChildren ??= SortChildren();

        private List<Node> SortChildren() => children.OrderByDescending(child => child.priority).ToList();


        public override Status Process()
        {
            foreach (var child in SortedChildren)
            {
                switch (child.Process())
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Success:
                        return Status.Success;
                    default:
                        continue; 
                }
            }

            return Status.Failure;
        }

        public override void Reset()
        {
            base.Reset();
            sortedChildren = null;
        }
    }

    public class Selector : Node
    {
        public Selector(string name, int priority = 0) : base(name, priority)
        {
        }
        
        public override Status Process()
        {
            if (currentChild < children.Count)
            {
                switch (children[currentChild].Process())
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Success:
                        Reset(); 
                        return Status.Success;
                    case Status.Failure:
                        currentChild++;
                        return Status.Running;
                }
            }

            Reset();
            return Status.Success;
        }
    }

    public class Sequence : Node
    {
        public Sequence(string name, int priority = 0) : base(name, priority)
        {
        }

        public override Status Process()
        {
            if(currentChild < children.Count) 
            {
                switch (children[currentChild].Process())
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Failure:
                        Reset();
                        return Status.Failure;
                    case Status.Success:
                        currentChild++;
                        return currentChild == children.Count ? Status.Success : Status.Running;
                }  
            }

            Reset();
            return Status.Success;
        }
    }

    public class Root : Node
    {
        public Root(string name, int priority = 0) : base(name, priority) { }

        public override Status Process()
        {
            while (currentChild < children.Count)
            {
                var status = children[currentChild].Process();
                if(status != Status.Success)
                {
                    return status;
                }
                currentChild++;
            }
            return Status.Success;
        }
    }

    public class Leaf : Node
    {
        private IStrategy strategy;

        public Leaf(String name, IStrategy strategy, int priority = 0) : base(name, priority)
        {
            this.strategy = strategy;
        }

        public override Status Process()
        {
            return strategy.Process();
        }

        public override void Reset()
        {
            strategy.Reset();
        }
    }

    public class Node
    {
        public enum Status { Success , Failure , Running }

        private string name;
        public readonly int priority;  

        public readonly List<Node> children = new();
        protected int currentChild;
        
        public Node(string name = "Node", int priority = 0)
        {
            this.name = name;
            this.priority = priority;
        }

        public void AddChild(Node child) 
        { 
            children.Add(child);
        }

        public virtual Status Process() => children[currentChild].Process(); 

        public virtual void Reset()
        {
            foreach(var child in children)
            {
                child.Reset();
            }
        }
    }
}
