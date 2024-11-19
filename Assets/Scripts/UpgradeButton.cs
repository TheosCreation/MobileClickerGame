using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Enum to define different types of upgrades
public enum UpgradeType
{
    ManualClick,  // Manual click upgrade type
}

public class UpgradeButton : MonoBehaviour
{
    public string buttonID;  // Unique identifier for this button, used for saving and loading data
    private Button button;  // Reference to the Button component
    [SerializeField] private TMP_Text costText;  // Text element to display the upgrade cost
    [SerializeField] private UpgradeType upgradeType = UpgradeType.ManualClick;  // Type of upgrade
    public double cost = 1000;  // Initial cost of the upgrade
    [SerializeField] private float difficultyScale = 1.2f;  // Scaling factor for the cost with each upgrade
    [SerializeField] private float upgradeAmmount = 0.08f;  // Amount to increase for each upgrade

    private void Awake()
    {
        // Get the Button component and load the saved data
        button = GetComponent<Button>();
        LoadData();
    }

    private void Start()
    {
        // Initialize the UI with the current cost value
        UpdateCostText();
    }

    private void OnEnable()
    {
        // Add the Upgrade method as a listener to the button's onClick event
        button.onClick.AddListener(Upgrade);
    }

    private void OnDisable()
    {
        // Remove all listeners from the button when it is disabled
        button.onClick.RemoveAllListeners();
    }

    private void Upgrade()
    {
        // Check if the player has enough money to purchase the upgrade
        if (GameManager.Instance.CurrentMoney > cost)
        {
            // Deduct the cost from the player's money
            GameManager.Instance.CurrentMoney -= cost;

            // Apply the upgrade (specific to the upgrade type and amount)
            GameManager.Instance.Upgrade(upgradeType, upgradeAmmount);

            // Increase the cost based on the difficulty scale
            cost *= difficultyScale;

            // Update the UI to show the new cost
            UpdateCostText();
        }
    }

    private void UpdateCostText()
    {
        // Format the cost into a readable string and update the cost text
        costText.text = "$" + NumberFormatter.FormatLargeNumber(cost);
    }

    // Method to save the upgrade button's data
    public void SaveData()
    {
        var data = new ManualClickerUpgradeSaveData
        {
            UpgradeCost = cost  // Save the current cost of the upgrade
        };

        // Save the data using the button's unique ID
        GameManager.Instance.GameState.SaveUpgradeButtonData(buttonID, data);
    }

    // Method to load the upgrade button's data
    private void LoadData()
    {
        // Try to retrieve saved data for this button using its unique ID
        if (GameManager.Instance.GameState.TryGetUpgradeButtonData(buttonID, out ManualClickerUpgradeSaveData data))
        {
            // If data is found, load the saved cost value
            cost = data.UpgradeCost;
        }
    }
}