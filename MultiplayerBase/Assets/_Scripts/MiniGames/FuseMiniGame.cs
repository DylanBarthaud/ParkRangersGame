using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utlility; 

public class FuseMiniGame : MiniGameBase
{
    private uint current = 0b111111; 
    private uint target = 0b111111;

    [SerializeField] private int numberOfLevers = 6;
    [SerializeField] GameObject[] levers; 
    [SerializeField, Range(1, 6)] private int minNumberOfLeversPulled = 1; 
    [SerializeField, Range(1, 6)] private int maxNumberOfLeversPulled = 6;

    List<uint> leverBinaryValues = new List<uint>();
    private Dictionary<int, bool> leverIndexIsActiveDictionary = new Dictionary<int, bool>();

    private void OnEnable()
    {
        ResetGame();

        Cursor.lockState = CursorLockMode.None;

        int numberOfLeversNeeded = UnityEngine.Random.Range(minNumberOfLeversPulled, maxNumberOfLeversPulled);
        int redHerringLeverAmount = numberOfLevers - numberOfLeversNeeded; 

        for(int i = 0;  i < numberOfLevers; i++)
        {
            uint binaryValue = (uint)UnityEngine.Random.Range(0b000000, 0b111111);
            leverBinaryValues.Add(binaryValue);
            leverIndexIsActiveDictionary.Add(i, false);

            if (i < numberOfLeversNeeded)
            {
                Debug.Log(i); 
                target ^= binaryValue;
            }
        }

        Debug.Log(Convert.ToString(target, 2).PadLeft(6, '0'));

        ListExtentions.Shuffle(leverBinaryValues); 
    }

    public void FlipLever(int index)
    {
        leverIndexIsActiveDictionary[index] = !leverIndexIsActiveDictionary[index];
        current ^= leverBinaryValues[index];

        Image leverImage = levers[index].GetComponent<Image>();
        if (leverIndexIsActiveDictionary[index]) leverImage.color = Color.green;
        else leverImage.color = Color.red;  
        Debug.Log(Convert.ToString(current, 2).PadLeft(6, '0'));
        if(current == target) EndGame(true);
    }

    private void ResetGame()
    {
        target = 0b111111;
        current = 0b111111;

        foreach (var lever in levers)
            lever.GetComponent<Image>().color = Color.red;
    }


    private void EndGame(bool success)
    {
        Cursor.lockState = CursorLockMode.Locked;
        miniGameObj.GetComponent<MiniGame>().OnCompleteServerRpc(success);
        EventManager.instance.OnPuzzleComplete(success);
        GameManager.instance.DisableMiniGame(MiniGameTypes.FuseBox);
    }
}
