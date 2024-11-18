using System;
using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DailyReward : MonoBehaviour
{
    private Button button;
    [SerializeField] private TMP_Text rewardText;
    private float timeUntilNextReward; // Time in seconds until the next reward is available

    private const string LastClaimTimeKey = "LastClaimTime";

    private float baseReward = 1000f; // Base reward for level 1
    private float growthFactor = 1.5f; // 10% increase per level

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        // Start the cooldown management
        StartCoroutine(DailyRewardCooldown());
    }
    private void OnEnable()
    {
        button.onClick.AddListener(ClaimDailyReward);
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }

    private IEnumerator DailyRewardCooldown()
    {
        while (true)
        {
            timeUntilNextReward = GetTimeUntilNextReward();

            if (timeUntilNextReward <= 0)
            {
                button.interactable = true;
                rewardText.text = "Claim Daily Reward " + NumberFormatter.FormatLargeNumber(GameManager.Instance.GameState.dailyRewardValue);
            }
            else
            {
                // Calculate remaining time in hours and minutes
                TimeSpan timeRemaining = TimeSpan.FromSeconds(timeUntilNextReward);
                rewardText.text = $"Daily Reward {timeRemaining.Hours:D2}:{timeRemaining.Minutes:D2}";
                button.interactable = false;
            }

            yield return new WaitForSeconds(60f); // Update every minute
        }
    }

    private float GetTimeUntilNextReward()
    {
        // Calculate the time difference between now and the last claim time
        DateTime nextClaimTime = GameManager.Instance.GameState.lastDailyRewardClaimTime.AddDays(1); // Reward is available 24 hours after the last claim
        return (float)(nextClaimTime - DateTime.Now).TotalSeconds;
    }

    public void ClaimDailyReward()
    {
        if (timeUntilNextReward <= 0)
        {
            GrantReward();

            // Update the last claim time to now
            GameManager.Instance.GameState.lastDailyRewardClaimTime = DateTime.Now;

            // Start the UI update again
            StartCoroutine(DailyRewardCooldown());
        }
        else
        {
            Debug.Log("Reward not available yet.");
        }
    }

    private void GrantReward()
    {
        Debug.Log("Reward claimed!");
        GameManager.Instance.CurrentMoney += GameManager.Instance.GameState.dailyRewardValue;
        UpdateReward();
    }

    private void UpdateReward()
    {
        GameManager.Instance.GameState.dailyRewardValue = baseReward * Mathf.Pow(growthFactor, GameManager.Instance.currentLevel);
    }
}