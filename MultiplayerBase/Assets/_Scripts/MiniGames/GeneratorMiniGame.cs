using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class GeneratorMiniGame : MiniGameBase
{
    [SerializeField] int neededFuelAmount;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void FillGen() => StartCoroutine(EndGame(true)); 

    private IEnumerator EndGame(bool success)
    {
        if (success)
        {
            Inventory inv = interactor.GetComponent<Inventory>();
            Item fuel = inv.HeavyItem; 
            inv.RemoveItem(fuel);
            fuel.DespawnItemServerRPC(); 
        }

        yield return new WaitForSeconds(0.2f);

        Cursor.lockState = CursorLockMode.Locked;
        miniGameObj.GetComponent<MiniGame>().OnComplete(success);
        EventManager.instance.OnPuzzleComplete(success);
        GameManager.instance.DisableMiniGame(MiniGameTypes.Generator);
    }
}
