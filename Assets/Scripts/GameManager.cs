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
        // Attempt to load from cloud first
        yield return LoadGame();

        InvokeRepeating(nameof(SaveGame), 60f, 300f); //Save game after 60 seconds and every 300 seconds after

        Debug.Log("Game manager init sucessfully");
        yield return null;
    }

    public IEnumerator LoadGame()
    {
        // Attempt to load from cloud first
        yield return LoadGameFromCloudAsync();

        // If cloud load failed, fall back to local save
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

    public void SaveGame()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.SaveLevelObjects();
        }

        // Update values before saving
        GameState.CurrentMoney = CurrentMoney;
        GameState.PointerValue = pointerValue;

        GameState.LevelSaveData.CurrentLevel = currentLevel;
        GameState.LevelSaveData.Progress = m_currentLevelProgress;
        GameState.LevelSaveData.LevelUpRequirement = levelUpRequirement;

        // Save locally
        SerializeGameStateToJson();

        // Save to cloud (if the player is authenticated)
        if (Social.localUser.authenticated)
        {
            SaveGameToCloud();
        }
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
    }

    
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

    public void LoadGameScene()
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
        PauseManager.Instance.UnPause();
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

            // Upload the current level to the leaderboards
            UploadLevelToLeaderboard();

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
        SaveGame();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGame();
        }
    }

    public void SaveGeneratorData(string id, GeneratorSaveData data)
    {
        GameState.SaveGeneratorData(id, data);
    }

    public bool TryGetGeneratorData(string id, out GeneratorSaveData data)
    {
        return GameState.TryGetGeneratorData(id, out data);
    }

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
    private string SerializeGameState()
    {
        return JsonUtility.ToJson(GameState);
    }

    private void DeserializeGameState(string json)
    {
        GameState = JsonUtility.FromJson<GameState>(json);
    }
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

    public IEnumerator LoadGameFromCloudAsync()
    {
        if (Social.localUser.authenticated)
        {
            // Reset flag
            cloudLoadSuccessful = false;

            // Start the async cloud load process
            bool cloudLoadCompleted = false;
            PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(
                gameStateFilePath,
                DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime,
                (status, game) => {
                    if (status == SavedGameRequestStatus.Success)
                    {
                        PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(game, (readStatus, data) => {
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

            // Wait until the cloud load process completes
            yield return new WaitUntil(() => cloudLoadCompleted);
        }
        else
        {
            Debug.LogWarning("User not authenticated for cloud services.");
        }
    }

    private void OnSavedGameOpenedForLoad(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(game, OnSaveGameDataRead);
        }
        else
        {
            Debug.LogError("Failed to open save game for loading.");
        }
    }

    private void OnSaveGameDataRead(SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            string json = System.Text.Encoding.UTF8.GetString(data);
            DeserializeGameState(json);
            Debug.Log("Game loaded successfully from cloud.");
        }
        else
        {
            Debug.LogError("Failed to read save game data.");
        }
    }

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
