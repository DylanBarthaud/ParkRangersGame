using UnityEngine;
using UnityEngine.UI;

public class SpringTrapMiniGame : MiniGameBase
{
    [SerializeField] private Slider skillCheckSlider; 
    [SerializeField] private Slider skillWindowSlider;

    [SerializeField] private float skillSliderMaxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float breakSpeed; 
    private float skillSliderSpeed = 0; 

    private bool attempted = false;

    private void OnEnable()
    {
        ResetGame();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            EndGame(false);
        }

        if (Input.GetKey(KeyCode.Space) && !attempted)
        {
            skillSliderSpeed += acceleration * Time.deltaTime;
            if (skillSliderSpeed >= skillSliderMaxSpeed) skillSliderSpeed = skillSliderMaxSpeed; 
        }
        else if (skillSliderSpeed > 0)
        {
            attempted = true;
            skillSliderSpeed -= breakSpeed * Time.deltaTime;
            if (skillCheckSlider.value >= skillCheckSlider.maxValue) skillSliderSpeed = 0; 
            if (skillSliderSpeed < 0) skillSliderSpeed = 0;
        }

        else if (attempted)
        {
            if(skillCheckSlider.value >= skillWindowSlider.value - 3 &&
               skillCheckSlider.value <= skillWindowSlider.value + 3)
            {
                EndGame(true);
            }

            else EndGame(false);
        }
    }

    private void FixedUpdate()
    {
        skillCheckSlider.value += skillSliderSpeed; 
    }

    private void ResetGame()
    {
        skillCheckSlider.value = 0;
        skillWindowSlider.value = Random.Range(35, 46);

        attempted = false; 
        skillSliderSpeed = 0;
    }

    private void EndGame(bool success)
    {
        EventManager.instance.OnPuzzleComplete(success);
        GameManager.instance.DisableMiniGame(MiniGameTypes.SpringTrap);
    }
}
