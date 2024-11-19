using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    // Singleton instance for easy access to the MainMenuManager from other scripts.
    public static MainMenuManager Instance { get; private set; }

    // Serialized fields for the main page and loading page UI components.
    [SerializeField] private MainMenuMainPage mainPage;
    [SerializeField] private LoadingPage loadingPage;

    // Ensures that only one instance of MainMenuManager exists in the scene.
    private void Awake()
    {
        if (Instance == null)
        {
            // If there's no instance, set this as the instance.
            Instance = this;
        }
        else
        {
            // If an instance already exists, destroy the new instance.
            Destroy(gameObject);
            return;
        }
    }

    // Initializes the main menu on start.
    private void Start()
    {
        // Open the main menu page.
        OpenMainPage();

        // Check if the player has logged in for the first time and handle achievement.
        if (!GameManager.Instance.GameState.hasLoggedIn)
        {
            UnlockLoginForFirstTimeAchivement();
            // Mark the player as logged in to prevent this from triggering again.
            GameManager.Instance.GameState.hasLoggedIn = true;
        }
    }

    // Unlocks the 'first-time login' achievement if the player is authenticated.
    private void UnlockLoginForFirstTimeAchivement()
    {
        if (Social.localUser.authenticated)
        {
            // Report the achievement progress (100% completion).
            Social.ReportProgress(
                GPGSIds.achievement_first_time_login,
                100.0,
                (bool _success) =>
                {
                    // Show the achievements UI when the achievement is successfully unlocked.
                    Social.ShowAchievementsUI();
                });
        }
    }

    // Switches to the main menu page, hiding the loading page.
    public void OpenMainPage()
    {
        mainPage.gameObject.SetActive(true);
        loadingPage.gameObject.SetActive(false);
    }

    // Switches to the loading page, hiding the main menu page.
    public void OpenLoadingPage()
    {
        loadingPage.gameObject.SetActive(true);
        mainPage.gameObject.SetActive(false);
    }
}