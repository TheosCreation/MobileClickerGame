using UnityEngine;
using System.Collections;

public class NotificationPermissions : MonoBehaviour
{
    // Constant for the Android notification permission.
    private const string NotificationPermission = "android.permission.POST_NOTIFICATIONS";

    // Requests the notification permission for Android 13 or higher.
    public void RequestNotificationPermission()
    {
        // Check if not running in the Unity editor and if the device is running Android 13 or higher
        if (!Application.isEditor && IsAndroid13OrHigher())
        {
            // Request the notification permission by calling the Android Java API
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                // Get the current activity context to request permission
                using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    // Call the Android method to request permission
                    activity.Call("requestPermissions", new string[] { NotificationPermission }, 0);
                }
            }
        }
    }

    // Checks if the device is running Android 13 or higher
    private bool IsAndroid13OrHigher()
    {
        // Check if the OS version is Android 13, 14, or 15
        return (SystemInfo.operatingSystem.Contains("Android OS 13") ||
                SystemInfo.operatingSystem.Contains("Android OS 14") ||
                SystemInfo.operatingSystem.Contains("Android OS 15"));
    }

    // Checks if the notification permission has been granted
    public bool IsPermissionGranted()
    {
        // Check if not running in the Unity editor and if the platform is Android
        if (!Application.isEditor && Application.platform == RuntimePlatform.Android)
        {
            // Use AndroidJavaClass to interact with Android's API to check the permission status
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    // Check the current permission status
                    int permissionStatus = activity.Call<int>("checkSelfPermission", NotificationPermission);
                    return permissionStatus == 0;  // Permission granted if status is 0
                }
            }
        }

        // Return false if running in the Unity editor or not on Android
        return false;
    }
}