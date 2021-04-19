using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade Tree", menuName = "Upgrade Tree", order = 0)]
public class UpgradeTreeSO : ScriptableObject
{
    public List<UpgradeInfo> tree;
}