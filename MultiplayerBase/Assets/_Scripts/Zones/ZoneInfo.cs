using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ZoneInfo", menuName = "Scriptable Objects/ZoneInfo")]
public class ZoneInfo : ScriptableObject
{
    public Sprite image; 
    public Zones Zone;
    public int MaxPlayers;
    public int TasksNeeded;
    [HideInInspector] public int NumberOfPlayers;
    [HideInInspector] public List<string> playersNames = new List<string>(); 
    [HideInInspector] public List<ulong> playersIDs = new List<ulong>();
    [HideInInspector] public int TasksComplete;
    [HideInInspector] public bool zoneIsComplete = false;

    private void Awake()
    {
        ResetZoneInfo(); 
    }

    public void ResetZoneInfo()
    {
        NumberOfPlayers = 0;
        playersNames = new List<string>();
        playersIDs = new List<ulong>();
        TasksComplete = 0;
        zoneIsComplete = false;
    }
}