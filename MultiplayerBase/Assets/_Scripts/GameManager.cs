using BlackboardSystem;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public MapHandler mapHandler;
    [Header("Grid")]
    [SerializeField] int width; 
    [SerializeField] int height;
    [SerializeField] float cellSize;

    public int numberOfPlayers;
    public List<BlackboardKey> playerBlackboardKeys;

    private int buttonsPressed = 0; 

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        mapHandler = new MapHandler(width, height, cellSize);

        EventManager.instance.onPlayerSpawned += OnPlayerSpawned;
        EventManager.instance.onPlayerKilled += OnPlayerKilled;
        EventManager.instance.onButtonPressed += OnButtonPressed;
    }

    private void OnButtonPressed()
    {
        buttonsPressed++;
        Debug.Log("BUTTON PRESSED");

        if (buttonsPressed >= 5)
        {
            Debug.Log("YOU WIN!"); 
            EndGame();
        }
    }

    private void OnPlayerSpawned(BlackboardKey key)
    {
        numberOfPlayers++;
        playerBlackboardKeys.Add(key);
    }

    private void OnPlayerKilled(BlackboardKey key)
    {
        numberOfPlayers--;
        playerBlackboardKeys.Remove(key);

        Blackboard blackboard = BlackboardController.instance.GetBlackboard();
        if(blackboard.TryGetValue(key, out PlayerInfo killedPlayerInfo))
        {
            killedPlayerInfo.playerCamera.enabled = false;
        }

        if(numberOfPlayers != 0)
        {
            if(blackboard.TryGetValue(playerBlackboardKeys[0], out PlayerInfo playerInfo))
            {
                playerInfo.playerCamera.enabled = true;
            }
        }

        if(numberOfPlayers == 0 && NetworkManager.Singleton.IsHost)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        playerBlackboardKeys.Clear();

        NetworkManager.Singleton.SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
