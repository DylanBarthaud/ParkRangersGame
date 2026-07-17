using BlackboardSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class PlayerInfoHolder : NetworkBehaviour, IAiSensible, IHurtable
{
    [Header("Player HealthSettings")]
    [SerializeField] int playerHealth;
    [SerializeField] GameObject injuredFilter;
    [SerializeField] Color startColor, endColor;
    [SerializeField] AudioSource[] audioSources;
    [SerializeField] VoiceInputController voiceInputController;
    [SerializeField] InspectController inspectController;
    [SerializeField] MultiplayerAudioHandlerWrapper multiplayerAudioHandler;
    [SerializeField] FirstPersonController firstPersonController;

    [SerializeField] GameObject playerCompass;
    [SerializeField] GameObject playerInv; 
    [SerializeField] private GameObject torchLight;

    [SerializeField] private GFXHandler gFXHandler;

    [Header("Raven Settings")]
    [SerializeField] int tryAddRavenTick = 2; 
    [SerializeField] int tryRemoveRavenTick = 5;
    int localLoseRavenTick;
    int localRavenTick;
    [SerializeField, Range(-60f, 0f)] float volumeToAddRavenGate = 0f; 

    private BlackboardKey playerInfo_Key;
    private PlayerInfo playerInfo;

    public bool isTorchActive => torchLight.activeInHierarchy; 

    public override void OnNetworkSpawn()
    {
        string clientIDString = OwnerClientId.ToString();
        playerInfo_Key = new BlackboardKey("Player" + clientIDString + "InfoKey");

        if(!IsOwner)
        {
            playerCompass.SetActive(false);
            playerInv.SetActive(false);
        }

        playerInfo = new PlayerInfo
        {
            position = transform.position,
            playerCamera = transform.GetChild(0).GetChild(0).GetComponent<Camera>(),
            audioListener = transform.GetComponent<AudioListener>(),
            health = playerHealth,
            id = OwnerClientId,
            spectatorIds = new List<ulong>(),
            ravenCount = 0,
            maxRavens = 10,
            voiceInputController = voiceInputController,
            inspectController = inspectController
        };
        UpdateInfo(false, 0);

        EventManager.instance.onTick_5 += OnTick_5;
        EventManager.instance.onTick += OnTick; 

        localRavenTick = 0;
        localLoseRavenTick = 0;
        injuredFilter.SetActive(false);
    }

    public override void OnNetworkDespawn()
    {
        EventManager.instance.onTick_5 -= OnTick_5;
        EventManager.instance.onTick -= OnTick;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && !isDead && IsOwner) IsKilled(); 
    }

    private void OnTick(int obj)
    {
        //Debug.Log(GetAudioDataSquared()); 
        if (GetAudioDataSquared() >= volumeToAddRavenGate 
            || firstPersonController.IsSprinting)
        {
            localRavenTick++;
            localLoseRavenTick = 0;
        }
        else localLoseRavenTick++;

        if (!IsOwner) return; 
        GridPosition playerGridPos = GameManager.instance.mapHandler.GetGridLocation(transform.position);
        GameManager.instance.playerGridPos = playerGridPos;
        //Debug.Log($"{playerGridPos.x} , {playerGridPos.z}");
    }

    private void OnTick_5(int tick)
    {
        playerInfo.position = transform.position; 
        BlackboardController.instance.GetBlackboard().SetValue(playerInfo_Key, playerInfo);

        if (playerInfo.ravenCount < playerInfo.maxRavens &&
            localRavenTick >= tryAddRavenTick &&
            GameManager.instance.mapHandler.GetGridLocation(transform.position) != GameManager.instance.HomeCell)
        {
            AddCrowServerRPC(); 
            localRavenTick = 0;
        }

        else if (localLoseRavenTick >= tryRemoveRavenTick &&
                 playerInfo.ravenCount > 0)
        {
            RemoveCrowServerRPC();
            localLoseRavenTick = 0;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddCrowServerRPC() => AddCrowClientRPC();

    [ClientRpc]
    private void AddCrowClientRPC() 
    {
        playerInfo.ravenCount++;
        if (playerInfo.ravenCount >= playerInfo.maxRavens) playerInfo.ravenCount = playerInfo.maxRavens;
        Debug.Log($"Add Raven to player {OwnerClientId} count: {playerInfo.ravenCount}");
    }

    [ServerRpc(RequireOwnership = false)]
    private void RemoveCrowServerRPC() => RemoveCrowClientRPC();

    [ClientRpc]
    private void RemoveCrowClientRPC() 
    {
        playerInfo.ravenCount--;
        if(playerInfo.ravenCount == 0) playerInfo.ravenCount = 0;
        //Debug.Log($"Remove Raven to player {OwnerClientId} count: {playerInfo.ravenCount}");
    }

    [ServerRpc(RequireOwnership = false)] public void ActivateTorchServerRPC(bool active) 
        => ActivateTorchClientRPC(active);
    [ClientRpc] private void ActivateTorchClientRPC(bool active) 
        => torchLight.SetActive(active);

    public void UpdateInfo(bool playerSeen, int importance = -1)
    {
        if (this == null || gameObject == null) return;
        if (!this) return;

        playerInfo.position = gameObject.transform.position;
        playerInfo.canSeePlayer = playerSeen;
        if (importance != -1) playerInfo.importance = importance;
    }

    private int GetImportance(Vector3 aiPos)
    {
        float distanceToAi = Vector3.Distance(transform.position, aiPos);

        int importance = 200 - (int)distanceToAi;

        return importance;
    }

    public int GetImportance(Ai_Senses caller)
    {
        if (caller == null || this == null || gameObject == null) return 0;

        Vector3 aiPos = caller.transform.position;
        return GetImportance(aiPos); 
    }

    public PlayerInfo GetPlayerInfo()
    {
        return playerInfo;
    }

    private void UpdateBlackboard(Blackboard blackboard, bool playerSeen, int importance)
    {
        UpdateInfo(playerSeen, importance);
        playerInfo.lastTimePlayerSeen = Time.time;

        blackboard.AddAction(() =>
        {
            blackboard.SetValue(playerInfo_Key, playerInfo);
        });
    }

    #region IAiSensible Implimentation

    public float GetAudioDataSquared()
    {
        float loudestHeardAudioSquared = -100f; 

        foreach(AudioSource audioSource  in audioSources)
        {
            if( audioSource == null) continue;
            if (audioSource.isPlaying && audioSource.maxDistance > loudestHeardAudioSquared)
                loudestHeardAudioSquared = audioSource.maxDistance * audioSource.maxDistance;
        }

        if (voiceInputController.GetVoiceVolumeSquared() > loudestHeardAudioSquared) 
            loudestHeardAudioSquared = voiceInputController.GetVoiceVolumeSquared();

        //Debug.Log(loudestHeardAudioSquared); 
        return loudestHeardAudioSquared;
    }

    public void OnSeen(Blackboard blackboard, Ai_Senses caller)
    {
        bool playerSeen = true;

        Vector3 aiPos = caller.transform.position;
        int importance = GetImportance(aiPos);

        UpdateBlackboard(blackboard, playerSeen, importance);
    }

    public void OnUnSeen(Blackboard blackboard, Ai_Senses caller)
    {
        bool playerSeen = false;

        int importance = 0; 
        if(blackboard.TryGetValue(playerInfo_Key, out PlayerInfo playerBlackboardInfo))
        {
            if(playerBlackboardInfo.canSeePlayer != playerSeen)
            {
                importance = 200;
            }
            else importance = 0;
        }

        UpdateBlackboard(blackboard, playerSeen, importance); 
    }
    #endregion

    #region IHurtable Implimentation
    public void IsHurt(string caller, int amount)
    {
        playerInfo.health -= amount;
        if (caller == "Monster_Ai") playerInfo.ravenCount = 0;

        Blackboard blackboard = BlackboardController.instance.GetBlackboard();
        blackboard.AddAction(() =>
        {
            blackboard.SetValue(playerInfo_Key, playerInfo);
        });

        if (playerInfo.health <= 50 && playerInfo.health > 0)
        {
            injuredFilter.SetActive(true);
            multiplayerAudioHandler.PlaySoundServerRpc("Scream");
            multiplayerAudioHandler.AudioHandler.PlaySound("Heartbeat");
            StartCoroutine(FadeOutDamagedOverlay()); 
        }

        if (playerInfo.health <= 0) IsKilled();

        else EventManager.instance.OnPlayerHurt(); 
    }

    private bool isDead = false;
    public void IsKilled()
    {
        if(isDead) return;
        isDead = true;
        if (IsOwner) EventManager.instance.OnPlayerKilled(playerInfo_Key);

        var blackboard = BlackboardController.instance?.GetBlackboard();
        if (blackboard != null)
        {
            blackboard.Remove(playerInfo_Key);
        }

        gFXHandler.DisableGFXServerRpc("AliveGFX");

        EventManager.instance.onTick_5 -= OnTick_5;
    }
    #endregion

    private IEnumerator FadeOutDamagedOverlay()
    {
        float t = 0;

        while(t < 1)
        {
            float lerped = Mathf.Lerp(2, 0.3f, t);
            injuredFilter.GetComponent<Image>().color = Color.Lerp(startColor, endColor, t);
            multiplayerAudioHandler.AudioHandler.ChangeAudioSourceVolume(2, lerped); 

            yield return null;
            t += Time.deltaTime; 
        }

        injuredFilter.SetActive(false);
    }

    private void OnApplicationQuit()
    {
        //NetworkObject.ChangeOwnership(0); 
        if(IsOwner) IsKilled();
    }
}