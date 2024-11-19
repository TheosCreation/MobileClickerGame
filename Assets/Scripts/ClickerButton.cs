using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClickerButton : MonoBehaviour
{
    // Identifier for this button instance
    public string buttonID;

    // Flag to determine if the button is locked
    public bool isLocked = false;

    // UI references for interaction and display
    [SerializeField] private Button clickerButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button unlockButton;
    [SerializeField] private TMP_Text unlockCostText;
    [SerializeField] private TMP_Text levelProgressText;
    [SerializeField] private TMP_Text upgradeCostText;

    // Progress bar for collection progress visualization
    [SerializeField] private ProgressBar collectionProgressBar;

    // Variables controlling collection mechanics
    [SerializeField] private float collectSpeed = 1.0f; // Base speed for automatic collection
    [SerializeField] private float progressMultiplier = 1.0f; // A multiplier to adjust difficulty

    // UI and collection data
    [SerializeField] private TMP_Text collectionAmountText;
    [SerializeField] private GameObject levelLockedOverlay;
    [SerializeField] private TMP_Text levelLockedText;

    // Currency and level data
    [SerializeField] private double collectionAmount = 1.0f;
    [SerializeField] private double currentUpgradeCost = 200.0f;
    [SerializeField] private double unlockCost = 10000;
    public int level = 1;
    public int levelRequiredToUnlock = 0;

    // Tracks collection progress
    float collectionProgress = 0.0f;

    /// <summary>
    /// Subscribe to button click events when the object is enabled.
    /// </summary>
    private void OnEnable()
    {
        clickerButton.onClick.AddListener(ManualCollect);
        upgradeButton.onClick.AddListener(Upgrade);
        unlockButton.onClick.AddListener(Unlock);
    }

    /// <summary>
    /// Unsubscribe from button click events when the object is disabled.
    /// </summary>
    private void OnDisable()
    {
        clickerButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.RemoveAllListeners();
        unlockButton.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// Load saved data when the object is initialized.
    /// </summary>
    private void Awake()
    {
        LoadData();
    }

    /// <summary>
    /// Set up the UI and initial states on start.
    /// </summary>
    private void Start()
    {
        UpdateCollectionAmountText();
        UpdateLevelText();
        UpdateUpgradeCostText();
        UpdateUnlockStatus();
        UpdateLevelRequirementStatus();
    }

    /// <summary>
    /// Handles collection progress updates and checks for completion.
    /// </summary>
    private void Update()
    {
        if (isLocked) return;

        // Increment the collection progress over time
        collectionProgress += (Time.deltaTime * collectSpeed) / progressMultiplier;

        CheckProgress();
    }

    /// <summary>
    /// Trigger manual collection when the button is clicked.
    /// </summary>
    private void ManualCollect()
    {
        if (isLocked) return;

        // Add to collection progress manually
        collectionProgress += GameManager.Instance.pointerValue / progressMultiplier;

        CheckProgress();
    }

    /// <summary>
    /// Upgrade the level, increasing collection efficiency.
    /// </summary>
    private void Upgrade()
    {
        if (GameManager.Instance.CurrentMoney > currentUpgradeCost)
        {
            GameManager.Instance.CurrentMoney -= currentUpgradeCost;
            level++;
            collectionAmount *= 1.5f; // Increase collection amount
            currentUpgradeCost *= 2.0f; // Double upgrade cost

            UpdateCollectionAmountText();
            UpdateLevelText();
            UpdateUpgradeCostText();
        }
    }

    /// <summary>
    /// Unlock the button functionality.
    /// </summary>
    private void Unlock()
    {
        if (GameManager.Instance.CurrentMoney > unlockCost)
        {
            GameManager.Instance.CurrentMoney -= unlockCost;
            isLocked = false;
            UpdateUnlockStatus();
        }
    }

    /// <summary>
    /// Check if collection progress has reached the target and trigger collection.
    /// </summary>
    void CheckProgress()
    {
        if (collectionProgress >= 1.0f)
        {
            Collection();
            collectionProgress %= 1.0f; // Reset progress while keeping remainder
        }

        // Update the progress bar
        collectionProgressBar.value = collectionProgress;
    }

    /// <summary>
    /// Add the collected amount to the player's currency.
    /// </summary>
    private void Collection()
    {
        GameManager.Instance.CurrentMoney += collectionAmount;
    }

    /// <summary>
    /// Update the collection amount text.
    /// </summary>
    private void UpdateCollectionAmountText()
    {
        collectionAmountText.text = "earn $" + NumberFormatter.FormatLargeNumber(collectionAmount);
    }

    /// <summary>
    /// Update the level progress text.
    /// </summary>
    private void UpdateLevelText()
    {
        levelProgressText.text = level.ToString();
    }

    /// <summary>
    /// Update the upgrade cost text.
    /// </summary>
    private void UpdateUpgradeCostText()
    {
        upgradeCostText.text = "cost $" + NumberFormatter.FormatLargeNumber(currentUpgradeCost);
    }

    /// <summary>
    /// Update the unlock status and toggle related UI elements.
    /// </summary>
    private void UpdateUnlockStatus()
    {
        unlockCostText.text = "Unlock cost $" + NumberFormatter.FormatLargeNumber(unlockCost);
        unlockButton.gameObject.SetActive(isLocked);
    }

    /// <summary>
    /// Update the overlay showing level requirements for unlocking.
    /// </summary>
    public void UpdateLevelRequirementStatus()
    {
        if (GameManager.Instance.currentLevel < levelRequiredToUnlock)
        {
            levelLockedOverlay.SetActive(true);
            levelLockedText.text = "Level required " + levelRequiredToUnlock.ToString();
        }
        else
        {
            levelLockedOverlay.SetActive(false);
        }
    }

    /// <summary>
    /// Save generator data for persistence.
    /// </summary>
    public void SaveData()
    {
        var data = new GeneratorSaveData
        {
            CurrentLevel = level,
            CollectAmount = (float)collectionAmount,
            UpgradeCost = (float)currentUpgradeCost,
            IsLocked = isLocked
        };
        GameManager.Instance.SaveGeneratorData(buttonID, data);
    }

    /// <summary>
    /// Load saved data if available.
    /// </summary>
    private void LoadData()
    {
        if (GameManager.Instance.TryGetGeneratorData(buttonID, out GeneratorSaveData data))
        {
            level = data.CurrentLevel;
            collectionAmount = data.CollectAmount;
            currentUpgradeCost = data.UpgradeCost;
            isLocked = data.IsLocked;
        }
    }
}