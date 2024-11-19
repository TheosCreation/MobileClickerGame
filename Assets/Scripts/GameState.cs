using System;
using System.Collections.Generic;

[Serializable]
public struct GeneratorSaveData
{
    // Stores the current level of the generator.
    public int CurrentLevel;
    // The amount collected by the generator.
    public float CollectAmount;
    // The cost required to upgrade the generator.
    public float UpgradeCost;
    // Indicates whether the generator is locked.
    public bool IsLocked;
}

[Serializable]
public struct ManualClickerUpgradeSaveData
{
    // The cost to upgrade the manual clicker.
    public double UpgradeCost;
}

[Serializable]
public class LevelSaveData
{
    // The current player level.
    public int CurrentLevel = 1;
    // The current progress toward the next level.
    public float Progress = 0.0f;
    // The requirement to level up.
    public float LevelUpRequirement;
}

public class GameState
{
    // The current amount of money the player has.
    public double CurrentMoney = 0;
    // Stores level-related data for the player.
    public LevelSaveData LevelSaveData = new LevelSaveData();
    // Dictionary to store data for each generator using a unique string identifier.
    public Dictionary<string, GeneratorSaveData> GeneratorsData = new Dictionary<string, GeneratorSaveData>();
    // Dictionary to store data for each manual clicker upgrade button.
    public Dictionary<string, ManualClickerUpgradeSaveData> UpgradeButtonsData = new Dictionary<string, ManualClickerUpgradeSaveData>();
    // The value of the manual clicker.
    public float PointerValue;
    // Indicates whether ads have been disabled in the game.
    public bool adsDisabled = false;
    // Achievement flags for specific milestones.
    public bool hasUnlocked1MillionAchievement = false;
    public bool hasUnlocked1BillionAchievement = false;
    // Indicates if the player has logged into the game.
    public bool hasLoggedIn = false;
    // The reward amount for daily rewards.
    public float dailyRewardValue = 1000f;
    // The last time the player claimed a daily reward.
    public DateTime lastDailyRewardClaimTime = DateTime.Now.AddDays(-1);
    // The last date for the most recent notification scheduled for daily notifications
    public string LastNotificationDate = "";

    // Constructor to initialize the level-up requirement and pointer value.
    public GameState(float levelUpRequirement, float pointerValue)
    {
        this.LevelSaveData.LevelUpRequirement = levelUpRequirement;
        this.PointerValue = pointerValue;
    }

    /// <summary>
    /// Save generator data with a unique identifier.
    /// </summary>
    /// <param name="id">Unique ID for the generator.</param>
    /// <param name="data">Data to be saved for the generator.</param>
    public void SaveGeneratorData(string id, GeneratorSaveData data)
    {
        if (GeneratorsData.ContainsKey(id))
        {
            GeneratorsData[id] = data; // Update the existing data.
        }
        else
        {
            GeneratorsData.Add(id, data); // Add new data.
        }
    }

    /// <summary>
    /// Attempt to retrieve saved generator data using a unique identifier.
    /// </summary>
    /// <param name="id">Unique ID for the generator.</param>
    /// <param name="data">Output parameter to hold the retrieved data.</param>
    /// <returns>True if the data was found; otherwise, false.</returns>
    public bool TryGetGeneratorData(string id, out GeneratorSaveData data)
    {
        return GeneratorsData.TryGetValue(id, out data);
    }

    /// <summary>
    /// Save upgrade button data with a unique identifier.
    /// </summary>
    /// <param name="id">Unique ID for the upgrade button.</param>
    /// <param name="data">Data to be saved for the upgrade button.</param>
    public void SaveUpgradeButtonData(string id, ManualClickerUpgradeSaveData data)
    {
        if (UpgradeButtonsData.ContainsKey(id))
        {
            UpgradeButtonsData[id] = data; // Update the existing data.
        }
        else
        {
            UpgradeButtonsData.Add(id, data); // Add new data.
        }
    }

    /// <summary>
    /// Attempt to retrieve saved upgrade button data using a unique identifier.
    /// </summary>
    /// <param name="id">Unique ID for the upgrade button.</param>
    /// <param name="data">Output parameter to hold the retrieved data.</param>
    /// <returns>True if the data was found; otherwise, false.</returns>
    public bool TryGetUpgradeButtonData(string id, out ManualClickerUpgradeSaveData data)
    {
        return UpgradeButtonsData.TryGetValue(id, out data);
    }
}