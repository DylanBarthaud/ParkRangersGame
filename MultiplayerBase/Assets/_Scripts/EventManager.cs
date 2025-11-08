using BlackboardSystem;
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

    /// <summary>
    /// Tick that happens every 0.2s
    /// </summary>
    /// <param name="tick"> number of ticks passed </param>
    public event Action<int> onTick;
    /// <summary>
    /// Tick that happens every 1s
    /// </summary>
    /// <param name="tick"> number of ticks passed </param>
    public event Action<int> onTick_5;
    public event Action<BlackboardKey> onPlayerSpawned;
    public event Action<BlackboardKey> onPlayerKilled;
    public event Action onButtonPressed; 
    public event Action<int,Interactor> onButtonHeld;
    public event Action onButtonReleased;

    /// <summary>
    /// Tick that happens every 0.2s
    /// </summary>
    /// <param name="tick"> 0.2s </param>
    public void OnTick(int tick)
    {
        if(onTick != null) onTick(tick);
    }
    /// <summary>
    /// Tick that happens every 1s
    /// </summary>
    /// <param name="tick"></param>
    public void OnTick_5(int tick)
    {
        if (onTick_5 != null) onTick_5(tick);
    }
    public void OnPlayerSpawned(BlackboardKey playerKey)
    {
        if(onPlayerSpawned != null) onPlayerSpawned(playerKey);
    }
    public void OnPlayerKilled(BlackboardKey playerKey)
    {
        if (onPlayerKilled != null) onPlayerKilled(playerKey);
    }
    public void OnButtonPressed()
    {
        if(onButtonPressed != null) onButtonPressed();
    }
    public void OnButtonHeld(int tick, Interactor interactor)
    {
        if(onButtonHeld != null) onButtonHeld(tick, interactor);
    }
    public void OnButtonReleased()
    {
        if(onButtonReleased != null) onButtonReleased();
    }
}
