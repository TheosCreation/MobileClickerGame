using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; private set; }

    [SerializeField] private TMP_Text totalMoneyText;
    [SerializeField] private ProgressBar levelProgressBar;
    [SerializeField] private TMP_Text currentLevelText;
    [SerializeField] private Button upgradeLevelButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void OnEnable()
    {
        upgradeLevelButton.onClick.AddListener(GameManager.Instance.UpgradeLevel);
    }

    private void OnDisable()
    {
        upgradeLevelButton.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        UpdateCurrentMoneyText(GameManager.Instance.CurrentMoney);
        UpdateLevelBar(GameManager.Instance.m_currentLevelProgress / GameManager.Instance.levelUpRequirement);
        UpdateLevel(GameManager.Instance.currentLevel);
    }


    public void UpdateCurrentMoneyText(double amount)
    {
        totalMoneyText.text = "$" + NumberFormatter.FormatLargeNumber(amount);
    }

    public void UpdateLevelBar(float levelBarProgress)
    {
        levelProgressBar.value = levelBarProgress;
    }

    public void UpdateLevel(int currentLevel)
    {
        currentLevelText.text = "Level " + currentLevel.ToString();
    }
}