using UnityEngine;
using Discord;
using System;

public class DiscordController : MonoBehaviour
{
    #if UNITY_STANDALONE_WIN
    public string details;
    public bool firstActivity = false;

    // sets status on start
    void Start()
    {

        // checks if discord is running
        if (DiscordSettings.set == false)
        {
            firstActivity = true;

            for (var i = 0; i < System.Diagnostics.Process.GetProcesses().Length; i++)
            {
                if (System.Diagnostics.Process.GetProcesses()[i].ToString() == "System.Diagnostics.Process (Discord)")
                {
                    DiscordSettings.discordRunning = true;
                    break;
                }
            }

            DiscordSettings.set = true;
        }

        // adds new activity if needed
        if (DiscordSettings.discordRunning && DiscordSettings.activity.Details != details)
        {
            // removes old activity if neccessary
            if (firstActivity == false) DiscordSettings.discord.Dispose();

            DiscordSettings.discord = new Discord.Discord(831242163907592243, (ulong)CreateFlags.Default);
            ActivityManager activityManager = DiscordSettings.discord.GetActivityManager();

            DiscordSettings.activity = new Activity
            {
                Details = details,

                Timestamps =
                {
                    Start = ToUnixTime()
                },

                Assets =
                {
                    LargeImage = "icon"
                }
            };

            activityManager.UpdateActivity(DiscordSettings.activity, (response) =>
            {
                if (response == Result.Ok)
                {
                    Debug.Log("Status set.");
                }
                else
                {
                    Debug.LogError("Status failed to set.");
                }
            });
        }
    }

    // runs callbacks forever
    void Update()
    {
        if (DiscordSettings.discordRunning)
        {
            DiscordSettings.discord.RunCallbacks();
        }
    }

    // deletes status when closed
    void OnApplicationQuit()
    {
        if (DiscordSettings.discordRunning)
        {
            DiscordSettings.discord.Dispose();
        }
    }

    long ToUnixTime()
    {
        DateTime now = DateTime.Now;
        long unixTime = ((DateTimeOffset)now).ToUnixTimeSeconds();

        return unixTime;
    }
    #endif
}