using BlackboardSystem;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    public MapHandler mapHandler;
    [Header("Grid")]
    [SerializeField] int width; 
    [SerializeField] int height;
    [SerializeField] float cellSize;

    [HideInInspector] public int numberOfPlayers;
    [HideInInspector] public List<BlackboardKey> playerBlackboardKeys;

    public UIManager uiManager;
    private int buttonsPressed = 0;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        mapHandler = new MapHandler(width, height, cellSize);

        EventManager.instance.onPlayerSpawned += OnPlayerSpawnedServerRpc;
        EventManager.instance.onPlayerKilled += OnPlayerKilledServerRpc;
        EventManager.instance.onButtonPressed += OnButtonPressedServerRpc;
    }

    [ServerRpc]
    private void OnButtonPressedServerRpc()
    {
        buttonsPressed++;
        //Debug.Log("BUTTON PRESSED");

        ChangeButtonsPressedUIClientRpc(); 

        if (buttonsPressed >= 5)
        {
            Debug.Log("YOU WIN!"); 
            EndGame();
        }
    }

    [ClientRpc]
    private void ChangeButtonsPressedUIClientRpc()
    {
        uiManager.ButtonsPressedText.text = buttonsPressed.ToString() + " / 5";
    }

    [ServerRpc]
    private void OnPlayerSpawnedServerRpc(BlackboardKey key)
    {
        numberOfPlayers++;
        playerBlackboardKeys.Add(key);
    }

    [ServerRpc]
    private void OnPlayerKilledServerRpc(BlackboardKey key)
    {
        numberOfPlayers--;
        playerBlackboardKeys.Remove(key);

        if(numberOfPlayers == 0)
        {
            EndGame();
        }

        ulong killedPlayerId = 10;
        if (BlackboardController.instance.GetBlackboard().TryGetValue(key, out PlayerInfo killedPlayerInfo))
        {
            killedPlayerId = killedPlayerInfo.id;
        }
        else return; 

        EnableSpectatorModeClientRpc(key, new ClientRpcParams 
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { killedPlayerId }
            }
        });  
    }

    [ClientRpc]
    private void EnableSpectatorModeClientRpc(BlackboardKey key, ClientRpcParams rpcParams = default)
    {
        Blackboard blackboard = BlackboardController.instance.GetBlackboard();
        if (blackboard.TryGetValue(key, out PlayerInfo killedPlayerInfo))
        {
            killedPlayerInfo.playerCamera.enabled = false;
        }

        if (numberOfPlayers != 0)
        {
            if (blackboard.TryGetValue(playerBlackboardKeys[0], out PlayerInfo playerInfo))
            {
                playerInfo.playerCamera.enabled = true;
            }
        }
    }

    private void EndGame()
    {
        playerBlackboardKeys.Clear();

        NetworkManager.Singleton.SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
