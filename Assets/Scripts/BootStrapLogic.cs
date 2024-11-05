using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapLogic : MonoBehaviour
{
    [SerializeField] private UnityServicesInit unityServicesInitilizer;
    [SerializeField] private PlayGamesAuth playGamesAuth;
    [SerializeField] private AdsInit adsInit;
    [SerializeField] private AdManager adManager;

    private void Start()
    {
        StartCoroutine(Validate());
    }

    private IEnumerator Validate()
    {
        yield return null;

        adsInit.Init();

        yield return StartCoroutine(playGamesAuth.Init());

        yield return StartCoroutine(unityServicesInitilizer.Init());

        yield return StartCoroutine(PurchaseManager.Instance.Init());

        yield return StartCoroutine(GameManager.Instance.Init());

        adManager.Init();

        Debug.Log("Starting Main Menu Scene Load!");

        //var operation = SceneManager.LoadSceneAsync(GameManager.Instance.mainMenuScene);
        //// Tell unity to activate the scene soon as its ready
        //operation.allowSceneActivation = true;
        //
        //// While the main menu scene is loading update the progress 
        //while (!operation.isDone)
        //{
        //    yield return new WaitForEndOfFrame();
        //}
    }
}
