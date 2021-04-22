using System.Collections.Generic;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
   [SerializeField]
   private List<SpawnEnemiesSO> spawnEnemiesSOs;

   [SerializeField]
   private Transform minTransform;
   private Vector3 _minPosition;
   public Vector3 MinPosition => _minPosition;
   
   [SerializeField]
   private Transform maxTransform;
   private Vector3 _maxPosition;
   
   public Vector3 MaxPosition => _maxPosition;

   [SerializeField]
   private Transform limitTransform;
   private float _limitPosition;

   public float LimitY => _limitPosition;
   private float _middlePointX;
   private int _spawnEnemiesIndex;

   [SerializeField] private DefaultEnemyScript finalBoss;
   [SerializeField] private Transform finalBossSpawnPoint;
   [SerializeField] private Transform finalBossFightPoint;

   private void Awake()
   {
      _minPosition = minTransform.position;
      _maxPosition = maxTransform.position;
      _limitPosition = limitTransform.position.y;
      _middlePointX = (_minPosition.x + _maxPosition.x) / 2.0f;
   }

   public bool SpawnMore(bool shouldProgress = false)
   {
      if (shouldProgress)
      {
         _spawnEnemiesIndex++;
         _spawnEnemiesIndex = Mathf.Clamp(_spawnEnemiesIndex, 0, spawnEnemiesSOs.Count - 1);
      }

      var spawnEnemy = spawnEnemiesSOs[_spawnEnemiesIndex];
      SpawnListOfEnemies(spawnEnemy.enemies);
      if (spawnEnemy.enemyPositionPrefab != null)
      {
         LeanPool.Spawn(spawnEnemy.enemyPositionPrefab);
      }
      
      GameLogic.GetInstance().XpPointsLabel().UpdateValues();
      return MaxLevelReached();
   }
   
   private void SpawnListOfEnemies(List<SpawnInfo> spawnInfos)
   {
      spawnInfos.ForEach(spawnInfo =>
      {
         var count = spawnInfo.minAmount;
         if (spawnInfo.randomize)
         {
            count = Random.Range(spawnInfo.minAmount, spawnInfo.maxAmount + 1);
         }

         for (int i = 0; i < count; i++)
         {
            var position = new Vector3(Random.Range(_minPosition.x, _maxPosition.x), Random.Range(_minPosition.y, _maxPosition.y), Random.Range(_minPosition.z, _maxPosition.z));
            var enemy = LeanPool.Spawn(spawnInfo.enemy, position, Quaternion.identity);
            enemy.Initiate(position.x < _middlePointX);            
         }
      });
   }

   public void SpawnFinalBoss()
   {
      var spawnedEnemy = LeanPool.Spawn(finalBoss, finalBossSpawnPoint.position, Quaternion.identity);
      spawnedEnemy.transform.DOMove(finalBossFightPoint.position, 6.0f).OnComplete(() => spawnedEnemy.Initiate());
   }
   
   private void OnDrawGizmos()
   {
      if (minTransform != null && maxTransform != null)
      {
         Gizmos.DrawLine(minTransform.position, maxTransform.position);   
      }

      if (limitTransform != null)
      {
         var vecLine = limitTransform.position;
         Gizmos.color = Color.red;
         Gizmos.DrawLine(new Vector3(vecLine.x - 4.0f, vecLine.y, vecLine.z), new Vector3(vecLine.x + 4.0f, vecLine.y, vecLine.z));
      }
   }

   public int GetCurrentLevel() => _spawnEnemiesIndex + 1;

   public bool MaxLevelReached() => _spawnEnemiesIndex >= spawnEnemiesSOs.Count - 1;
}
