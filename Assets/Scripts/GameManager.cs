using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [HideInInspector] public GameState GameState;
    public string mainMenuScene = "MainMenu";

    public int currentLevel = 1;
    public float m_currentLevelProgress = 0.0f;
    public float pointerValueUpgradeStep = 0.1f;
    public float maxDifficulty = 5.0f;
    [SerializeField] private float levelUpRequirement = 100.0f;
    [SerializeField] private float levelUpMultiplier = 1.2f;
    public float pointerValue = 0.1f; // amount to collect on manual clicks

    private double m_CurrentMoney;
    public double CurrentMoney
    {
        get
        {
            return m_CurrentMoney;
        }
        set
        {
            double difference = value - m_CurrentMoney;
            if (difference > 0)
            {
                UpdateLevelProgress(difference);
            }

            m_CurrentMoney = value;
            UiManager.Instance.UpdateCurrentMoneyText(value);
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            GameState = new GameState();
        }
        else
        {
            Destroy(gameObject);
        }

        //UnSerializeGameStateFromJson();
    }

    public IEnumerator Init()
    {
        //load from save files


        Debug.Log("Game manager init sucessfully");
        yield return null;
    }

    public void LoadGame()
    {
        Debug.Log("Loading Game Scene... ");
        MainMenuManager.Instance.OpenLoadingPage();
        SceneManager.LoadSceneAsync("GameScene");
    }

    public void LoadMainMenu()
    {
        Debug.Log("Loading Main Menu... ");
        SceneManager.LoadScene(mainMenuScene);
    }
    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void UpgradeLevel()
    {
        if (m_currentLevelProgress >= levelUpRequirement)
        {
            // Level up
            currentLevel++;

            // Reset progress
            m_currentLevelProgress = 0.0f;

            // Increase the requirement for the next level
            levelUpRequirement *= levelUpMultiplier;

            // Update UI
            UiManager.Instance.UpdateLevel(currentLevel);
            UiManager.Instance.UpdateLevelBar(m_currentLevelProgress / levelUpRequirement);

            // Update generators status
            foreach(ClickerButton generator in LevelManager.Instance.generators)
            {
                generator.UpdateLevelRequirementStatus();
            }
        }
    }

    private void UpdateLevelProgress(double value)
    {
        m_currentLevelProgress += (float)value;

        // Update the level bar based on the new requirement
        UiManager.Instance.UpdateLevelBar(m_currentLevelProgress / levelUpRequirement);
    }

    public void Upgrade(UpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case UpgradeType.ManualClick:
                pointerValue = Mathf.Min(pointerValue + pointerValueUpgradeStep, maxDifficulty);
                break;
            default:
                break;
        }
    }
}
