using System.Collections;
using Unity.Notifications.Android;
using UnityEngine.Android;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

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

        //// Check if the app was opened from a notification tap
        //var notificationIntentData = AndroidNotificationCenter.GetLastNotificationIntent();
        //if (notificationIntentData != null)
        //{
        //    OnNotificationTapped(notificationIntentData);
        //}
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

        ScheduleDailyNotification("Welcome Back!", "Ready to collect more money? Come back and play again!", "OpenGame");

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

        // Set the fire time to the current date at a specific time (e.g., 9:00 AM)
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

        // Start a coroutine to reschedule this notification daily
        StartCoroutine(RescheduleDailyNotification(notification, fireTime));
    }

    private IEnumerator RescheduleDailyNotification(AndroidNotification notification, System.DateTime fireTime)
    {
        // Wait until the next day
        yield return new WaitForSeconds((float)(fireTime - System.DateTime.Now).TotalSeconds);

        // Reschedule the notification for the next day
        notification.FireTime = fireTime.AddDays(1);
        AndroidNotificationCenter.SendNotification(notification, "default_channel");

        // Call this method again to keep rescheduling daily
        StartCoroutine(RescheduleDailyNotification(notification, fireTime.AddDays(1)));
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