using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorMiniGame : MiniGameBase
{
    [SerializeField] Slider slider; 
    [SerializeField] int maxFuelAmount;
    int currentFuelAmount;

    private void OnEnable()
    {
        slider.maxValue = maxFuelAmount;
        Cursor.lockState = CursorLockMode.None;
    }

    public void FillGen()
    {
        currentFuelAmount++;
        if(currentFuelAmount >= maxFuelAmount)
        {
            EndGame(true); 
        }
        else slider.value = currentFuelAmount;
    }

    private IEnumerator EndGame(bool success)
    {
        yield return new WaitForSeconds(0.2f);

        Cursor.lockState = CursorLockMode.Locked;
        miniGameObj.GetComponent<MiniGame>().OnCompleteServerRpc(success);
        EventManager.instance.OnPuzzleComplete(success);
        GameManager.instance.DisableMiniGame(MiniGameTypes.Generator);
    }
}
