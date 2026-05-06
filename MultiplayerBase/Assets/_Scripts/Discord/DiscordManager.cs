using Discord;
using Newtonsoft.Json.Converters;
using System;
using UnityEngine;

public class DiscordManager : MonoBehaviour
{

    Discord.Discord discord;
    long launchtime = 0;
    //DateTime currentTime = DateTime.UtcNow;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        var epochStart = new System.DateTime(1970, 1, 1, 1, 0, 0, System.DateTimeKind.Utc);
        var timestamp = (System.DateTime.Now - epochStart).TotalSeconds;

        launchtime = (long)timestamp;

        discord = new Discord.Discord(1501573658127368262, (ulong)Discord.CreateFlags.NoRequireDiscord);
        ChangeActivity();

        
        //launchtime = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();
        //System.DateTime.Now.ToUniversalTime();
        //DateTimeOffset.UtcNow.ToUnixTimeSeconds();

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
            State = "Playing",
            Details = "and Crying!",
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
