using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum UpgradeType
{
    ManualClick,

}


public class UpgradeButton : MonoBehaviour
{
    private Button button;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private UpgradeType upgradeType = UpgradeType.ManualClick;
    [SerializeField] private double cost = 1000;
    [SerializeField] private float difficultyScale = 1.2f;

    private void Awake()
    {
        button = GetComponent<Button>();
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
            GameManager.Instance.Upgrade(upgradeType);

            cost *= difficultyScale;
            UpdateCostText();
        }
    }

    private void UpdateCostText()
    {
        costText.text = "$" + NumberFormatter.FormatLargeNumber(cost);
    }
}
