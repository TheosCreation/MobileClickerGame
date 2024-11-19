using Assets.Scripts.JsonSerialization;
using GooglePlayGames.BasicApi.SavedGame;
using GooglePlayGames.BasicApi;
using GooglePlayGames;
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
    private bool cloudLoadSuccessful = false;

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
            if (UiManager.Instance != null)
            {
                UiManager.Instance.UpdateCurrentMoneyText(value);
            }

            if (LevelManager.Instance != null)
            {
                if (m_CurrentMoney > 1000000 && !GameState.hasUnlocked1MillionAchievement)
                {
                    LevelManager.Instance.Unlock1MillionMoneyAchivement();
                    GameState.hasUnlocked1MillionAchievement = true;
                }
                if (m_CurrentMoney > 1000000000 && !GameState.hasUnlocked1BillionAchievement)
                {
                    LevelManager.Instance.Unlock1BillionMoneyAchivement();
                    GameState.hasUnlocked1BillionAchievement = true;
                }
            }
        }
    }

    private void Awake()
    {
        // Initializes the GameManager as a singleton and sets up default game state.
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

    /// <summary>
    /// Initializes the game manager, loading game data and setting save intervals.
    /// </summary>
    public IEnumerator Init()
    {
        yield return LoadGame();
        InvokeRepeating(nameof(SaveGame), 60f, 300f);
        Debug.Log("Game manager init successfully");
        yield return null;
    }

    /// <summary>
    /// Loads the game from either the cloud or a local file.
    /// </summary>
    public IEnumerator LoadGame()
    {
        yield return LoadGameFromCloudAsync();
        if (!cloudLoadSuccessful)
        {
            Debug.Log("Cloud load failed, falling back to local save.");
            UnSerializeGameStateFromJson();
        }

        m_currentLevelProgress = GameState.LevelSaveData.Progress;
        currentLevel = GameState.LevelSaveData.CurrentLevel;
        levelUpRequirement = GameState.LevelSaveData.LevelUpRequirement;
        pointerValue = GameState.PointerValue;
        CurrentMoney = GameState.CurrentMoney;
    }

    /// <summary>
    /// Saves the game to local storage and the cloud.
    /// </summary>
    public void SaveGame()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.SaveLevelObjects();
        }

        GameState.CurrentMoney = CurrentMoney;
        GameState.PointerValue = pointerValue;

        GameState.LevelSaveData.CurrentLevel = currentLevel;
        GameState.LevelSaveData.Progress = m_currentLevelProgress;
        GameState.LevelSaveData.LevelUpRequirement = levelUpRequirement;

        SerializeGameStateToJson();

        if (Social.localUser.authenticated)
        {
            SaveGameToCloud();
        }
    }

    /// <summary>
    /// Loads the game state from a JSON file.
    /// </summary>
    public void UnSerializeGameStateFromJson()
    {
        try
        {
            GameState data = DataService.LoadData<GameState>(gameStateFilePath, false);
            GameState = data ?? new GameState(levelUpRequirement, pointerValue);
            Debug.Log("Game state loaded successfully.");
        }
        catch
        {
            GameState = new GameState(levelUpRequirement, pointerValue);
            Debug.Log("Initialized fresh game state to allow the game to proceed.");
        }
    }

    /// <summary>
    /// Serialize the game state to a JSON file locally.
    /// </summary>
    public void SerializeGameStateToJson()
    {
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

    /// <summary>
    /// Load the game scene and display the loading page.
    /// </summary>
    public void LoadGameScene()
    {
        Debug.Log("Loading Game Scene... ");
        MainMenuManager.Instance.OpenLoadingPage();
        SceneManager.LoadSceneAsync("GameScene");
    }

    /// <summary>
    /// Load the main menu scene and save the current game state.
    /// </summary>
    public void LoadMainMenu()
    {
        Debug.Log("Loading Main Menu... ");
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.SaveLevelObjects();
        }
        PauseManager.Instance.UnPause();
        SceneManager.LoadScene(mainMenuScene);
    }

    /// <summary>
    /// Quit the application.
    /// </summary>
    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    /// <summary>
    /// Upgrade the player's level and update associated UI and game mechanics.
    /// </summary>
    public void UpgradeLevel()
    {
        if (m_currentLevelProgress >= levelUpRequirement)
        {
            currentLevel++;
            UploadLevelToLeaderboard();

            m_currentLevelProgress = 0.0f;
            levelUpRequirement *= levelUpMultiplier;

            UiManager.Instance.UpdateLevel(currentLevel);
            UiManager.Instance.UpdateLevelBar(m_currentLevelProgress / levelUpRequirement);

            foreach (ClickerButton generator in LevelManager.Instance.generators)
            {
                generator.UpdateLevelRequirementStatus();
            }

            AdManager.Instance.CreateInterstitial();
            AdManager.Instance.ShowInterstitial();
        }
    }

    /// <summary>
    /// Update the player's level progress and refresh the progress bar.
    /// </summary>
    /// <param name="value">Amount of progress to add.</param>
    private void UpdateLevelProgress(double value)
    {
        m_currentLevelProgress += (float)value;

        if (UiManager.Instance != null)
        {
            UiManager.Instance.UpdateLevelBar(m_currentLevelProgress / levelUpRequirement);
        }
    }

    /// <summary>
    /// Upgrade specific game mechanics, such as manual clicks.
    /// </summary>
    /// <param name="upgradeType">Type of upgrade to apply.</param>
    /// <param name="upgradeStep">Value to increase the upgrade.</param>
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

    /// <summary>
    /// Save the game state when the application quits.
    /// </summary>
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    /// <summary>
    /// Save the game state when the application is paused.
    /// </summary>
    /// <param name="pauseStatus">True if the application is paused.</param>
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGame();
        }
    }

    /// <summary>
    /// Save generator-specific data to the game state.
    /// </summary>
    /// <param name="id">Generator identifier.</param>
    /// <param name="data">Generator data to save.</param>
    public void SaveGeneratorData(string id, GeneratorSaveData data)
    {
        GameState.SaveGeneratorData(id, data);
    }

    /// <summary>
    /// Attempt to retrieve saved generator data from the game state.
    /// </summary>
    /// <param name="id">Generator identifier.</param>
    /// <param name="data">Output parameter for the generator data.</param>
    /// <returns>True if data was found; otherwise, false.</returns>
    public bool TryGetGeneratorData(string id, out GeneratorSaveData data)
    {
        return GameState.TryGetGeneratorData(id, out data);
    }

    /// <summary>
    /// Save the game state to the cloud if the player is authenticated.
    /// </summary>
    public void SaveGameToCloud()
    {
        if (Social.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(
                gameStateFilePath,
                DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime,
                OnSavedGameOpenedForSave);
        }
    }

    /// <summary>
    /// Handle the process of saving game data to the cloud after opening a save slot.
    /// </summary>
    private void OnSavedGameOpenedForSave(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(SerializeGameState());
            SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder()
                .WithUpdatedDescription("Game saved at " + DateTime.Now)
                .Build();

            PlayGamesPlatform.Instance.SavedGame.CommitUpdate(game, update, data, OnSaveGameWritten);
        }
        else
        {
            Debug.LogError("Failed to open save game for saving.");
        }
    }

    /// <summary>
    /// Serialize the game state to a JSON string.
    /// </summary>
    /// <returns>Serialized game state as a JSON string.</returns>
    private string SerializeGameState()
    {
        return JsonUtility.ToJson(GameState);
    }

    /// <summary>
    /// Deserialize the game state from a JSON string.
    /// </summary>
    /// <param name="json">JSON string representing the game state.</param>
    private void DeserializeGameState(string json)
    {
        GameState = JsonUtility.FromJson<GameState>(json);
    }

    /// <summary>
    /// Callback invoked when cloud save data has been successfully written.
    /// </summary>
    private void OnSaveGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("Game saved successfully to cloud.");
        }
        else
        {
            Debug.LogError("Failed to write save data to cloud.");
        }
    }

    /// <summary>
    /// Attempt to load the game state from the cloud asynchronously.
    /// </summary>
    public IEnumerator LoadGameFromCloudAsync()
    {
        if (Social.localUser.authenticated)
        {
            cloudLoadSuccessful = false;

            bool cloudLoadCompleted = false;
            PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(
                gameStateFilePath,
                DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime,
                (status, game) =>
                {
                    if (status == SavedGameRequestStatus.Success)
                    {
                        PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(game, (readStatus, data) =>
                        {
                            if (readStatus == SavedGameRequestStatus.Success)
                            {
                                string json = System.Text.Encoding.UTF8.GetString(data);
                                DeserializeGameState(json);
                                cloudLoadSuccessful = true;
                                Debug.Log("Game loaded successfully from cloud.");
                            }
                            else
                            {
                                Debug.LogError("Failed to read save game data from cloud.");
                            }
                            cloudLoadCompleted = true;
                        });
                    }
                    else
                    {
                        Debug.LogError("Failed to open save game for loading.");
                        cloudLoadCompleted = true;
                    }
                }
            );

            yield return new WaitUntil(() => cloudLoadCompleted);
        }
        else
        {
            Debug.LogWarning("User not authenticated for cloud services.");
        }
    }

    /// <summary>
    /// Upload the player's current level to the global and daily leaderboards.
    /// </summary>
    public void UploadLevelToLeaderboard()
    {
        if (Social.localUser.authenticated)
        {
            Social.ReportScore(currentLevel, GPGSIds.leaderboard_highest_level_leaderboard, (bool success) =>
            {
                if (success)
                {
                    Debug.Log("Successfully uploaded level to global leaderboard.");
                }
                else
                {
                    Debug.LogError("Failed to upload level to global leaderboard.");
                }
            });

            Social.ReportScore(currentLevel, GPGSIds.leaderboard_daily_highest_level_leaderboard, (bool success) =>
            {
                if (success)
                {
                    Debug.Log("Successfully uploaded level to daily leaderboard.");
                }
                else
                {
                    Debug.LogError("Failed to upload level to daily leaderboard.");
                }
            });
        }
        else
        {
            Debug.LogWarning("User not authenticated, cannot upload score to leaderboard.");
        }
    }
}
