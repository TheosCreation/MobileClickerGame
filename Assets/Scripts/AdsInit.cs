using GoogleMobileAds.Api;
using UnityEngine;

public class AdsInit : MonoBehaviour
{
    InitializationStatus m_initStatus;

    public void Init()
    {
        MobileAds.Initialize(m_initStatus =>
        {
            Debug.Log("Goggle ads are init");
        });
    }
}
