using Netcode.Transports.Facepunch;
using NUnit.Framework;
using Steamworks;
using Steamworks.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SteamManager : MonoBehaviour
{
    [SerializeField] TMP_InputField lobbyIDInputField;
    [SerializeField] private TextMeshProUGUI lobbyId;
    [SerializeField] private TextMeshProUGUI peopleInLobby;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject inLobbyMenu;
    [SerializeField] private GameObject startMenu;

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] public UnityEngine.UI.Image loadingBarFill;

    private static int maxMembersInLobby = 8;

    private void OnEnable()
    {
        SteamMatchmaking.OnLobbyCreated += LobbyCreated;
        SteamMatchmaking.OnLobbyEntered += LobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += MemberJoinedLobby;
        SteamFriends.OnGameLobbyJoinRequested += GameLobbyJoinRequested;
    }
    private void MemberJoinedLobby(Lobby lobby, Friend friend)
    {
        Debug.Log(friend.ToString() + "joined");
        peopleInLobby.text = ""; 
        IEnumerable<Friend> friends = lobby.Members;
        foreach (Friend currentFriend in friends)
        {
            peopleInLobby.text += currentFriend.Name + "\n";
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        CheckUI(); 
    }

    private void GameLobbyJoinRequested(Lobby lobby, SteamId id)
    {
        lobby.Join();
    }

    private void LobbyCreated(Result result, Lobby lobby)
    {
        if (result == Result.OK)
        {
            lobby.SetPublic();
            lobby.SetJoinable(true);

            NetworkManager.Singleton.StartHost();
            //peopleInLobby.text = lobby.Owner.Name + "\n";
        }
    }

    private void LobbyEntered(Lobby lobby)
    {
        LobbySaver.instance.currentLobby = lobby;
        lobbyId.text = lobby.Id.ToString();
        peopleInLobby.text = "";
        CheckUI();
        if (NetworkManager.Singleton.IsHost) return;
        NetworkManager.Singleton.gameObject.GetComponent<FacepunchTransport>().targetSteamId = lobby.Owner.Id;
        NetworkManager.Singleton.StartClient();
    }

    public async void HostLobby()
    {
        await SteamMatchmaking.CreateLobbyAsync(maxMembersInLobby);
    }

    public async void JoinLobbyWithID()
    {
        ulong id;
        if (!ulong.TryParse(lobbyIDInputField.text, out id))
        {
            return;
        }

        Lobby[] lobbies = await SteamMatchmaking.LobbyList.WithSlotsAvailable(1).RequestAsync();
        foreach (var lobby in lobbies)
        {
            if (lobby.Id == id)
            {
                await lobby.Join();
                return;
            }
        }

    }

    public void CopyID()
    {
        TextEditor textEditor = new TextEditor();
        textEditor.text = lobbyId.text;
        textEditor.SelectAll();
        textEditor.Copy();
    }

    public void LeaveLobby()
    {
        LobbySaver.instance.currentLobby?.Leave();
        LobbySaver.instance.currentLobby = null;
        NetworkManager.Singleton.Shutdown();
        CheckUI();
    }
    private void CheckUI()
    {
        if (LobbySaver.instance.currentLobby != null)
        {
            startMenu.SetActive(false);
            mainMenu.SetActive(false);
            inLobbyMenu.SetActive(true);

            lobbyId.text = LobbySaver.instance.currentLobby?.Id.ToString();
            IEnumerable<Friend> friends = LobbySaver.instance.currentLobby?.Members;
            foreach (Friend friend in friends)
            {
                peopleInLobby.text += friend.Name + "\n";
            }
        }
        else
        {
            startMenu.SetActive(true);
            mainMenu.SetActive(false);
            inLobbyMenu.SetActive(false);
        }
    }

    public void StartGameServer()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            loadingScreen.SetActive(true);

            NetworkManager.Singleton.SceneManager.LoadScene("MainGame", LoadSceneMode.Single);

            
        }
    }

    private void OnDisable()
    {
        SteamMatchmaking.OnLobbyCreated -= LobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= LobbyEntered;
        SteamFriends.OnGameLobbyJoinRequested -= GameLobbyJoinRequested;
    }

    //https://www.youtube.com/watch?v=wvXDCPLO7T0
    public void LoadScene(int sceneId)
    {
        StartCoroutine(LoadSceneAsync(sceneId));

    }

    IEnumerator LoadSceneAsync(int sceneId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);


        loadingScreen.SetActive(true);

        while(!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);

            loadingBarFill.fillAmount = progressValue;

            yield return null;
        }
    }
}

