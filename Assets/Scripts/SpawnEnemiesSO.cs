using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spawn Enemies", menuName = "Spawn Enemies", order = 0)]
public class SpawnEnemiesSO : ScriptableObject
{
    public List<SpawnInfo> enemies;
    [SerializeField]
    public GameObject enemyPositionPrefab;
}
