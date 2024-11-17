using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClickerButton : MonoBehaviour
{
    public string buttonID;
    public bool isLocked = false;
    [SerializeField] private Button clickerButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button unlockButton;
    [SerializeField] private TMP_Text unlockCostText;
    [SerializeField] private TMP_Text levelProgressText;
    [SerializeField] private TMP_Text upgradeCostText;

    [SerializeField] private ProgressBar collectionProgressBar;
    [SerializeField] private float collectSpeed = 1.0f; // Base speed for auto-collect
    [SerializeField] private float progressMultiplier = 1.0f; // Difficulty multiplier
    [SerializeField] private TMP_Text collectionAmountText;
    [SerializeField] private GameObject levelLockedOverlay;
    [SerializeField] private TMP_Text levelLockedText;
    [SerializeField] private double collectionAmount = 1.0f;
    [SerializeField] private double currentUpgradeCost = 200.0f;
    [SerializeField] private double unlockCost = 10000;
    public int level = 1;
    public int levelRequiredToUnlock = 0;
    float collectionProgress = 0.0f;

    private void OnEnable()
    {
        clickerButton.onClick.AddListener(ManualCollect);
        upgradeButton.onClick.AddListener(Upgrade);
        unlockButton.onClick.AddListener(Unlock);
    }

    private void OnDisable()
    {
        clickerButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.RemoveAllListeners();
        unlockButton.onClick.RemoveAllListeners();
    }

    private void Awake()
    {
        LoadData();
    }

    private void Start()
    {
        UpdateCollectionAmountText();
        UpdateLevelText();
        UpdateUpgradeCostText();
        UpdateUnlockStatus();
        UpdateLevelRequirementStatus();
    }

    private void Update()
    {
        if(isLocked) return;

        // Increment the collection progress over time, adjusted by the multiplier
        collectionProgress += (Time.deltaTime * collectSpeed) / progressMultiplier;

        CheckProgress();
    }

    private void ManualCollect()
    {
        if (isLocked) return;

        // Increment the collection progress manually, adjusted by the multiplier
        collectionProgress += GameManager.Instance.pointerValue / progressMultiplier; // Increase by a fixed amount divided by the multiplier

        CheckProgress();
    }
    
    private void Upgrade()
    {
        if (GameManager.Instance.CurrentMoney > currentUpgradeCost)
        {
            GameManager.Instance.CurrentMoney -= currentUpgradeCost;
            level++;
            collectionAmount *= 1.5f;
            currentUpgradeCost *= 2.0f;

            UpdateCollectionAmountText();
            UpdateLevelText();
            UpdateUpgradeCostText();
        }
    }

    private void Unlock()
    {
        if(GameManager.Instance.CurrentMoney > unlockCost)
        {
            GameManager.Instance.CurrentMoney -= unlockCost;
            isLocked = false;
            UpdateUnlockStatus();
        }
    }

    void CheckProgress()
    {
        // Check if the progress has reached or exceeded 1
        if (collectionProgress >= 1.0f)
        {
            Collection();
            collectionProgress %= 1.0f; // Wrap around by taking the remainder
        }

        // Update the progress bar's value
        collectionProgressBar.value = collectionProgress;
    }

    private void Collection()
    {
        GameManager.Instance.CurrentMoney += collectionAmount;
    }

    private void UpdateCollectionAmountText()
    {
        collectionAmountText.text = "earn $" + NumberFormatter.FormatLargeNumber(collectionAmount);
    }

    private void UpdateLevelText()
    {
        levelProgressText.text = level.ToString();
    }
    private void UpdateUpgradeCostText()
    {
        upgradeCostText.text = "cost $" + NumberFormatter.FormatLargeNumber(currentUpgradeCost);
    }

    private void UpdateUnlockStatus()
    {
        unlockCostText.text = "Unlock cost $" + NumberFormatter.FormatLargeNumber(unlockCost);
        unlockButton.gameObject.SetActive(isLocked);
    }
    
    public void UpdateLevelRequirementStatus()
    {
        if(GameManager.Instance.currentLevel < levelRequiredToUnlock)
        {
            levelLockedOverlay.SetActive(true);
            levelLockedText.text = "Level required " + levelRequiredToUnlock.ToString();
        }
        else
        {
            levelLockedOverlay.SetActive(false);
        }
    }

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