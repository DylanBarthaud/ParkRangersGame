using BlackboardSystem;
using System;
using System.Collections.Generic;
using Unity.Netcode;
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

        ChangeButtonsPressedUIClientRpc(buttonsPressed); 

        if (buttonsPressed >= 5)
        {
            Debug.Log("YOU WIN!"); 
            EndGame();
        }
    }

    [ClientRpc]
    private void ChangeButtonsPressedUIClientRpc(int buttons)
    {
        uiManager.ButtonsPressedText.text = buttons.ToString() + " / 5";
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnPlayerSpawnedServerRpc(BlackboardKey key)
    {
        numberOfPlayers++;
        playerBlackboardKeys.Add(key);
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnPlayerKilledServerRpc(BlackboardKey key)
    {
        Debug.Log("PLAYER KILLED SERVER RPC"); 
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

        EnableSpectatorModeClientRpc(key, killedPlayerId);  
    }

    [ClientRpc]
    private void EnableSpectatorModeClientRpc(BlackboardKey key, ulong clientId)
    {
        Debug.Log("IN CLIENT RPC" + NetworkManager.Singleton.LocalClientId + ", " + clientId);
        if (clientId != NetworkManager.Singleton.LocalClientId) return;
        Debug.Log("IS OWNER"); 

        Blackboard blackboard = BlackboardController.instance.GetBlackboard();
        if (blackboard.TryGetValue(key, out PlayerInfo killedPlayerInfo))
        {
            Debug.Log("CAM OFF");
            killedPlayerInfo.playerCamera.enabled = false;
        }

        if (blackboard.TryGetValue(playerBlackboardKeys[0], out PlayerInfo playerInfo))
        {
            Debug.Log("SPECTATE ON");
            playerInfo.playerCamera.enabled = true;
            uiManager.SetSpectatePanelOn();
        }
    }

    private void EndGame()
    {
        playerBlackboardKeys.Clear();

        NetworkManager.Singleton.SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
