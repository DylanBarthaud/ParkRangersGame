using BlackboardSystem;
using System;
using UnityEngine;

[Serializable]
public struct PlayerInfo
{
    public Vector3 position;
    public bool canSeePlayer;
    public int importance;
    public int fear; 
    public ulong id; 
    public BlackboardKey key; 
}
