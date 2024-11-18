using UnityEngine;

public class MainMenuMainPage : UiPage
{
    public void StartGame() { GameManager.Instance.LoadGameScene(); }
    public void OpenSettings() { Debug.Log("Open Settings"); }
    public void QuitGame() { GameManager.Instance.Quit(); }

    public void TestNotifications() { NotificationManager.Instance.ScheduleNotification("Test Notification", "Click this to open game", "OpenGame"); }
}
