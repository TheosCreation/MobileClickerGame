using System;
using System.Collections;
using Unity.Notifications.Android;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    private const string LastNotificationDateKey = "LastNotificationDate";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator Init()
    {
        AndroidNotificationCenter.Initialize();

        var channel = new AndroidNotificationChannel()
        {
            Id = "default_channel",
            Name = "General Notifications",
            Importance = Importance.High,
            Description = "Notifications for general game events.",
            EnableVibration = true,
            CanShowBadge = true,
            LockScreenVisibility = LockScreenVisibility.Public
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        // Check if we need to schedule the daily notification
        if (IsNewDay())
        {
            ScheduleDailyNotification("Welcome Back!", "Ready to collect more money? Come back and play again!", "OpenGame");
        }

        yield return null;
    }

    public void ScheduleNotification(string title, string text, string intentData)
    {
        Debug.Log("Scheduled a message");

        // Create the notification
        var notification = new AndroidNotification
        {
            Title = title,
            Text = text,
            FireTime = System.DateTime.Now.AddSeconds(0.1f),
            SmallIcon = "icon_app_logo",
            IntentData = intentData
        };

        // Send the notification with the specified channel
        AndroidNotificationCenter.SendNotification(notification, "default_channel");
    }

    public void ScheduleDailyNotification(string title, string text, string intentData)
    {
        Debug.Log("Scheduled a daily message");

        var fireTime = System.DateTime.Today.AddHours(9); // 9:00 AM daily

        // If it's already past 9:00 AM today, schedule for tomorrow
        if (fireTime <= System.DateTime.Now)
        {
            fireTime = fireTime.AddDays(1);
        }

        // Create the notification
        var notification = new AndroidNotification
        {
            Title = title,
            Text = text,
            FireTime = fireTime,
            SmallIcon = "icon_app_logo",
            IntentData = intentData
        };

        // Send the notification with the specified channel
        AndroidNotificationCenter.SendNotification(notification, "default_channel");

        // Update the game state with the current date for whenever we need to schedule another notification
        GameManager.Instance.GameState.LastNotificationDate = System.DateTime.Now.ToString("yyyy-MM-dd");
    }

    // Check if the current date is different from the last scheduled date
    private bool IsNewDay()
    {
        string lastScheduledDateString = GameManager.Instance.GameState.LastNotificationDate;

        if (string.IsNullOrEmpty(lastScheduledDateString))
        {
            // If there's no record of the last scheduled date, it's considered a new day
            return true;
        }

        DateTime lastScheduledDate = DateTime.Parse(lastScheduledDateString);

        // Compare only the date part (ignoring time)
        return lastScheduledDate.Date != DateTime.Now.Date;
    }

    private void OnNotificationTapped(AndroidNotificationIntentData intentData)
    {
        // Check the IntentData for specific actions
        if (intentData.Notification.IntentData == "OpenGame")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("BootStrap");
        }
    }
}