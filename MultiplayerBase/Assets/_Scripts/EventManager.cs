using System;
using Unity.VisualScripting;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    private void Awake()
    {
        if (instance == null) instance = this; 
    }

    public event Action<PlayerInfo> onPlayerSpawned; 
    public void OnPlayerSpawned(PlayerInfo player)
    {
        if(onPlayerSpawned != null)
        {
            onPlayerSpawned(player);
        }
    }
}
