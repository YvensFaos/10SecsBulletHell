using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradePanelController : MonoBehaviour
{
    [SerializeField]
    private List<UpgradeButtonLogic> buttons;
    
    [SerializeField]
    private UpgradeButtonLogic buttonPrefab;

    [SerializeField]
    private Transform buttonsParent;
    
    private void Awake()
    {
        buttons = GetComponentsInChildren<UpgradeButtonLogic>(true).ToList();
    }

    private void OnEnable()
    {
        RefreshValues();
    }

    public void RefreshValues()
    {
        var available = GameLogic.GetInstance().UpgradeManager().GetAvailableUpgrades();

        var mustCreate = available.FindAll(info =>
        {
            var containsUpgrade = buttons.Find(button => button.GetUpgradeInfo().id.Equals(info.id));
            return containsUpgrade == null;
        });
        
        mustCreate.ForEach(info =>
        {
            var newButton = Instantiate(buttonPrefab, buttonsParent);
            newButton.Initiate(info, this);
            buttons.Add(newButton);
        });
        RefreshCostValues();
    }

    public void RemoveMe(UpgradeButtonLogic button)
    {
        buttons.Remove(button);
    }

    public void RefreshCostValues()
    {
        buttons.ForEach(logic => logic.UpgradeCost());
    }
}
