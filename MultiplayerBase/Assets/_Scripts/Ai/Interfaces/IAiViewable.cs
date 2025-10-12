using BlackboardSystem;
using UnityEngine;

public interface IAiViewable
{
    public int GetImportance();
    public void OnSeen(Blackboard blackboard); 
}
