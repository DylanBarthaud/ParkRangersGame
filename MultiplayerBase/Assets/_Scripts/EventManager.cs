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

    public event Action<bool, IInteractable> onPuzzleComplete; 

    public event Action<int,Interactor> onButtonHeld;
    public event Action onButtonReleased;

    public event Action<Vector3> onBurrow;  
    public event Action<Vector3> onUnBurrow;

    public event Action<string> onQuestComplete; 

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

    public void OnPuzzleComplete(bool success = true, IInteractable puzzle = null)
    {
        if(onPuzzleComplete != null) onPuzzleComplete(success, puzzle);
    }
    public void OnButtonHeld(int tick, Interactor interactor)
    {
        if(onButtonHeld != null) onButtonHeld(tick, interactor);
    }
    public void OnButtonReleased()
    {
        if(onButtonReleased != null) onButtonReleased();
    }

    public void OnBurrow(Vector3 burrowPos)
    {
        if (onBurrow != null) onBurrow(burrowPos); 
    }
    public void OnUnBurrow(Vector3 unBurrowPos)
    {
        if (onBurrow != null) onUnBurrow(unBurrowPos);

    }

    public void OnQuestComplete(string quest)
    {
        if(onQuestComplete != null) onQuestComplete(quest);
    }
}
