using Steamworks.Data;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.SceneManagement;


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

        NetworkManager.Singleton.OnClientStopped += QuitToMainMenu;
    }

    public void QuitToMainMenu(bool isHost)
    {
        LeaveLobby();
        SceneManager.LoadScene("Menu"); 
    }

    private void OnApplicationQuit() => LeaveLobby();

    public void LeaveLobby()
    {
        currentLobby?.Leave();
        currentLobby = null;
        NetworkManager.Singleton.Shutdown();
    }
}
