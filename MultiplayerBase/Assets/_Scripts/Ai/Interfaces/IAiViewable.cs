using BlackboardSystem;
using System;
using UnityEngine;

public interface IAiViewable
{
    public int GetImportance(Vector3 viewersPos);
    public void OnSeen(Blackboard blackboard); 
}
