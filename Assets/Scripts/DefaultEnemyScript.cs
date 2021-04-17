using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;
using UnityTemplateProjects;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Collider))]
public class DefaultEnemyScript : MonoBehaviour
{
    [Header("Enemy Info")] [SerializeField]
    private int health = 3;

    [SerializeField] private float actTimer = 1.0f;

    [Header("Enemy Configuration")] 
    [SerializeField]
    private AnimationCurve movementCurve;

    [SerializeField] private float movementIntensity;
    private float _internalCurvePosition;

    [SerializeField] private BulletBehavior defaultBullet;
    [SerializeField] private List<Transform> gunPlacement;

    [SerializeField] private AttackScript attackScript;
    [SerializeField] private List<CustomMechanicScript> customMechanicScripts;


    [Header("Enemy Priorities")] [SerializeField]
    private EnemyPrioritySO priorities;

    /// <summary>
    /// Used to calculated the added priority values and facilitate the querying for actions.
    /// </summary>
    private EnemyPriority _internalPriorities;

    private float _enemyPrioritySum;
    private IEnumerator _logicCoroutine;
    private PlayerBehavior _player;

    void Start()
    {
        _player = GameLogic.GetInstance().Player;
        CalculatePrioritySum();
        Initiate();
    }

    /// <summary>
    /// Should be called whenever the enemy is reinserted into the scene.
    /// </summary>
    public void Initiate()
    {
        _internalCurvePosition = 0.0f;
        _logicCoroutine = EnemyLogic();
        StartCoroutine(_logicCoroutine);
    }

    private IEnumerator EnemyLogic()
    {
        void ExecuteAllCustomScripts()
        {
            customMechanicScripts.ForEach(customMechanicScript => customMechanicScript.PerformCustomMechanic());
        }

        while (health > 0)
        {
            if (priorities.alwaysUseCustomMechanic)
            {
                ExecuteAllCustomScripts();
            }
            else
            {
                var action = GetAction();
                switch (action)
                {
                    case EnemyPriorityEnum.Attack:
                        if (attackScript != null)
                        {
                            attackScript.PerformAttack();
                        }
                        else
                        {
                            PerformSimpleAttack();
                        }

                        break;
                    case EnemyPriorityEnum.Defend:
                        // ?
                        break;
                    case EnemyPriorityEnum.Moving:
                        _internalCurvePosition += movementIntensity;
                        var newPosition = movementCurve.Evaluate(_internalCurvePosition);
                        transform.DOMove(new Vector3(newPosition, transform.position.y - _internalCurvePosition), actTimer);
                        break;
                    case EnemyPriorityEnum.Waiting:
                        // Does nothing
                        break;
                    case EnemyPriorityEnum.Custom:
                        if (!priorities.alwaysUseCustomMechanic)
                        {
                            ExecuteAllCustomScripts();    
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            yield return new WaitForSeconds(actTimer);
        }
    }

    /// <summary>
    /// Invoked when there is not specific AttackScript
    /// </summary>
    private void PerformSimpleAttack()
    {
        gunPlacement.ForEach(gunPlacementTransform =>
        {
            LeanPool.Spawn(defaultBullet, gunPlacementTransform.transform.position, Quaternion.identity);
        });
    }

    public void TakeDamage()
    {
        health -= _player.GetPlayerBulletDamage();
        if (health <= 0)
        {
            StopCoroutine(_logicCoroutine);
            LeanPool.Despawn(this);
        }
    }

    private EnemyPriorityEnum GetAction()
    {
        var randomValue = Random.Range(0.0f, _enemyPrioritySum);
        if (randomValue < _internalPriorities.aggresivity)
        {
            return EnemyPriorityEnum.Attack;
        }

        if (randomValue < _internalPriorities.defensive)
        {
            return EnemyPriorityEnum.Defend;
        }

        if (randomValue < _internalPriorities.moving)
        {
            return EnemyPriorityEnum.Moving;
        }

        if (randomValue < _internalPriorities.waiting)
        {
            return EnemyPriorityEnum.Waiting;
        }

        if (randomValue <= _internalPriorities.customMechanic)
        {
            return EnemyPriorityEnum.Custom;
        }

        return EnemyPriorityEnum.Waiting;
    }

    /// <summary>
    /// Calculate the sum of all priorities.
    /// </summary>
    /// <returns></returns>
    private void CalculatePrioritySum()
    {
        var sum = 0.0f;
        sum += priorities.priority.aggresivity;
        _internalPriorities.aggresivity = sum;

        sum += priorities.priority.defensive;
        _internalPriorities.defensive = sum;

        sum += priorities.priority.moving;
        _internalPriorities.moving = sum;

        sum += priorities.priority.waiting;
        _internalPriorities.waiting = sum;

        sum += priorities.priority.customMechanic;
        _internalPriorities.customMechanic = sum;

        _enemyPrioritySum = sum;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerBullet"))
        {
            TakeDamage();
        }
    }
}