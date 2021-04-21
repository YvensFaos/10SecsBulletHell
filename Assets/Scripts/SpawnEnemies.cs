using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

public class SpawnEnemies : CustomMechanicScript
{
    [SerializeField] private DefaultEnemyScript enemyToSpawn;
    [SerializeField] private List<Transform> spawnPosition;
    
    public override void PerformCustomMechanic(DefaultEnemyScript enemy)
    {
        spawnPosition.ForEach(spawnTransform =>
            {
                var spawnedEnemy = LeanPool.Spawn(enemyToSpawn, spawnTransform.position, Quaternion.identity);
                spawnedEnemy.Initiate(false);
            }
        );
    }
}
