using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Purchasing;

public class AdManager : MonoBehaviour
{
    // Singleton instance of AdManager
    public static AdManager Instance { get; private set; }

    // Banner Ad
    private BannerView m_banner;
    private string m_bannerID = "ca-app-pub-3940256099942544/6300978111"; // Test ad unit ID

    // Interstitial Ad
    private InterstitialAd m_interstitial;
    private string m_interstitialID = "ca-app-pub-3898769752999198/8336587416"; // Production ad unit ID

    // Rewarded Ad
    private RewardedAd m_rewarded;
    private string m_rewardedID = "ca-app-pub-3898769752999198/8426158692"; // Production ad unit ID
    private float currentReward = 0f; // Current reward amount for rewarded ads

    private void Awake()
    {
        // Ensure a single instance of AdManager persists across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialize interstitial ads when the game starts
        CreateInterstitial();
    }

    /// <summary>
    /// Creates a banner ad instance and positions it at the top of the screen.
    /// </summary>
    void CreateBanner()
    {
        Debug.Log("Creating banner");

        // Destroy any existing banner to avoid duplicates
        if (m_banner != null)
        {
            DestroyBanner();
        }

        // Create banner only if ads are not disabled
        if (!GameManager.Instance.GameState.adsDisabled)
        {
            m_banner = new BannerView(m_bannerID, AdSize.Banner, AdPosition.Top);
        }
    }

    /// <summary>
    /// Loads a banner ad for display.
    /// </summary>
    private void LoadBanner()
    {
        // Skip loading if ads are disabled
        if (GameManager.Instance.GameState.adsDisabled) return;

        // Create a new banner instance if none exists
        if (m_banner == null)
        {
            CreateBanner();
        }

        // Request and load the banner ad
        var adRequest = new AdRequest();
        m_banner.LoadAd(adRequest);
    }

    /// <summary>
    /// Destroys the current banner ad instance.
    /// </summary>
    private void DestroyBanner()
    {
        if (m_banner != null)
        {
            m_banner.Destroy();
            m_banner = null;
        }
    }

    /// <summary>
    /// Creates and loads an interstitial ad.
    /// </summary>
    public void CreateInterstitial()
    {
        // Skip creation if ads are disabled
        if (GameManager.Instance.GameState.adsDisabled) return;

        // Request and load the interstitial ad
        AdRequest adRequest = new AdRequest();
        InterstitialAd.Load(m_interstitialID, adRequest, (InterstitialAd ad, LoadAdError err) =>
        {
            if (err != null || ad == null)
            {
                Debug.LogError("Interstitial ad failed to load: " + err);
                return;
            }

            Debug.Log("Interstitial ad loaded with response: " + ad.GetResponseInfo());
            m_interstitial = ad;
        });
    }

    /// <summary>
    /// Destroys the current interstitial ad instance.
    /// </summary>
    public void DestroyInterstitial()
    {
        if (m_interstitial != null)
        {
            m_interstitial.Destroy();
            m_interstitial = null;
        }
    }

    /// <summary>
    /// Shows the interstitial ad if it is loaded.
    /// </summary>
    public void ShowInterstitial()
    {
        if (m_interstitial != null)
        {
            m_interstitial.Show();
        }
    }

    /// <summary>
    /// Creates a rewarded ad and sets the reward amount.
    /// </summary>
    /// <param name="Amount">The reward amount for the user.</param>
    public void CreateRewarded(float Amount)
    {
        Debug.Log("Creating rewarded ad");

        // Update reward amount if different from the current value
        if (currentReward != Amount)
        {
            currentReward = Amount;
        }

        // Destroy any existing rewarded ad to avoid conflicts
        if (m_rewarded != null)
        {
            DestroyRewarded();
        }

        // Request and load the rewarded ad
        AdRequest adRequest = new AdRequest();
        RewardedAd.Load(m_rewardedID, adRequest, (RewardedAd ad, LoadAdError err) =>
        {
            if (err != null || ad == null)
            {
                Debug.LogError("Rewarded ad failed to load: " + err);
                return;
            }

            Debug.Log("Rewarded ad loaded with response: " + ad.GetResponseInfo());
            m_rewarded = ad;
            ShowRewarded();
        });
    }

    /// <summary>
    /// Destroys the current rewarded ad instance.
    /// </summary>
    private void DestroyRewarded()
    {
        if (m_rewarded != null)
        {
            m_rewarded = null;
        }
    }

    /// <summary>
    /// Shows the rewarded ad if it is loaded.
    /// </summary>
    public void ShowRewarded()
    {
        if (m_rewarded != null)
        {
            m_rewarded.Show(HandleUserEarnedReward);
        }
    }

    /// <summary>
    /// Handles the reward logic after the user completes watching a rewarded ad.
    /// </summary>
    /// <param name="reward">The reward data provided by the ad.</param>
    private void HandleUserEarnedReward(Reward reward)
    {
        Debug.Log("User rewarded with " + NumberFormatter.FormatLargeNumber(currentReward));
        GameManager.Instance.CurrentMoney += currentReward;
    }

    /// <summary>
    /// Disables all ads and destroys any existing ad instances.
    /// </summary>
    public void DisableAds()
    {
        Debug.Log("Disabling all ads");
        GameManager.Instance.GameState.adsDisabled = true;

        // Destroy all ad instances
        DestroyBanner();
        DestroyInterstitial();
        DestroyRewarded();
    }

}