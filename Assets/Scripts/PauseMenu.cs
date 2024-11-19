

// The PauseMenu class inherits from UiPage, indicating it's a UI page for the pause menu.
public class PauseMenu : UiPage
{
    // Called when the "Back" button is pressed.
    public void Back()
    {
        // Closes the pause menu by calling the UiManager's ClosePauseMenu method.
        UiManager.Instance.ClosePauseMenu();
    }

    // Called when the "Open Main Menu" button is pressed.
    public void OpenMainMenu()
    {
        // Loads the main menu scene through the GameManager's LoadMainMenu method.
        GameManager.Instance.LoadMainMenu();
    }
}