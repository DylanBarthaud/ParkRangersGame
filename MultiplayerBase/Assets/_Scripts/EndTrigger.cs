using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    private int playersIn = 0;
    private int amountOfPlayers = 0;
    private GameManager _gameManager;

    void Awake()
    {
        _gameManager = gameObject.GetComponent<GameManager>();
        amountOfPlayers = _gameManager.numberOfPlayers;
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

        _gameManager = gameObject.GetComponent<GameManager>();
        _gameManager.numberOfPlayers = amountOfPlayers;

        if (playersIn == amountOfPlayers*2 && playersIn >= 1)
        {
            Debug.Log("YOU WIN!");
            _gameManager.EndGame();
        }
        
    }
}
