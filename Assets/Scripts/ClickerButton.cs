using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClickerButton : MonoBehaviour
{
    [SerializeField] private Button clickerButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private ProgressBar levelProgressBar;
    [SerializeField] private TMP_Text levelProgressText;

    [SerializeField] private ProgressBar collectionProgressBar;
    [SerializeField] private float collectSpeed = 1.0f; // Base speed for auto-collect
    [SerializeField] private float progressMultiplier = 1.0f; // Difficulty multiplier
    [SerializeField] private TMP_Text collectionAmountText;

    [SerializeField] private double collectionAmount = 1.0f;
    [SerializeField] private double currentUpgradeCost = 200.0f;
    private int level = 1;
    float collectionProgress = 0.0f;

    private void OnEnable()
    {
        clickerButton.onClick.AddListener(ManualCollect);
        upgradeButton.onClick.AddListener(Upgrade);
    }

    private void OnDisable()
    {
        clickerButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        UpdateCollectionAmountText();
        UpdateLevelText();
    }

    private void Update()
    {
        // Increment the collection progress over time, adjusted by the multiplier
        collectionProgress += (Time.deltaTime * collectSpeed) / progressMultiplier;

        CheckProgress();
    }

    private void ManualCollect()
    {
        // Increment the collection progress manually, adjusted by the multiplier
        collectionProgress += 0.1f / progressMultiplier; // Increase by a fixed amount divided by the multiplier

        CheckProgress();
    }
    
    private void Upgrade()
    {
        if (GameManager.Instance.CurrentMoney > currentUpgradeCost)
        {
            level++;
            collectionAmount *= 1.5f;
            UpdateCollectionAmountText();

            GameManager.Instance.CurrentMoney -= currentUpgradeCost;
            currentUpgradeCost *= 2.0f;
            UpdateLevelText();
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
        collectionAmountText.text = "earn $ " + NumberFormatter.FormatLargeNumber(collectionAmount);
    }

    private void UpdateLevelText()
    {
        levelProgressText.text = level.ToString();
    }
}