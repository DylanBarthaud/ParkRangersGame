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

    public event Action<int> onTick;  
    public event Action<int> onTick_5;
    public event Action<PlayerInfo> onPlayerSpawned;
    public void OnTick(int tick)
    {
        if(onTick != null) onTick(tick);
    }
    public void OnTick_5(int tick)
    {
        if (onTick_5 != null) onTick_5(tick);
    }
    public void OnPlayerSpawned(PlayerInfo player)
    {
        if(onPlayerSpawned != null) onPlayerSpawned(player);
    }
}
