using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    private int playersIn;
    private int amountOfPlayers;
    private GameManager _gameManager;

    void Awake()
    {
        _gameManager = gameObject.GetComponent<GameManager>();
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
        _gameManager.numberOfPlayers = amountOfPlayers;
        if (amountOfPlayers == playersIn)
        {
            Debug.Log("YOU WIN!");
            _gameManager.EndGame();
        }
    }
}
