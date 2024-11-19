using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    // Singleton instance of UiManager
    public static UiManager Instance { get; private set; }

    // References to UI elements
    [SerializeField] private TMP_Text totalMoneyText;    // Text to display total money
    [SerializeField] private ProgressBar levelProgressBar;  // Progress bar for the level progress
    [SerializeField] private TMP_Text currentLevelText;   // Text to display current level
    [SerializeField] private Button upgradeLevelButton;   // Button to upgrade level
    [SerializeField] private Button shopButton;           // Button to open the shop menu
    [SerializeField] private ShopMenu shopMenu;           // Reference to the shop menu
    [SerializeField] private Button pauseButton;          // Button to pause the game
    [SerializeField] private PauseMenu pauseMenu;         // Reference to the pause menu

    private void Awake()
    {
        // Ensure only one instance of UiManager exists (Singleton pattern)
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // Destroy this instance if another instance already exists
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        // Add listeners to button click events
        upgradeLevelButton.onClick.AddListener(GameManager.Instance.UpgradeLevel);  // Upgrade level on button click
        shopButton.onClick.AddListener(OpenShopMenu);  // Open shop menu on button click
        pauseButton.onClick.AddListener(OpenPauseMenu); // Open pause menu on button click
    }

    private void OnDisable()
    {
        // Remove listeners to prevent memory leaks when the object is disabled
        upgradeLevelButton.onClick.RemoveAllListeners();
        shopButton.onClick.RemoveAllListeners();
        pauseButton.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        // Initialize UI elements with data from the GameManager
        UpdateCurrentMoneyText(GameManager.Instance.CurrentMoney);  // Display current money
        UpdateLevelBar(GameManager.Instance.m_currentLevelProgress / GameManager.Instance.levelUpRequirement);  // Update progress bar
        UpdateLevel(GameManager.Instance.currentLevel);  // Display current level

        // Close both pause and shop menus on start
        ClosePauseMenu();
        CloseShopMenu();
    }

    // Method to update the total money text on the UI
    public void UpdateCurrentMoneyText(double amount)
    {
        totalMoneyText.text = "$" + NumberFormatter.FormatLargeNumber(amount);  // Format and display the money
    }

    // Method to update the level progress bar on the UI
    public void UpdateLevelBar(float levelBarProgress)
    {
        levelProgressBar.value = levelBarProgress;  // Set the progress value
    }

    // Method to update the current level text on the UI
    public void UpdateLevel(int currentLevel)
    {
        currentLevelText.text = "Level " + currentLevel.ToString();  // Display current level
    }

    // Method to open the shop menu
    public void OpenShopMenu()
    {
        shopMenu.gameObject.SetActive(true);  // Activate the shop menu
    }

    // Method to close the shop menu
    public void CloseShopMenu()
    {
        shopMenu.gameObject.SetActive(false);  // Deactivate the shop menu
    }

    // Method to open the pause menu
    public void OpenPauseMenu()
    {
        PauseManager.Instance.Pause();  // Pause the game
        pauseMenu.gameObject.SetActive(true);  // Activate the pause menu
    }

    // Method to close the pause menu
    public void ClosePauseMenu()
    {
        PauseManager.Instance.UnPause();  // Unpause the game
        pauseMenu.gameObject.SetActive(false);  // Deactivate the pause menu
    }
}