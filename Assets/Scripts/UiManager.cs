using System.Text;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; private set; }

    [SerializeField] private TMP_Text totalMoneyText;

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
    }

    private void Start()
    {
        UpdateCurrentMoneyText(GameManager.Instance.CurrentMoney);
    }


    public void UpdateCurrentMoneyText(double amount)
    {
        totalMoneyText.text = "$ " + NumberFormatter.FormatLargeNumber(amount);
    }

}