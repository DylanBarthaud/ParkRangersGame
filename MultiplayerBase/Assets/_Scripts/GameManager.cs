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

    [ServerRpc(RequireOwnership = false)]
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

        if(numberOfPlayers <= 0)
        {
            EndGame();
            return; 
        }

        List<BlackboardKey> spectatorKeys = new List<BlackboardKey>();
        spectatorKeys.Add(key);

        List<ulong> spectatePlayerIds = new List<ulong>();
        Blackboard blackboard = BlackboardController.instance.GetBlackboard(); 
        if (blackboard.TryGetValue(key, out PlayerInfo killedPlayerInfo))
        {
            spectatePlayerIds.Add(killedPlayerInfo.id);
            foreach(var spectatorKey in killedPlayerInfo.spectators)
            {
                spectatorKeys.Add(spectatorKey);
                if (blackboard.TryGetValue(spectatorKey, out PlayerInfo spectatorInfo)) spectatePlayerIds.Add(spectatorInfo.id); 
            }
        }
        else return;

        BlackboardKey playerToSpectateKey = playerBlackboardKeys[0];
        BlackboardKey[] spectatorKeysArray = spectatorKeys.ToArray();
        ulong[] spectatePlayerIdsArray = spectatePlayerIds.ToArray(); 
        EnableSpectatorModeClientRpc(spectatorKeysArray, spectatePlayerIdsArray, numberOfPlayers, playerToSpectateKey);  
    }

    [ClientRpc]
    private void EnableSpectatorModeClientRpc(BlackboardKey[] keys, ulong[] spectatorIds, int numOfPlayers, BlackboardKey playerToSpectateKey)
    {
        Debug.Log("IN CLIENT RPC" + NetworkManager.Singleton.LocalClientId + ", " + spectatorIds);

        bool needsToSpectate = false;
        foreach (ulong id in spectatorIds)
        {
            if (id == NetworkManager.Singleton.LocalClientId) { needsToSpectate = true; break; }
        }
        if(!needsToSpectate) return;

        Debug.Log("IS OWNER"); 

        Blackboard blackboard = BlackboardController.instance.GetBlackboard();
        if (blackboard.TryGetValue(keys[0], out PlayerInfo killedPlayerInfo))
        {
            Debug.Log("CAM OFF");
            killedPlayerInfo.playerCamera.enabled = false;
        }

        if (numOfPlayers != 0)
        {
            Debug.Log("NUM OF PLAYER > 0");

            if (blackboard.TryGetValue(playerToSpectateKey, out PlayerInfo playerInfo))
            {
                Debug.Log("SPECTATE ON");
                playerInfo.playerCamera.enabled = true;
                foreach (var key in keys) playerInfo.spectators.Add(key);
                uiManager.SetSpectatePanelOn(); 
            }
        }
    }

    private void EndGame()
    {
        playerBlackboardKeys.Clear();

        NetworkManager.Singleton.SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
