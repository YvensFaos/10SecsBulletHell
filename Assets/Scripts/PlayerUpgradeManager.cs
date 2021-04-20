using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerUpgradeManager : MonoBehaviour
{
    [SerializeField] private UpgradeTreeSO upgradeTree;

    [SerializeField] private UpgradePanelController panelController;

    private HashSet<UpgradeInfo> _available;
    private List<UpgradeInfo> _unavailable;

    private float _inflation = 1.0f;

    [SerializeField] private float internalInflationIncrease = 0.5f;

    private void Awake()
    {
        _available = new HashSet<UpgradeInfo>();
        _unavailable = new List<UpgradeInfo>();

        var upgrades = upgradeTree.tree;
        upgrades.ForEach(info =>
        {
            if (info.requirements)
            {
                _unavailable.Add(info);
            }
            else
            {
                _available.Add(info);
            }
        });
    }

    public void Unlock(UpgradeInfo upgradeInfo)
    {
        _available.Remove(upgradeInfo);

        var player = GameLogic.GetInstance().Player;
        switch (upgradeInfo.type)
        {
            case UpgradeTypeEnum.INCREASE_HEALTH:
                player.IncreaseHealth((int) upgradeInfo.increment);
                break;
            case UpgradeTypeEnum.BULLET_SPEED:
                player.IncreaseBulletSpeed(upgradeInfo.increment);
                break;
            case UpgradeTypeEnum.BULLET_DAMAGE:
                player.IncreaseBulletDamage((int) upgradeInfo.increment);
                break;
            case UpgradeTypeEnum.EXTRA_GUN:
                player.UnlockNewGun();
                break;
            case UpgradeTypeEnum.SHIELD_UNLOCK:
                player.UnlockShield();
                break;
            case UpgradeTypeEnum.SHIELD_HEALTH:
                if (player.HasShieldUnlocked())
                {
                    player.GetShield().IncreaseShieldStrenght((int) upgradeInfo.increment);
                }

                break;
            case UpgradeTypeEnum.SHIELD_COOLDOWN:
                if (player.HasShieldUnlocked())
                {
                    player.GetShield().ReduceShieldCoolddown(upgradeInfo.increment);
                }

                break;
            case UpgradeTypeEnum.MOVEMENT_SPEED:
                player.IncreaseMovementSpeed(upgradeInfo.increment);
                break;
            case UpgradeTypeEnum.RECOVER_HEALTH:
                player.RecoverHealth();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _inflation += internalInflationIncrease;
        UnlockNewUpgrades(upgradeInfo.unlocks);
    }

    private void UnlockNewUpgrades(string[] unlock)
    {
        if (unlock != null && unlock.Length > 0)
        {
            foreach (string unlockUpgrade in unlock)
            {
                var exists = _unavailable.Find(info => info.id.Equals(unlockUpgrade));
                if (exists.id != "")
                {
                    _unavailable.Remove(exists);
                    if (!_available.Contains(exists))
                    {
                        _available.Add(exists);
                    }
                }
            }

            panelController.RefreshValues();
        }
    }

    public float GetInflation() => _inflation;

    public List<UpgradeInfo> GetAvailableUpgrades()
    {
        return _available.ToList();
    }
}