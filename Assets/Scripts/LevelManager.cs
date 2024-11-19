using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // The parent object containing all the generator buttons.
    [SerializeField] private Transform generatorHolder;
    // Array to store all generator buttons in the level.
    [HideInInspector] public ClickerButton[] generators;

    // The parent object containing all the upgrade buttons.
    [SerializeField] private Transform upgradesHolder;
    // Array to store all upgrade buttons in the level.
    [HideInInspector] public UpgradeButton[] upgrades;

    // Singleton instance of the LevelManager for global access.
    public static LevelManager Instance { get; private set; }

    private void Awake()
    {
        // Ensure only one instance of the LevelManager exists.
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances.
        }

        // Retrieve all generator buttons as children of the generator holder.
        generators = generatorHolder.GetComponentsInChildren<ClickerButton>();
        // Retrieve all upgrade buttons as children of the upgrades holder.
        upgrades = upgradesHolder.GetComponentsInChildren<UpgradeButton>();
    }

    /// <summary>
    /// Unlock the "1 Million Money" achievement if the user is authenticated.
    /// </summary>
    public void Unlock1MillionMoneyAchivement()
    {
        // Check if the user is logged into their social account.
        if (Social.localUser.authenticated)
        {
            // Report progress for the 1 million achievement.
            Social.ReportProgress(
                GPGSIds.achievement_reach_1_million_dollars, // Achievement ID.
                100.0, // Progress percentage.
                (bool _success) => // Callback to handle the result.
                {
                    // Show the achievements UI if progress is successfully reported.
                    Social.ShowAchievementsUI();
                });
        }
    }

    /// <summary>
    /// Unlock the "1 Billion Money" achievement if the user is authenticated.
    /// </summary>
    public void Unlock1BillionMoneyAchivement()
    {
        // Check if the user is logged into their social account.
        if (Social.localUser.authenticated)
        {
            // Report progress for the 1 billion achievement.
            Social.ReportProgress(
                GPGSIds.achievement_reach_1_billion_dollar, // Achievement ID.
                100.0, // Progress percentage.
                (bool _success) => // Callback to handle the result.
                {
                    // Show the achievements UI if progress is successfully reported.
                    Social.ShowAchievementsUI();
                });
        }
    }

    /// <summary>
    /// Save the data of all generators and upgrades in the level.
    /// </summary>
    public void SaveLevelObjects()
    {
        // Iterate through all generator buttons and save their data.
        foreach (var generator in generators)
        {
            generator.SaveData();
        }

        // Iterate through all upgrade buttons and save their data.
        foreach (var upgrade in upgrades)
        {
            upgrade.SaveData();
        }
    }
}