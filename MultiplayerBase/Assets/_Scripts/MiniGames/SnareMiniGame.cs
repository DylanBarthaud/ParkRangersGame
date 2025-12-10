using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SnareMiniGame : MonoBehaviour
{
    [SerializeField] Slider setChecks;
    [SerializeField] Slider slider;

    [SerializeField] TextMeshProUGUI winsAndLosesText; 

    [SerializeField] int winsNeeded = 1;
    [SerializeField] int losesNeeded = 1;   
    private int wins = 0;
    private int loses = 0; 

    private bool goingBackwards = false;

    [HideInInspector] public SnareTrap trap;

    private void OnEnable()
    {
        wins = 0; 
        loses = 0;
        winsAndLosesText.text = $"Wins: {wins}/{winsNeeded} Loses: {loses}/{losesNeeded}";
        ResetGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (slider.value > setChecks.value - 3 &&
                slider.value < setChecks.value + 3)
            {
                wins++;
                if(wins >= winsNeeded)
                {
                    EventManager.instance.OnPuzzleComplete();
                    GameManager.instance.uiManager.DisableSnareGameUi();
                    trap.canInteract = false;
                }
            }
            else
            {
                loses++;

                if(loses >= losesNeeded)
                {
                    FailGame();
                }
            }

            winsAndLosesText.text = $"Wins: {wins}/{winsNeeded} Loses: {loses}/{losesNeeded}";

            ResetGame();
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            FailGame();
        }
    }

    private void FixedUpdate()
    {
        if (goingBackwards) slider.value--;
        else slider.value++;

        if (slider.value == 50) goingBackwards = true;
        if (slider.value == 0) goingBackwards = false;
    }

    private void ResetGame()
    {
        setChecks.value = Random.Range(0, 51);
        slider.value = 0;
    }

    private void FailGame()
    {
        EventManager.instance.OnPuzzleComplete(false);
        GameManager.instance.uiManager.DisableSnareGameUi();
        trap.canInteract = false;
    }
}
