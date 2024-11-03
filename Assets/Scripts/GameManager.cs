using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [HideInInspector] public GameState GameState;
    private double m_CurrentMoney;
    public string mainMenuScene = "MainMenu";

    public double CurrentMoney
    {
        get
        {
            return m_CurrentMoney;
        }
        set
        {
            m_CurrentMoney = value;
            UiManager.Instance.UpdateCurrentMoneyText(value);
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            GameState = new GameState();
        }
        else
        {
            Destroy(gameObject);
        }

        //UnSerializeGameStateFromJson();
    }

    public IEnumerator Init()
    {
        //load from save files


        yield return null;
    }

    public void LoadGame()
    {
        Debug.Log("Loading Game Scene... ");
        MainMenuManager.Instance.OpenLoadingPage();
        SceneManager.LoadSceneAsync("GameScene");
    }

    public void LoadMainMenu()
    {
        Debug.Log("Loading Main Menu... ");
        SceneManager.LoadScene(mainMenuScene);
    }
    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
