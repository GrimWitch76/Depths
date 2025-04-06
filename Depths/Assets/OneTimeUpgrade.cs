using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OneTimeUpgrade : MonoBehaviour
{
    [SerializeField] private OneTimeUpgradeType _upgradeType;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private TextMeshProUGUI _upgradeText;
    [SerializeField] private Transform _checkMark;
    [SerializeField] private int _upgradeCost;

    private bool _purchased;

    public void Start()
    {
        UpdateUI();
    }

    public void PurchaseUpgrade()
    {
        if (WorldStateManager.Instance.DrillShip.Money >= _upgradeCost)
        {
            WorldStateManager.Instance.DrillShip.ApplyOneTimeUpgrade(_upgradeType);
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        _checkMark.gameObject.SetActive(_purchased);
        _upgradeButton.interactable = !_purchased && WorldStateManager.Instance.DrillShip.Money >= _upgradeCost;
        
        if(_purchased)
        {
            _upgradeText.text = "Purchased";
        }
    }
}
