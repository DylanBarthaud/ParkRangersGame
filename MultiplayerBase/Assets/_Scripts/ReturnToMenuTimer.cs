using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenuTimer : MonoBehaviour
{
    [SerializeField] float time = 3;

    private void Awake() => StartCoroutine(ReturnToLobby());

    private IEnumerator ReturnToLobby()
    {
        yield return new WaitForSeconds(time);
        NetworkManager.Singleton.SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
