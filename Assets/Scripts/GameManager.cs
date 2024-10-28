using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [HideInInspector] public GameState GameState;
    private double m_CurrentMoney;

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
}
