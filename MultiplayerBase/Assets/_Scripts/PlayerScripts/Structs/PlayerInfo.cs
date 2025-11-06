using BlackboardSystem;
using System;
using UnityEngine;

[Serializable]
public struct PlayerInfo
{
    public Camera playerCamera;
    public Vector3 position;
    public int health; 
    public bool canSeePlayer;
    public int importance;
    public int fear; 
    public ulong id; 
}
