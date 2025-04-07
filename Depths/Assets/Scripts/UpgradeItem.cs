using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItem : MonoBehaviour
{
    [SerializeField] UpgradeData _upgrade;
    [SerializeField] TextMeshProUGUI _upgradeText;
    [SerializeField] TextMeshProUGUI _upgradeButtonText;
    [SerializeField] Button _upgradeButton;

    private int _currentUpgradeLevel = 0;

    public void Start()
    {
        UpdateUI();
    }

    public void PurchaseUpgrade()
    {
        if(WorldStateManager.Instance.DrillShip.Money >= _upgrade.costPerLevel[_currentUpgradeLevel])
        {
            _currentUpgradeLevel++;
            WorldStateManager.Instance.DrillShip.ApplyUpgrade(_upgrade, _currentUpgradeLevel);
            UpdateUI();
            WorldStateManager.Instance.DrillShip.DeductMoney(_upgrade.costPerLevel[_currentUpgradeLevel]);
        }
    }

    public void UpdateUI()
    {
        if(_currentUpgradeLevel+1 != _upgrade.maxLevel)
        {
            _upgradeButton.interactable = WorldStateManager.Instance.DrillShip.Money >= _upgrade.costPerLevel[_currentUpgradeLevel];
            _upgradeButtonText.text = "Upgrade" + "\n" + "$" + _upgrade.costPerLevel[_currentUpgradeLevel];
        }
        else
        {
            _upgradeButtonText.text = "Max Upgrade";
            _upgradeButton.interactable = false;
        }

        _upgradeText.text = _upgrade.name + "\n" + "lvl: " + (_currentUpgradeLevel+1);
    }
}
