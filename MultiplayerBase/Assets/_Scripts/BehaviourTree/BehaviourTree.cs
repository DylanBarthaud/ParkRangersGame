using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTrees
{
    public class Root : Node
    {
        public Root(string name) : base(name) { }

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

        public Leaf(String name, IStrategy strategy) : base(name)
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
        public readonly List<Node> children = new();
        protected int currentChild;
        
        public Node(string name = "Node")
        {
            this.name = name;
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
