using System;
using System.Text;

[Serializable]
public struct UpgradeInfo
{
    public bool requirements;
    public string id;
    public int cost;
    public UpgradeTypeEnum type;
    public float increment;
    public string[] unlocks;

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendFormat("[{0}] - {1} - cost: {2} | increment: {3}.", id, type.ToString(), cost.ToString(), increment);
        return sb.ToString();
    }
}