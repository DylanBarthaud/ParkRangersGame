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
    }

    void OnTriggerExit()
    {
        playersIn--;
    }

    void Update()
    {
        GameManager _gameManager = GameManager.instance; 
        _gameManager.numberOfPlayers = amountOfPlayers;

        if (playersIn == amountOfPlayers*2 && playersIn >= 1)
        {
            _gameManager.EndGame();
        }
    }
}
