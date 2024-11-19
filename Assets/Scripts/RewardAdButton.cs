using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This class handles the functionality of a reward ad button,
// including cooldown logic and reward calculation based on the player's level.
public class RewardAdButton : MonoBehaviour
{
    private Button button;  // Button component attached to this object
    [SerializeField] private float rewardCooldown = 60f; // Cooldown in seconds between ad rewards
    private bool isOnCooldown = false;  // Flag to track if the button is on cooldown
    private float cooldownEndTime;  // Time when cooldown will end

    private float baseReward = 1000f; // Base reward value at level 1
    private float growthFactor = 1.5f;  // Growth factor for increasing rewards
    private float exponentialReward;  // Final calculated reward, based on the player's level

    [SerializeField] private TMP_Text buttonText;  // Reference to the text component on the button

    private void Awake()
    {
        // Get the Button component and ensure it's attached
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("Button component is missing!");  // Log an error if the button is not found
        }
    }

    private void OnEnable()
    {
        // Add listener to the button click event when the button is enabled
        button.onClick.AddListener(HandleAdButtonClick);
        UpdateReward();  // Update the reward calculation when enabled
    }

    private void OnDisable()
    {
        // Remove the button click listener when the button is disabled
        button.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        // Initialize UI based on the current state
        UpdateUI();
    }

    private void Update()
    {
        // If the button is on cooldown, check if the cooldown time has passed
        if (isOnCooldown && Time.time >= cooldownEndTime)
        {
            isOnCooldown = false;  // Cooldown is over
            UpdateUI();  // Update UI to reflect the button's state
        }
    }

    private void HandleAdButtonClick()
    {
        // Handle the ad reward functionality when the button is clicked
        AdManager.Instance.CreateRewarded(exponentialReward);  // Trigger the ad and reward the player

        // Start the cooldown after the ad is watched
        isOnCooldown = true;
        cooldownEndTime = Time.time + rewardCooldown;  // Set the cooldown end time based on the current time
        UpdateUI();  // Update the UI to reflect the cooldown state
    }

    private void UpdateReward()
    {
        // Update the reward based on the player's current level
        exponentialReward = baseReward * Mathf.Pow(growthFactor, GameManager.Instance.currentLevel);
    }

    private void UpdateUI()
    {
        // Update the button's interactability and the button text based on the cooldown status
        if (isOnCooldown)
        {
            button.interactable = false;  // Disable the button while on cooldown
            if (buttonText != null) buttonText.text = "Cooldown...";  // Show "Cooldown..." on the button
        }
        else
        {
            button.interactable = true;  // Enable the button if not on cooldown
            if (buttonText != null)
                buttonText.text = "Watch Ad for " + NumberFormatter.FormatLargeNumber(exponentialReward);  // Show the reward amount
        }
    }
}
