using Steamworks.Data;
using UnityEngine;

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
}
