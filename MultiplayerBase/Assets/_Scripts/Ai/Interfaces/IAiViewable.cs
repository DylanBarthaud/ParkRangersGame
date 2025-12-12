using BlackboardSystem;
using System;
using UnityEngine;

public interface IAiViewable
{
    public int GetImportance(Ai_Senses caller);
    public void OnSeen(Blackboard blackboard, Ai_Senses caller); 
    public void OnUnSeen(Blackboard blackboard, Ai_Senses caller);
}
