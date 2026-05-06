using UnityEngine;
using System.Collections;

public class UpdateDiscord : MonoBehaviour
{

    public GameObject discordManagerObject;
    DiscordManager discordManager;

    public bool activeOnInit = false;
    public float loadTime = 5.0f;

    //Discord Data
    public string newState;
    public string newDetails;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        //discordManagerObject = GameObject.FindGameObjectWithTag("DiscordManager");
        //discordManager = discordManagerObject.GetComponent<DiscordManager>();
        if (activeOnInit == true)
        {
            StartCoroutine(GetManager());
            UpdateDiscordData();
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            UpdateDiscordActivity();
        }
    }

    public void UpdateDiscordActivity()
    {
        StartCoroutine(GetManager());
        UpdateDiscordData();
    }

    IEnumerator GetManager()
    {
        yield return new WaitForSeconds(loadTime);
        discordManagerObject = GameObject.FindGameObjectWithTag("DiscordManager");
        discordManager = discordManagerObject.GetComponent<DiscordManager>();

        UpdateDiscordData();
    }


    void UpdateDiscordData()
    {

        discordManager.state = newState;
        discordManager.details = newDetails;

        discordManager.ChangeActivity();
    }

}
