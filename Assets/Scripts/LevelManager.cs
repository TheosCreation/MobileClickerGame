using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Transform generatorHolder;
    [HideInInspector] public ClickerButton[] generators;
    [SerializeField] private Transform upgradesHolder;
    [HideInInspector] public UpgradeButton[] upgrades;
    public static LevelManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        generators = generatorHolder.GetComponentsInChildren<ClickerButton>();
        upgrades = upgradesHolder.GetComponentsInChildren<UpgradeButton>();
    }

    private void Start()
    {
        UnlockLoginForFirstTimeAchivement();
    }

    private void UnlockLoginForFirstTimeAchivement()
    {
        if (Social.localUser.authenticated)
        {
            Social.ReportProgress(
                GPGSIds.achievement_first_time_login,
                100.0,
                (bool _success) =>
                {
                    Social.ShowAchievementsUI();
                });

        }
    }
    public void Unlock1MillionMoneyAchivement()
    {
        if (Social.localUser.authenticated)
        {
            Social.ReportProgress(
                GPGSIds.achievement_reach_1_million_dollars,
                100.0,
                (bool _success) =>
                {
                    Social.ShowAchievementsUI();
                });

        }
    }

    public void Unlock1BillionMoneyAchivement()
    {
        if (Social.localUser.authenticated)
        {
            Social.ReportProgress(
                GPGSIds.achievement_reach_1_billion_dollar,
                100.0,
                (bool _success) =>
                {
                    Social.ShowAchievementsUI();
                });

        }
    }

    public void SaveLevelObjects()
    {
        foreach (var generator in generators)
        {
            generator.SaveData();
        }
        
        foreach (var upgrade in upgrades)
        {
            upgrade.SaveData();
        }
    }

}