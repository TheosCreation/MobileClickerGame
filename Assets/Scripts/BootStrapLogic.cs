using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapLogic : MonoBehaviour
{
    // Reference to Unity Services initializer
    [SerializeField] private UnityServicesInit unityServicesInitilizer;

    // Reference to Play Games authentication logic
    [SerializeField] private PlayGamesAuth playGamesAuth;

    // Reference to Ads initialization logic
    [SerializeField] private AdsInit adsInit;

    // Flag to determine whether to load the main menu after initialization
    [SerializeField] private bool loadMainMenu = true;

    // Reference to notification permissions logic
    [SerializeField] private NotificationPermissions notificationPermissions;

    /// <summary>
    /// Called when the script is started. Begins the validation and initialization sequence.
    /// </summary>
    private void Start()
    {
        StartCoroutine(Validate());
    }

    /// <summary>
    /// Validates and initializes required services and managers before loading the main menu.
    /// </summary>
    private IEnumerator Validate()
    {
        // Wait for one frame to ensure the scene is ready
        yield return null;

        // Initialize ads
        adsInit.Init();

        // Initialize Play Games authentication
        yield return StartCoroutine(playGamesAuth.Init());

        // Initialize Unity services
        yield return StartCoroutine(unityServicesInitilizer.Init());

        // Initialize the purchase manager
        yield return StartCoroutine(PurchaseManager.Instance.Init());

        // Initialize the game manager
        yield return StartCoroutine(GameManager.Instance.Init());

        // Initialize the notification manager
        yield return StartCoroutine(NotificationManager.Instance.Init());

        // Check and request notification permissions if not granted
        if (!notificationPermissions.IsPermissionGranted())
        {
            notificationPermissions.RequestNotificationPermission();
        }

        // If the main menu is set to load, load it asynchronously
        if (loadMainMenu)
        {
            Debug.Log("Starting Main Menu Scene Load!");

            // Begin loading the main menu scene
            var operation = SceneManager.LoadSceneAsync(GameManager.Instance.mainMenuScene);

            // Allow the scene to activate as soon as it is ready
            operation.allowSceneActivation = true;

            // Wait until the scene loading is complete
            while (!operation.isDone)
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
}