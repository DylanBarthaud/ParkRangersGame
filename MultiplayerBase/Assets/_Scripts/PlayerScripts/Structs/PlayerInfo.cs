using BlackboardSystem;
using NUnit.Framework;
using System;
using System.Collections.Generic;
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
    public List<BlackboardKey> spectators; 
}
