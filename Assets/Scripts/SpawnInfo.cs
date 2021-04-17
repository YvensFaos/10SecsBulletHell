using System;

[Serializable]
public struct SpawnInfo
{
    public DefaultEnemyScript enemy;
    public int minAmount;
    public int maxAmount;
    public bool randomize;
}
