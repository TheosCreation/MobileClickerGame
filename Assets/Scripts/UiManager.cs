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
    [SerializeField] private Button shopButton;
    [SerializeField] private ShopMenu shopMenu;
    [SerializeField] private Button pauseButton;
    [SerializeField] private PauseMenu pauseMenu;

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
        shopButton.onClick.AddListener(OpenShopMenu);
        pauseButton.onClick.AddListener(OpenPauseMenu);
    }

    private void OnDisable()
    {
        upgradeLevelButton.onClick.RemoveAllListeners();
        shopButton.onClick.RemoveAllListeners();
        pauseButton.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        UpdateCurrentMoneyText(GameManager.Instance.CurrentMoney);
        UpdateLevelBar(GameManager.Instance.m_currentLevelProgress / GameManager.Instance.levelUpRequirement);
        UpdateLevel(GameManager.Instance.currentLevel);

        ClosePauseMenu();
        CloseShopMenu();
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

    public void OpenShopMenu()
    {
        shopMenu.gameObject.SetActive(true);
    }

    public void CloseShopMenu()
    {
        shopMenu.gameObject.SetActive(false);
    }

    public void OpenPauseMenu()
    {
        PauseManager.Instance.Pause();
        pauseMenu.gameObject.SetActive(true);
    }
    public void ClosePauseMenu()
    {
        PauseManager.Instance.UnPause();
        pauseMenu.gameObject.SetActive(false);
    }
}