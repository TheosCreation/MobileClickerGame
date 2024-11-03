using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }

    private MainMenuMainPage mainPage;
    private LoadingPage loadingPage;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        mainPage = GetComponentInChildren<MainMenuMainPage>();
        loadingPage = GetComponentInChildren<LoadingPage>();
    }

    private void Start()
    {
        OpenMainPage();
    }

    public void OpenMainPage()
    {
        mainPage.gameObject.SetActive(true);
        loadingPage.gameObject.SetActive(false);
    }

    public void OpenLoadingPage()
    {
        loadingPage.gameObject.SetActive(true);
        mainPage.gameObject.SetActive(false);
    }
}