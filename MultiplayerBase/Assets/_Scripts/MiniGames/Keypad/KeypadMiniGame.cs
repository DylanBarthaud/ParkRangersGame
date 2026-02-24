using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utlility; 

public class KeypadMiniGame : MiniGameBase
{
    private static int CODE_LENGTH = 4;
    private static int DIGIT_AMOUNT = 10;
    [SerializeField] private Image[] symbolCode;
    [SerializeField] private List<Sprite> symbols;
    public List<Sprite> Symbols => symbols;
    private int[] symbolSeed = new int[DIGIT_AMOUNT];
    public int[] SymbolSeed => symbolSeed;


    private int[] code = new int[CODE_LENGTH];
    public int[] Code => code;
    private List<int> currentCode = new List<int>();

    public Sprite GetSymbol(int index) => symbols[index];
    public void SetSymbolSeed(int[] symbolSeed) => this.symbolSeed = symbolSeed;
    public void SetCode(int[] code) => this.code = code;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void ButtonPress(int index)
    {
        if(currentCode.Count == CODE_LENGTH) return;

        Debug.Log(index); 
        currentCode.Add(index);
    }

    public void EnterButton()
    {
        //if (currentCode.Count < CODE_LENGTH) return;

        bool success = code.SequenceEqual(currentCode);
        StartCoroutine(EndGame(success));
        ClearCurrentCode();
    }

    public void ClearCurrentCode() => currentCode.Clear();
    
    private IEnumerator EndGame(bool success)
    {
        yield return new WaitForSeconds(0.2f);

        Cursor.lockState = CursorLockMode.Locked;
        miniGameObj.GetComponent<MiniGame>().OnCompleteServerRpc(success);
        EventManager.instance.OnPuzzleComplete(success);
        GameManager.instance.DisableMiniGame(MiniGameTypes.Keypad);
    }

    public void Reset()
    {
        ClearCurrentCode();

        for (int i =  0; i < symbolSeed.Length; i++) symbolSeed[i] = i;
        ListExtentions.Shuffle(symbolSeed);

        for (int i = 0; i < CODE_LENGTH; i++)
        {
            int roll = Random.Range(0, DIGIT_AMOUNT);

            code[i] = roll;
            Debug.Log(roll);
            symbolCode[i].sprite = GetSymbol(symbolSeed[roll]);
        }
    }
}
