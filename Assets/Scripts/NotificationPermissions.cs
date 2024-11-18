using UnityEngine;
using System.Collections;

public class NotificationPermissions : MonoBehaviour
{
    private const string NotificationPermission = "android.permission.POST_NOTIFICATIONS";

    public void RequestNotificationPermission()
    {
        // Check if not running in the Unity editor and if the device is running Android 13 or higher
        if (!Application.isEditor && IsAndroid13OrHigher())
        {
            // Request the notification permission
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    activity.Call("requestPermissions", new string[] { NotificationPermission }, 0);
                }
            }
        }
    }

    private bool IsAndroid13OrHigher()
    {
        return (SystemInfo.operatingSystem.Contains("Android OS 13") ||
                SystemInfo.operatingSystem.Contains("Android OS 14") ||
                SystemInfo.operatingSystem.Contains("Android OS 15"));
    }

    public bool IsPermissionGranted()
    {
        // Check if not running in the Unity editor and if on Android
        if (!Application.isEditor && Application.platform == RuntimePlatform.Android)
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    int permissionStatus = activity.Call<int>("checkSelfPermission", NotificationPermission);
                    return permissionStatus == 0;  // Permission granted
                }
            }
        }

        return false; // In the editor or not on Android, return false
    }
}