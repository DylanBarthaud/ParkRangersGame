using BlackboardSystem;
using System;
using UnityEngine;

public interface IAiViewable
{
    public int GetImportance(Ai_Eyes caller);
    public void OnSeen(Blackboard blackboard, Ai_Eyes caller); 
    public void OnUnSeen(Blackboard blackboard, Ai_Eyes caller);
}
