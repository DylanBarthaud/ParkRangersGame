using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    private int playersIn = 0;
    private int amountOfPlayers = 0;

    void Awake()
    {
        amountOfPlayers = GameManager.instance.numberOfPlayers;
    }

    void OnTriggerEnter()
    {
        playersIn++;
        Debug.Log(playersIn);
    }

    void OnTriggerExit()
    {
        playersIn--;
        Debug.Log(playersIn);
    }

    void Update()
    {
        Debug.Log(amountOfPlayers);

        GameManager _gameManager = GameManager.instance; 
        _gameManager.numberOfPlayers = amountOfPlayers;

        if (playersIn == amountOfPlayers*2 && playersIn >= 1)
        {
            Debug.Log("YOU WIN!");
            _gameManager.EndGame();
        }
        
    }
}
