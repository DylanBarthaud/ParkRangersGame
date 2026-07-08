using Discord;
using Newtonsoft.Json.Converters;
using System;
using UnityEngine;



public class DiscordManager : MonoBehaviour
{
    private static DiscordManager instance;

    Discord.Discord discord;
    long launchtime = 0;

    public string state = string.Empty;
    public string details = string.Empty;

    private void Awake()
    {
        if(instance != null) Destroy(gameObject);
        else instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        var epochStart = new System.DateTime(1970, 1, 1, 1, 0, 0, System.DateTimeKind.Utc);
        var timestamp = (System.DateTime.Now - epochStart).TotalSeconds;

        launchtime = (long)timestamp;

        discord = new Discord.Discord(1501573658127368262, (ulong)Discord.CreateFlags.NoRequireDiscord);
        ChangeActivity();

        DontDestroyOnLoad(this.gameObject);
    }


    private void OnDisable()
    {
        discord.Dispose();
    }

    public void ChangeActivity()
    {
        var activityManager = discord.GetActivityManager();
        var activity = new Discord.Activity
        {
            State = state,
            Details = details,
            Assets =
            {
                LargeImage = "",
                LargeText = "",
                SmallImage = "",
                SmallText = "",
            },
            Timestamps =
            {
                Start = launchtime
            },
            Party =
            {
                Id = "bfroom1",
                Size =
                {
                    CurrentSize = 1,
                    MaxSize = 4,
                }
            }
        };

        activityManager.UpdateActivity(activity, (res) =>
        {
            Debug.Log("Activity updated!");
        });

    }

    
    // Update is called once per frame
    void Update()
    {
        discord.RunCallbacks();
    }
}
