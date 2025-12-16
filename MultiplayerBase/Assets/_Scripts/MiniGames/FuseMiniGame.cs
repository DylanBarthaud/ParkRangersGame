using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class FuseMiniGame : MiniGameBase
{
    [SerializeField] int roumds = 3;
    private int round = 0;

    [SerializeField] private int chances = 1;
    private int timesLost = 0; 

    [SerializeField] GameObject[] levers;
    private int pulledLeversAmount = 0;
    private List<int> pulledLeversList = new List<int>();

    private List<int> pattern = new List<int>();

    [SerializeField] AudioHandler audioHandler;

    private void OnEnable()
    {
        ResetGame();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            LockMouse();
            EndGame(false);
        }
    }

    private void ResetGame()
    {
        foreach (int i in pattern)
        {
            levers[i].transform.GetChild(0).GetComponent<Image>().color = Color.red;
        }

        round = 0;
        timesLost = 0; 
        pattern.Clear();
        LockMouse();
        ShowPattern();
    }

    private void ShowPattern()
    {
        round++;
        int newEntry = Random.Range(0, 9);  
        pattern.Add(newEntry);

        StartCoroutine(ShowColour());
    }

    private IEnumerator ShowColour()
    {
        yield return new WaitForSeconds(0.5f); 
        foreach (int i in pattern)
        {
            levers[i].transform.GetChild(0).GetComponent<Image>().color = Color.green;
            audioHandler.PlaySound("Beep", false, 1, 5, 100, 1.4f - (((float)i + 1) / 10)); 
            yield return new WaitForSeconds(0.5f);
            levers[i].transform.GetChild(0).GetComponent<Image>().color = Color.red;
            yield return new WaitForSeconds(0.1f);
        }

        UnlockMouse();
    }

    public void PullLever(int i)
    {
        audioHandler.PlaySound("Switch");

        pulledLeversAmount++;
        pulledLeversList.Add(i);
        if(pulledLeversAmount == round)
        {
            pulledLeversAmount = 0; 
            LockMouse();
            CheckPattern();
        }
    }
    private void CheckPattern()
    {
        if(pulledLeversList.SequenceEqual(pattern))
        {
            if (round == roumds) EndGame(true);
            else
            {
                ShowPattern();
            }
        }

        else
        {
            timesLost++;
            if (timesLost == chances) EndGame(false);
            else ResetGame();
        }

        pulledLeversList.Clear();
    }

    private void EndGame(bool sucess)
    {
        EventManager.instance.OnPuzzleComplete(sucess);
        GameManager.instance.DisableMiniGame(MiniGameTypes.FuseBox);
    }

    private void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    private void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
}

