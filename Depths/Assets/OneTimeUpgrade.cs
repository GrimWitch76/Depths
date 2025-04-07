using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OneTimeUpgrade : MonoBehaviour
{
    [SerializeField] private OneTimeUpgradeType _upgradeType;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private TextMeshProUGUI _upgradeText;
    [SerializeField] private Image _checkMark;
    [SerializeField] private Sprite _checkMarkSprite;
    [SerializeField] private AudioSource _purchaseSfx;

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
            WorldStateManager.Instance.DrillShip.DeductMoney(_upgradeCost);
            _checkMark.sprite = _checkMarkSprite;
            _purchaseSfx.Play();
            _upgradeButton.interactable = false;
        }
    }

    public void UpdateUI()
    {
        _upgradeButton.interactable = !_purchased && WorldStateManager.Instance.DrillShip.Money >= _upgradeCost;
        
        if(_purchased)
        {
            _upgradeText.text = "Purchased";
        }
    }
}
