using GoogleMobileAds.Api;
using UnityEngine;

public class AdsInit : MonoBehaviour
{
    // Stores the initialization status of Google Mobile Ads
    InitializationStatus m_initStatus;

    /// <summary>
    /// Initializes Google Mobile Ads SDK.
    /// </summary>
    public void Init()
    {
        // Initialize the Mobile Ads SDK and log the status upon completion
        MobileAds.Initialize(m_initStatus =>
        {
            Debug.Log("Google ads are initialized");
        });
    }
}