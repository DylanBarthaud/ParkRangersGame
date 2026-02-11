using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utlility;

public class FuseMiniGame : MiniGameBase
{
    private uint current = 0b111111;
    private uint target = 0b111111;

    [Header("Game Settings")]
    private int numberOfLevers = 6;
    [SerializeField] GameObject[] levers;
    [SerializeField, Range(1, 6)] private int minNumberOfLeversPulled = 1;
    [SerializeField, Range(1, 6)] private int maxNumberOfLeversPulled = 6;

    [Header("Externals")]
    [SerializeField] GameObject[] displayLights;

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
                current ^= binaryValue;
            }
        }

        Debug.Log(Convert.ToString(target, 2).PadLeft(6, '0'));

        ListExtentions.Shuffle(leverBinaryValues);
        SetCurrentDisplayLights();
    }

    public void FlipLever(int index)
    {
        leverIndexIsActiveDictionary[index] = !leverIndexIsActiveDictionary[index];
        current ^= leverBinaryValues[index];

        Image leverImage = levers[index].GetComponent<Image>();
        if (leverIndexIsActiveDictionary[index]) leverImage.color = Color.green;
        else leverImage.color = Color.red;

        SetCurrentDisplayLights(); 

        Debug.Log(Convert.ToString(current, 2).PadLeft(6, '0'));

        if (current == target) StartCoroutine(EndGame(true));
    }

    private void SetCurrentDisplayLights()
    {
        List<int> lightIndexes = GetLightIndexes(current);
        int i = 0;
        foreach (GameObject light in displayLights)
        {
            foreach (int lightIndex in lightIndexes)
            {
                if (i == lightIndex)
                {
                    light.GetComponent<Image>().color = Color.green;
                    break;
                }

                light.GetComponent<Image>().color = Color.red;
            }

            i++;
        }
    }

    private List<int> GetLightIndexes(uint binaryData)
    {
        List<int> indexes = new List<int>();

        for(int i = 0; i < numberOfLevers; i++)
        {
            bool check = (binaryData & (1 << i)) != 0;
            if(check) indexes.Add(i);
        }

        return indexes;
    }

    private void ResetGame()
    {
        target = 0b111111;
        current = 0b111111;

        leverIndexIsActiveDictionary.Clear();

        foreach (var lever in levers)
            lever.GetComponent<Image>().color = Color.red;
        foreach (var light in displayLights)
            light.GetComponent<Image>().color = Color.red;
    }

    private IEnumerator EndGame(bool success)
    {
        yield return new WaitForSeconds(1); 

        Cursor.lockState = CursorLockMode.Locked;
        miniGameObj.GetComponent<MiniGame>().OnCompleteServerRpc(success);
        EventManager.instance.OnPuzzleComplete(success);
        GameManager.instance.DisableMiniGame(MiniGameTypes.FuseBox);
    }
}
