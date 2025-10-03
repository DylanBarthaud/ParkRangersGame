using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    private void Awake()
    {
        if (instance == null) instance = this; 
    }

    public event Action<GameObject> onPlayerSpawned; 
    public void OnPlayerSpawned(GameObject player)
    {
        if(onPlayerSpawned != null)
        {
            onPlayerSpawned(player);
        }
    }
}
