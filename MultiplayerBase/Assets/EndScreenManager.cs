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
    public void OpenDisplay()
    {
        playerInfo = playerObj.GetComponent<PlayerInfoHolder>().GetPlayerInfo();

        OpenWinDisplay();
        Debug.Log("OpenedEndDisplay");
    }

    private void OpenWinDisplay()
    {
        if (playerInfo.health <= 0)
        {
            Debug.Log("OpenedLossScreen");
            winScreen.enabled = false;
            lossScreen.enabled = true;
            StartCoroutine(MovePlayerToStats(waitTimer));
        }
        else
        {
            Debug.Log("OpenedWinScreen");
            winScreen.enabled = true;
            lossScreen.enabled = false;
            StartCoroutine(MovePlayerToStats(waitTimer));
        }

    }

    private IEnumerator MovePlayerToStats(float time)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("Attempted To Move Player");
        playerObj.transform.position = endLoc.transform.position;
    }
}
