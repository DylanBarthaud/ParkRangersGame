using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EndScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject playerObj;
    private PlayerInfo playerInfo;
    public Image winScreen;
    public Image lossScreen;
    public GameObject endLoc;

    public float waitTimer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerInfo = playerObj.GetComponent<PlayerInfo>();

        OpenWinDisplay();
    }

    private void OpenWinDisplay()
    {
        if (playerInfo.health <= 0)
        {
            winScreen.enabled = false;
            lossScreen.enabled = true;
            StartCoroutine(MovePlayerToStats(waitTimer));
        }
        else
        {
            winScreen.enabled = true;
            lossScreen.enabled = false;
            StartCoroutine(MovePlayerToStats(waitTimer));
        }

    }

    private IEnumerator MovePlayerToStats(float time)
    {
        yield return new WaitForSeconds(time);

        playerObj.transform.position = endLoc.transform.position;
    }
}
