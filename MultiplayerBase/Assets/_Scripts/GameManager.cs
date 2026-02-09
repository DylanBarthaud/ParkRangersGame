using BlackboardSystem;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SerializableKeysAndValues<Tkey, Tval>
{
    [SerializeField] private Tkey _key;
    public Tkey Key => _key;

    [SerializeField] private Tval _val;
    public Tval Value => _val;
}

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    [Header("Gameplay")]
    [SerializeField] int taskCompleteNeeded = 5;
    [SerializeField] List<SerializableKeysAndValues<MiniGameTypes, GameObject>> miniGames;
    private Dictionary<MiniGameTypes, GameObject> miniGameDictionary = new Dictionary<MiniGameTypes, GameObject>();

    public MapHandler mapHandler;
    [SerializeField] Terrain terrain;
    [Header("Grid")]
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] float cellSize;
    [Header("Spawns")]
    [SerializeField] GameObject[] spawnableObjects;
    [SerializeField] int numberOfSpawns;
    [SerializeField, Range(0f, 180f)] float maxSteepness;  

    [HideInInspector] public int numberOfPlayers = 0;
    [HideInInspector] public List<BlackboardKey> playerBlackboardKeys;

    public UIManager uiManager;
    private int buttonsPressed = 0;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        EventManager.instance.onPlayerSpawned += OnPlayerSpawnedServerRpc;
        EventManager.instance.onPlayerKilled += OnPlayerKilledServerRpc;
        EventManager.instance.onPuzzleComplete += OnPuzzleComplete;


        foreach (var kv in miniGames)
        {
            miniGameDictionary.Add(kv.Key, kv.Value);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsHost) return;
        mapHandler = new MapHandler(width, height, cellSize, terrain, maxSteepness, spawnableObjects, numberOfSpawns);

        ChangeButtonsPressedUIClientRpc(0); 
    }


    public void SpawnObjectOnNetwork(GameObject obj, Vector3 pos, Quaternion rot, bool destroyWithScene = true)
    {
        GameObject spawnedObject = Instantiate(obj, pos, rot);
        spawnedObject.GetComponent<NetworkObject>().Spawn(destroyWithScene);

    }

    public void EnableMiniGame(MiniGameTypes gameType, GameObject caller)
    {
        GameObject miniGamePanel = miniGameDictionary[gameType];
        uiManager.EnableMiniGameUi(miniGamePanel, caller);
    }

    public void DisableMiniGame(MiniGameTypes gameType)
    {
        GameObject miniGamePanel = miniGameDictionary[gameType];
        uiManager.DisableMiniGameUi(miniGamePanel);
    }

    private void OnPuzzleComplete(bool s, IInteractable puzzle)
    {
        if (!s) return;
        OnPuzzleCompleteServerRpc(); 
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnPuzzleCompleteServerRpc()
    {
        buttonsPressed++;

        ChangeButtonsPressedUIClientRpc(buttonsPressed); 

        if (buttonsPressed >= taskCompleteNeeded)
        {
            Debug.Log("YOU WIN!"); 
            EndGame();
        }
    }

    [ClientRpc]
    private void ChangeButtonsPressedUIClientRpc(int buttons)
    {
        uiManager.ButtonsPressedText.text = buttons.ToString() + " / " + taskCompleteNeeded;
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnPlayerSpawnedServerRpc(BlackboardKey key)
    {
        numberOfPlayers++;
        Debug.Log("Number of players: " + numberOfPlayers);
        playerBlackboardKeys.Add(key);
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnPlayerKilledServerRpc(BlackboardKey key)
    {
        numberOfPlayers--;
        playerBlackboardKeys.Remove(key);
        //Debug.Log(numberOfPlayers);

        if(numberOfPlayers <= 0)
        {
            EndGame();
            return; 
        }

        BlackboardKey playerToSpectateKey = playerBlackboardKeys[0];
        List<ulong> spectatePlayerIds = new List<ulong>();
        Blackboard blackboard = BlackboardController.instance.GetBlackboard(); 
        if (blackboard.TryGetValue(key, out PlayerInfo killedPlayerInfo))
        {
            spectatePlayerIds.Add(killedPlayerInfo.id);
            foreach(var spectatorId in killedPlayerInfo.spectatorIds)
            {
                spectatePlayerIds.Add(spectatorId);
                if (blackboard.TryGetValue(playerToSpectateKey, out PlayerInfo playerInfo)) playerInfo.spectatorIds.Add(spectatorId);
            }
        }
        else return;

        ulong[] spectatePlayerIdsArray = spectatePlayerIds.ToArray(); 
        EnableSpectatorModeClientRpc(spectatePlayerIdsArray, numberOfPlayers, playerToSpectateKey);
    }

    [ClientRpc]
    private void EnableSpectatorModeClientRpc(ulong[] spectatorIds, int numOfPlayers, BlackboardKey playerToSpectateKey)
    {
        bool needsToSpectate = false;
        foreach (ulong id in spectatorIds)
        {
            if (id == NetworkManager.Singleton.LocalClientId) { needsToSpectate = true; break; }
        }
        if(!needsToSpectate) return;

        Blackboard blackboard = BlackboardController.instance.GetBlackboard();
        if (numOfPlayers != 0)
        {
            if (blackboard.TryGetValue(playerToSpectateKey, out PlayerInfo playerInfo))
            {
                //Debug.Log("SPECTATE ON");
                playerInfo.playerCamera.enabled = true;
                uiManager.SetSpectatePanelOn(); 
            }
        }
    }

    public AudioClip GetAudioClip(ulong callerId)
    {
        List<AudioClip> audioClipList = new List<AudioClip>();

        Blackboard blackboard = BlackboardController.instance.GetBlackboard();
        foreach (BlackboardKey key in playerBlackboardKeys)
        {
            if(blackboard.TryGetValue(key, out PlayerInfo playerInfo))
            {
                if (playerInfo.id == callerId) continue;

                AudioClip newClip = playerInfo.voiceInputController.CreatePlayerVoiceClip(); 
                if(newClip != null) audioClipList.Add(newClip);
            }
        }

        if(audioClipList.Count <= 0) return null;

        int roll = UnityEngine.Random.Range(0, audioClipList.Count);
        return audioClipList[roll];   
    }

    private void EndGame()
    {
        playerBlackboardKeys.Clear();
        numberOfPlayers = 0;

        NetworkManager.Singleton.SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
