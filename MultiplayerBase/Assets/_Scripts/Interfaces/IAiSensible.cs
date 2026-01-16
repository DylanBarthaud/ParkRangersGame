using BlackboardSystem;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface IAiSensible
{
    public int GetImportance(Ai_Senses caller);
    public void OnSeen(Blackboard blackboard, Ai_Senses caller); 
    public void OnUnSeen(Blackboard blackboard, Ai_Senses caller);
    /// <returns> Loudest heard audio squared </returns>
    public float GetAudioDataSquared();
}
