using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : UiPage
{
    public void Back()
    {
        UiManager.Instance.ClosePauseMenu();
    }

    public void OpenMainMenu()
    {
        GameManager.Instance.LoadMainMenu();
    }
}
