using System;
using TMPro;
using UnityEngine;

public class UpgradeButtonLogic : MonoBehaviour
{
    [SerializeField] private UpgradeInfo upgradeInfo;

    [SerializeField] private TextMeshProUGUI upgradeText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private bool repeatable;

    private UpgradePanelController _controller;
    private int _currentCost;

    public void Initiate(UpgradeInfo newUpgradeInfo, UpgradePanelController controller)
    {
        upgradeInfo = newUpgradeInfo;
        upgradeText.text = GetUpgradeTextGivenType(upgradeInfo);
        _controller = controller;
        UpgradeCost();
    }

    public void ClickMe()
    {
        if (GameLogic.GetInstance().TryToSpendXpPoints(_currentCost))
        {
            if (!repeatable)
            {
                _controller.RemoveMe(this);
                Destroy(gameObject);    
            }
            GameLogic.GetInstance().UpgradeManager().Unlock(upgradeInfo);
        }
        else
        {
            //Buzz sound
        }
    }

    private static string GetUpgradeTextGivenType(UpgradeInfo upgradeInfo)
    {
        switch (upgradeInfo.type)
        {
            case UpgradeTypeEnum.INCREASE_HEALTH:
                return "Increase Health";
            case UpgradeTypeEnum.BULLET_SPEED:
                return "Bullet Speed";
            case UpgradeTypeEnum.BULLET_DAMAGE:
                return "Bullet Damage";
            case UpgradeTypeEnum.EXTRA_GUN:
                return "Adds 1 Extra Gun";
            case UpgradeTypeEnum.SHIELD_UNLOCK:
                return "Unlocks Shield";
            case UpgradeTypeEnum.SHIELD_HEALTH:
                return "Increase Shield Strength";
            case UpgradeTypeEnum.SHIELD_COOLDOWN:
                return "Reduces Shield's Cooldown";
            case UpgradeTypeEnum.MOVEMENT_SPEED:
                return "Increase Movement Speed";
            case UpgradeTypeEnum.RECOVER_HEALTH:
                return "Recovers Health Entirely";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void UpgradeCost()
    {
        _currentCost = Mathf.CeilToInt(upgradeInfo.cost * GameLogic.GetInstance().UpgradeManager().GetInflation());
        costText.text = _currentCost.ToString();
    }
    
    public UpgradeInfo GetUpgradeInfo() => upgradeInfo;
}