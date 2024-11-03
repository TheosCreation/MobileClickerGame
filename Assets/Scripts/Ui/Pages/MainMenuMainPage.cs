using UnityEngine;

public class MainMenuMainPage : UiPage
{
    public void StartGame() { GameManager.Instance.LoadGame(); }
    public void OpenSettings() { Debug.Log("Open Settings"); }
    public void QuitGame() { GameManager.Instance.Quit(); }
}
