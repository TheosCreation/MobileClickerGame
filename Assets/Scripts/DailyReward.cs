using System;
using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DailyReward : MonoBehaviour
{
    private Button button; // Reference to the button for claiming the daily reward
    [SerializeField] private TMP_Text rewardText; // UI text element to display reward information
    private float timeUntilNextReward; // Time in seconds until the next reward is available

    private const string LastClaimTimeKey = "LastClaimTime"; // Key used for saving and loading the last claim time

    private float baseReward = 1000f; // Base reward amount for level 1
    private float growthFactor = 1.5f; // Growth factor for increasing reward based on player level

    private void Awake()
    {
        button = GetComponent<Button>(); // Get the Button component attached to the GameObject
    }

    private void Start()
    {
        // Start the cooldown management coroutine
        StartCoroutine(DailyRewardCooldown());
    }

    private void OnEnable()
    {
        // Add a listener for the button click event
        button.onClick.AddListener(ClaimDailyReward);
    }

    private void OnDisable()
    {
        // Remove all listeners from the button to avoid memory leaks
        button.onClick.RemoveAllListeners();
    }

    private IEnumerator DailyRewardCooldown()
    {
        while (true) // Infinite loop to continuously check the cooldown status
        {
            timeUntilNextReward = GetTimeUntilNextReward(); // Calculate the time remaining until the next reward

            if (timeUntilNextReward <= 0) // If the reward is available
            {
                button.interactable = true; // Enable the button for interaction
                rewardText.text = "Claim Daily Reward " + NumberFormatter.FormatLargeNumber(GameManager.Instance.GameState.dailyRewardValue); // Update the reward text
            }
            else // If the reward is still on cooldown
            {
                // Calculate the remaining time in hours and minutes
                TimeSpan timeRemaining = TimeSpan.FromSeconds(timeUntilNextReward);
                rewardText.text = $"Daily Reward {timeRemaining.Hours:D2}:{timeRemaining.Minutes:D2}"; // Display the time remaining
                button.interactable = false; // Disable the button
            }

            yield return new WaitForSeconds(60f); // Wait for 60 seconds before updating again
        }
    }

    private float GetTimeUntilNextReward()
    {
        // Calculate the time difference between now and the last claim time, adding 24 hours for the next availability
        DateTime nextClaimTime = GameManager.Instance.GameState.lastDailyRewardClaimTime.AddDays(1);
        return (float)(nextClaimTime - DateTime.Now).TotalSeconds; // Return the time remaining in seconds
    }

    public void ClaimDailyReward()
    {
        if (timeUntilNextReward <= 0) // If the reward is available to claim
        {
            GrantReward(); // Grant the reward to the player

            // Update the last claim time to the current time
            GameManager.Instance.GameState.lastDailyRewardClaimTime = DateTime.Now;

            // Restart the cooldown coroutine
            StartCoroutine(DailyRewardCooldown());
        }
        else
        {
            Debug.Log("Reward not available yet."); // Log a message if the reward is not ready
        }
    }

    private void GrantReward()
    {
        Debug.Log("Reward claimed!"); // Log the reward claim
        GameManager.Instance.CurrentMoney += GameManager.Instance.GameState.dailyRewardValue; // Add the reward value to the player's current money
        UpdateReward(); // Update the next reward value
    }

    private void UpdateReward()
    {
        // Calculate the next reward value based on the player's level, using exponential growth
        GameManager.Instance.GameState.dailyRewardValue = baseReward * Mathf.Pow(growthFactor, GameManager.Instance.currentLevel);
    }
}