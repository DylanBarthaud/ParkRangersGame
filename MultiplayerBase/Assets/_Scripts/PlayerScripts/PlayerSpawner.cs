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
    [SerializeField] private BlackboardController blackboardController;
    private Blackboard blackboard;

    private void Awake()
    {
        blackboard = blackboardController.GetBlackboard();
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
    }

    private void OnSceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (!IsHost) return;

        foreach(ulong id in clientsCompleted)
        {
            GameObject player = Instantiate(playerPrefab);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(id, DESTROY_WITH_SCENE_BOOL);

            string idAsString = id.ToString();
            string keyName = "Player" + idAsString + "InfoKey";

            blackboard.GetOrRegisterKey(keyName); 
        }
    }
}
