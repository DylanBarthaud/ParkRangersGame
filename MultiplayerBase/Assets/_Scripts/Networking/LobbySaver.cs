using Steamworks.Data;
using TMPro;
using UnityEngine;
using Unity.Netcode;


public class LobbySaver : MonoBehaviour
{
    public Lobby? currentLobby;

    public static LobbySaver instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        else Destroy(gameObject);
    }

    public void QuitToMainMenu()
    {
        if (GameManager.instance != null)
            GameManager.instance.HandlePlayerDisconnectServerRPC(NetworkManager.Singleton.LocalClientId);
        LeaveLobby();
    }

    private void OnApplicationQuit()
    {
        if(GameManager.instance != null) 
            GameManager.instance.HandlePlayerDisconnectServerRPC(NetworkManager.Singleton.LocalClientId);
        LeaveLobby();
    }

    public void LeaveLobby()
    {
        currentLobby?.Leave();
        currentLobby = null;
        NetworkManager.Singleton.Shutdown();
    }
}
