using Assets.Scripts.JsonSerialization;
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
    public float maxDifficulty = 5.0f;
    public float levelUpRequirement = 100.0f;
    [SerializeField] private float levelUpMultiplier = 1.2f;
    public float pointerValue = 0.1f; // amount to collect on manual clicks

    private IDataService DataService = new JsonDataService();
    private const string gameStateFilePath = "/game-state.json";

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

            GameState = new GameState(levelUpRequirement, pointerValue);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public IEnumerator Init()
    {
        //load from save files
        UnSerializeGameStateFromJson();

        Debug.Log("Game manager init sucessfully");
        yield return null;
    }

    public void UnSerializeGameStateFromJson()
    {
        try
        {
            GameState data = DataService.LoadData<GameState>(gameStateFilePath, false);
            if (data != null)
            {
                GameState = data; // Load the game state if data is present
                Debug.Log("Game state loaded successfully.");
            }
            else
            {
                // If the file doesn't exist or is empty, ensure a fresh start
                GameState = new GameState(levelUpRequirement, pointerValue);
                Debug.Log("No game state data found, starting fresh.");
            }
        }
        catch
        {
            // Initialize default game state to ensure gameplay can continue
            GameState = new GameState(levelUpRequirement, pointerValue);
            Debug.Log("Initialized fresh game state to allow the game to proceed.");
        }

        m_currentLevelProgress = GameState.LevelSaveData.Progress;
        currentLevel = GameState.LevelSaveData.CurrentLevel;
        levelUpRequirement = GameState.LevelSaveData.LevelUpRequirement;

        pointerValue = GameState.PointerValue;
        CurrentMoney = GameState.CurrentMoney;
    }

    public void SerializeGameStateToJson()
    {
        // Update values then save
        GameState.CurrentMoney = CurrentMoney;
        GameState.PointerValue = pointerValue;

        GameState.LevelSaveData.CurrentLevel = currentLevel;
        GameState.LevelSaveData.Progress = m_currentLevelProgress;
        GameState.LevelSaveData.LevelUpRequirement = levelUpRequirement;

        long startTime = DateTime.Now.Ticks;
        try
        {
            if (!DataService.SaveData(gameStateFilePath, GameState, false))
            {
                Debug.LogError("Failed to save game state.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error saving game state: " + ex.Message);
        }
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
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.SaveLevelObjects();
        }
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

            // Display ad
            AdManager.Instance.CreateInterstitial();
            AdManager.Instance.ShowInterstitial();
        }
    }

    private void UpdateLevelProgress(double value)
    {
        m_currentLevelProgress += (float)value;

        // Update the level bar based on the new requirement
        if (UiManager.Instance != null)
        {
            UiManager.Instance.UpdateLevelBar(m_currentLevelProgress / levelUpRequirement);
        }
    }

    public void Upgrade(UpgradeType upgradeType, float upgradeStep = 0.0f)
    {
        switch (upgradeType)
        {
            case UpgradeType.ManualClick:
                pointerValue = Mathf.Min(pointerValue + upgradeStep, maxDifficulty);
                break;
            default:
                break;
        }
    }

    private void OnApplicationQuit()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.SaveLevelObjects();
        }
        SerializeGameStateToJson();
    }

    public void SaveGeneratorData(string id, GeneratorSaveData data)
    {
        GameState.SaveGeneratorData(id, data);
    }

    public bool TryGetGeneratorData(string id, out GeneratorSaveData data)
    {
        return GameState.TryGetGeneratorData(id, out data);
    }
}
