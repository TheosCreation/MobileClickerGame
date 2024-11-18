using GoogleMobileAds.Api;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }

    // Banner Ad
    private BannerView m_banner;
    private string m_bannerID = "ca-app-pub-3940256099942544/6300978111"; //test ad

    // Interstitial Ad
    private InterstitialAd m_interstitial;
    private string m_interstitialID = "ca-app-pub-3898769752999198/8336587416"; //from ad mob

    // Rewarded Ad
    private RewardedAd m_rewarded;
    private string m_rewardedID = "ca-app-pub-3898769752999198/8426158692"; //from ad mob
    private float currentReward = 0f; 

    private void Awake()
    {
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
        //LoadBanner();
        CreateInterstitial();
    }

    void CreateBanner()
    {
        Debug.Log("Creating banner");

        if (m_banner != null)
        {
            DestroyBanner();
        }

        // Only create the banner if ads are not disabled
        if (!GameManager.Instance.GameState.adsDisabled)
        {
            m_banner = new BannerView(m_bannerID, AdSize.Banner, AdPosition.Top);
        }
    }

    private void LoadBanner()
    {
        if (GameManager.Instance.GameState.adsDisabled) return;

        if (m_banner == null)
        {
            CreateBanner();
        }

        var adRequest = new AdRequest();
        m_banner.LoadAd(adRequest);
    }

    private void DestroyBanner()
    {
        if (m_banner != null)
        {
            m_banner.Destroy();
            m_banner = null;
        }
    }

    public void CreateInterstitial()
    {
        if (GameManager.Instance.GameState.adsDisabled) return;

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

    public void DestroyInterstitial()
    {
        if (m_interstitial != null)
        {
            m_interstitial.Destroy();
            m_interstitial = null;
        }
    }

    public void ShowInterstitial()
    {
        if (m_interstitial != null)
        {
            m_interstitial.Show();
        }
    }

    public void CreateRewarded(float Amount)
    {
        Debug.Log("Creating rewarded ad");

        if (currentReward != Amount)
        {
            currentReward = Amount;
        }

        if (m_rewarded != null)
        {
            DestroyRewarded();
        }

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

    private void DestroyRewarded()
    {
        if (m_rewarded != null)
        {
            m_rewarded = null;
        }
    }

    public void ShowRewarded()
    {
        if (m_rewarded != null)
        {
            m_rewarded.Show(HandleUserEarnedReward);
        }
    }

    private void HandleUserEarnedReward(Reward reward)
    {
        Debug.Log("User rewarded with " + NumberFormatter.FormatLargeNumber(currentReward));
        GameManager.Instance.CurrentMoney += currentReward;
    }

    public void DisableAds()
    {
        Debug.Log("Disabling all ads");
        GameManager.Instance.GameState.adsDisabled = true;

        DestroyBanner();
        DestroyInterstitial();
        DestroyRewarded();
    }
}