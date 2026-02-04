using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using BlackboardSystem;

public class PlayerSpawner : NetworkBehaviour
{
    private const bool DESTROY_WITH_SCENE_BOOL = true;

    [SerializeField] private GameObject playerPrefab;
    private Blackboard blackboard;

    private void Awake()
    {
        blackboard = BlackboardController.instance.GetBlackboard();
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnSceneLoaded;
    }

    private void OnSceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (!IsHost || sceneName != "MainGame") return;

        int i = 0; 
        foreach(ulong id in clientsCompleted)
        {
            GameObject player = Instantiate(playerPrefab, new Vector3(1475 + i * 5, 85, 1300), Quaternion.identity);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(id, DESTROY_WITH_SCENE_BOOL);

            PlayerInfo info = player.GetComponent<PlayerInfoHolder>().GetPlayerInfo();

            string idAsString = id.ToString();
            string keyName = "Player" + idAsString + "InfoKey";

            BlackboardKey key = blackboard.GetOrRegisterKey(keyName);

            blackboard.SetValue(key, info);
            EventManager.instance.OnPlayerSpawned(key);
            i++;
        }
    }
}
