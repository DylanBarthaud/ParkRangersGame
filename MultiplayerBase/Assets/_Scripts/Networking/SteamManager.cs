using UnityEngine;
using Steamworks;
using TMPro;
using Steamworks.Data;
using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Netcode.Transports.Facepunch;
using UnityEngine.SceneManagement;

public class SteamManager : MonoBehaviour
{


    [SerializeField] TMP_InputField lobbyIDInputField;
    [SerializeField] private TextMeshProUGUI lobbyId;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject inLobbyMenu;

    private static int maxMembersInLobby = 4;

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
        }
    }

    private void LobbyEntered(Lobby lobby)
    {
        LobbySaver.instance.currentLobby = lobby;
        lobbyId.text = lobby.Id.ToString();
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
            mainMenu.SetActive(false);
            inLobbyMenu.SetActive(true);
        }
        else
        {
            mainMenu.SetActive(true);
            inLobbyMenu.SetActive(false);
        }
    }

    public void StartGameServer()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("MainGame", LoadSceneMode.Single);
        }
    }

    private void OnDisable()
    {
        SteamMatchmaking.OnLobbyCreated -= LobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= LobbyEntered;
        SteamFriends.OnGameLobbyJoinRequested -= GameLobbyJoinRequested;
    }
}

