using System;
using System.Collections.Generic;

[Serializable]
public struct GeneratorSaveData
{
    public int CurrentLevel;
    public float CollectAmount;
    public float UpgradeCost;
    public bool IsLocked;
}

[Serializable]
public struct ManualClickerUpgradeSaveData
{
    public double UpgradeCost;
}

[Serializable]
public class LevelSaveData
{
    public int CurrentLevel = 1;
    public float Progress = 0.0f;
    public float LevelUpRequirement;
}

public class GameState
{
    public double CurrentMoney = 0;
    public LevelSaveData LevelSaveData = new LevelSaveData();
    public Dictionary<string, GeneratorSaveData> GeneratorsData = new Dictionary<string, GeneratorSaveData>();
    public Dictionary<string, ManualClickerUpgradeSaveData> UpgradeButtonsData = new Dictionary<string, ManualClickerUpgradeSaveData>();
    public float PointerValue;
    public bool adsDisabled = false;
    public bool hasUnlocked1MillionAchievement = false;
    public bool hasUnlocked1BillionAchievement = false;

    public float dailyRewardValue = 1000f;
    public DateTime lastDailyRewardClaimTime = DateTime.Now.AddDays(-1);

    public GameState(float levelUpRequirement, float pointerValue)
    {
        this.LevelSaveData.LevelUpRequirement = levelUpRequirement;
        this.PointerValue = pointerValue;
    }

    // Save generator data with a unique ID
    public void SaveGeneratorData(string id, GeneratorSaveData data)
    {
        if (GeneratorsData.ContainsKey(id))
        {
            GeneratorsData[id] = data;
        }
        else
        {
            GeneratorsData.Add(id, data);
        }
    }

    // Try to get saved generator data by ID
    public bool TryGetGeneratorData(string id, out GeneratorSaveData data)
    {
        return GeneratorsData.TryGetValue(id, out data);
    }

    public void SaveUpgradeButtonData(string id, ManualClickerUpgradeSaveData data)
    {
        if (UpgradeButtonsData.ContainsKey(id))
        {
            UpgradeButtonsData[id] = data;
        }
        else
        {
            UpgradeButtonsData.Add(id, data);
        }
    }

    public bool TryGetUpgradeButtonData(string id, out ManualClickerUpgradeSaveData data)
    {
        return UpgradeButtonsData.TryGetValue(id, out data);
    }
}

