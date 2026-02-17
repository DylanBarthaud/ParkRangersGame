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
    private static int DIGIT_AMOUNT = 9;
    [SerializeField] private Image[] symbolCode;
    [SerializeField] private List<Sprite> symbols; 
    private Dictionary<int, Sprite> numberToSymbolKey = new Dictionary<int, Sprite>();
    private int[] code = new int[CODE_LENGTH];

    private List<int> currentCode = new List<int>();

    private void Awake()
    {
        Reset();
    }

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
        if (currentCode.Count < CODE_LENGTH) return;

        bool success = code.SequenceEqual(currentCode);
        Debug.Log(success);
        StartCoroutine(EndGame(success)); 
    }

    public void ClearCurrentCode()
    {
        currentCode.Clear();
    }

    private IEnumerator EndGame(bool success)
    {
        yield return new WaitForSeconds(0.2f);

        Cursor.lockState = CursorLockMode.Locked;
        miniGameObj.GetComponent<MiniGame>().OnCompleteServerRpc(success);
        EventManager.instance.OnPuzzleComplete(success);
        GameManager.instance.DisableMiniGame(MiniGameTypes.Keypad);
    }

    private void Reset()
    {
        ClearCurrentCode();
        ListExtentions.Shuffle(symbols);

        for (int i = 0; i < DIGIT_AMOUNT; i++)
            numberToSymbolKey.Add(i, symbols[i]);

        for (int i = 0; i < CODE_LENGTH; i++)
        {
            int roll = Random.Range(0, DIGIT_AMOUNT);
            Debug.Log(roll);
            code[i] = roll;
            symbolCode[i].sprite = numberToSymbolKey[roll];
        }
    }
}
