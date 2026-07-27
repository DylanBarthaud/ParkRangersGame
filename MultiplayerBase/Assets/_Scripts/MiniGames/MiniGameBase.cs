using UnityEngine;

public class MiniGameBase : MonoBehaviour 
{
    protected GameObject miniGameObj; 
    protected Interactor interactor;
    public void SetMiniGameObj(GameObject obj) => miniGameObj = obj;
    public void SetInteractor(Interactor interactor) => this.interactor = interactor;
    public virtual void QuitGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        miniGameObj.GetComponent<MiniGame>().OnComplete(false);
        EventManager.instance.OnPuzzleComplete(false);
        GameManager.instance.DisableMiniGame();
    }

    private void Update()
    {
        CheckQuit();
    }

    protected void CheckQuit() { if (Input.GetKeyUp(KeyCode.O)) QuitGame(); }
}
