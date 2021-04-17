using System.Collections.Generic;
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

   private void Awake()
   {
      _minPosition = minTransform.position;
      _maxPosition = maxTransform.position;
      _limitPosition = limitTransform.position.y;
      _middlePointX = (_minPosition.x + _maxPosition.x) / 2.0f;
   }

   public void SpawnMore(bool shouldProgress)
   {
      if (shouldProgress)
      {
         _spawnEnemiesIndex++;
         _spawnEnemiesIndex = Mathf.Clamp(_spawnEnemiesIndex, 0, spawnEnemiesSOs.Count);
      }

      var spawnEnemy = spawnEnemiesSOs[_spawnEnemiesIndex];
      SpawnListOfEnemies(spawnEnemy.enemies);
      if (spawnEnemy.enemyPositionPrefab != null)
      {
         LeanPool.Spawn(spawnEnemy.enemyPositionPrefab);
      }
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
            Vector3 position = new Vector3(Random.Range(_minPosition.x, _maxPosition.x), Random.Range(_minPosition.y, _maxPosition.y), Random.Range(_minPosition.z, _maxPosition.z));
            var enemy = LeanPool.Spawn(spawnInfo.enemy, position, Quaternion.identity);
            enemy.Initiate(position.x < _middlePointX);            
         }
      });
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
}
