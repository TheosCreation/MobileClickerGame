using System.Collections;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Core.Environments;

public class UnityServicesInit : MonoBehaviour
{
    public string m_environment = "production";
    public bool m_initialised { get; private set; }

    public IEnumerator Init()
    {
        var options = new InitializationOptions().SetEnvironmentName(m_environment);

        // Initialize Unity Services outside the try-catch block
        var initTask = UnityServices.InitializeAsync(options);

        yield return initTask; // Wait for initialization to complete

        if (initTask.IsCompleted)
        {
            try
            {
                if (UnityServices.State == ServicesInitializationState.Initialized)
                {
                    Debug.Log("Unity Services initialized successfully");
                    m_initialised = true;
                }
                else
                {
                    Debug.Log("Unity Services initialization failed");
                    m_initialised = false;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Unity Services failed to init with Exception: " + e);
                m_initialised = false;
            }
        }
        else
        {
            Debug.LogError("Unity Services initialization task did not complete.");
            m_initialised = false;
        }
    }
}
