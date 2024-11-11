using GoogleMobileAds.Api;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }
    BannerView m_banner;
    string m_bannerID = "ca-app-pub-3940256099942544/6300978111";

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


    void CreateBanner()
    {
        Debug.Log("Creating banner");
        if(m_banner  != null)
        {
            DestroyAd();
        }

        m_banner = new BannerView(m_bannerID, AdSize.Banner, AdPosition.Top);
    }

    private void DestroyAd()
    {
        m_banner.Destroy(); 
        m_banner = null;
    }

    private void Load()
    {
        if (m_banner == null)
        {
            CreateBanner();
        }

        var adreq = new AdRequest();
        m_banner.LoadAd(adreq);
        //m_banner.Hide();
    }
}
