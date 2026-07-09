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
        LeaveLobby();
    }

    private void OnApplicationQuit()
    {
        LeaveLobby();
    }

    public void LeaveLobby()
    {
        currentLobby?.Leave();
        currentLobby = null;
        NetworkManager.Singleton.Shutdown();
    }
}
