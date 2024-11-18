using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardAdButton : MonoBehaviour
{
    private Button button;
    [SerializeField] private float rewardCooldown = 60f; // Cooldown in seconds
    private bool isOnCooldown = false;
    private float cooldownEndTime;

    private float baseReward = 1000f; // Base reward for level 1
    private float growthFactor = 1.5f;
    private float exponentialReward;

    [SerializeField] private TMP_Text buttonText;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("Button component is missing!");
        }
    }

    private void OnEnable()
    {
        button.onClick.AddListener(HandleAdButtonClick);
        UpdateReward();
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        // Check if the cooldown is over
        if (isOnCooldown && Time.time >= cooldownEndTime)
        {
            isOnCooldown = false;
            UpdateUI();
        }
    }

    private void HandleAdButtonClick()
    {
        // Handle the ad reward functionality
        AdManager.Instance.CreateRewarded(exponentialReward);

        // Set cooldown
        isOnCooldown = true;
        cooldownEndTime = Time.time + rewardCooldown; // Set the time when cooldown ends
        UpdateUI();
    }

    private void UpdateReward()
    {
        exponentialReward = baseReward * Mathf.Pow(growthFactor, GameManager.Instance.currentLevel);
    }

    private void UpdateUI()
    {
        if (isOnCooldown)
        {
            button.interactable = false;
            if (buttonText != null) buttonText.text = "Cooldown...";
        }
        else
        {
            button.interactable = true;
            if (buttonText != null) buttonText.text = "Watch Ad for " + NumberFormatter.FormatLargeNumber(exponentialReward);
        }
    }
}
