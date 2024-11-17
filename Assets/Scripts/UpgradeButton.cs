using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum UpgradeType
{
    ManualClick,

}


public class UpgradeButton : MonoBehaviour
{
    public string buttonID;
    private Button button;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private UpgradeType upgradeType = UpgradeType.ManualClick;
    public double cost = 1000;
    [SerializeField] private float difficultyScale = 1.2f;
    [SerializeField] private float upgradeAmmount = 0.08f;

    private void Awake()
    {
        button = GetComponent<Button>();
        LoadData();
    }

    private void Start()
    {
        UpdateCostText();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(Upgrade);   
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }

    private void Upgrade()
    {
        if(GameManager.Instance.CurrentMoney > cost)
        {
            GameManager.Instance.CurrentMoney -= cost;
            GameManager.Instance.Upgrade(upgradeType, upgradeAmmount);

            cost *= difficultyScale;
            UpdateCostText();
        }
    }

    private void UpdateCostText()
    {
        costText.text = "$" + NumberFormatter.FormatLargeNumber(cost);
    }

    public void SaveData()
    {
        var data = new ManualClickerUpgradeSaveData
        {
            UpgradeCost = cost
        };
        GameManager.Instance.GameState.SaveUpgradeButtonData(buttonID, data);
    }

    private void LoadData()
    {
        if (GameManager.Instance.GameState.TryGetUpgradeButtonData(buttonID, out ManualClickerUpgradeSaveData data))
        {
            cost = data.UpgradeCost;
        }
    }
}
