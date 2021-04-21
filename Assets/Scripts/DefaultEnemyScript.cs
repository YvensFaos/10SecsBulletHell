using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Lean.Pool;
using UnityEngine;
using UnityTemplateProjects;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Collider))]
public class DefaultEnemyScript : MonoBehaviour
{
    [Header("Enemy Info")] [SerializeField]
    private int health = 3;

    private int _currentHealth;

    [SerializeField] private float actTimer = 1.0f;
    private int _movementDirection;

    [Header("Enemy Configuration")] [SerializeField]
    private AnimationCurve movementCurve;

    [SerializeField] private float movementIntensity;
    [SerializeField] private float movementOnY;
    private float _internalCurvePosition;
    private Vector3 _minPosition;
    private Vector3 _maxPosition;

    [SerializeField] private BulletBehavior defaultBullet;
    [SerializeField] private List<GunPosition> gunPlacements;

    [SerializeField] private AttackScript attackScript;
    [SerializeField] private List<CustomMechanicScript> customMechanicScripts;
    [SerializeField] private ShieldBehavior shieldBehavior;

    private bool _hasShield;
    private bool _dead;

    [Header("Enemy Priorities")] [SerializeField]
    private EnemyPrioritySO priorities;

    [Header("Enemy Particles")] [SerializeField]
    private ParticleSystem damageParticles;

    [SerializeField] private ParticleSystem destructionParticles;

    [Header("Enemy Sounds")] [SerializeField]
    private AudioSource attackSound;

    [SerializeField] private AudioSource hitSound;
    [SerializeField] private AudioSource destructionSound;


    /// <summary>
    /// Used to calculated the added priority values and facilitate the querying for actions.
    /// </summary>
    private EnemyPriority _internalPriorities;

    private float _enemyPrioritySum;
    private IEnumerator _logicCoroutine;
    private PlayerBehavior _player;
    private TweenerCore<Vector3, Vector3, VectorOptions> _internalTween;

    private void Start()
    {
        _player = GameLogic.GetInstance().Player;
        _hasShield = shieldBehavior != null;
        if (_hasShield)
        {
            shieldBehavior.gameObject.SetActive(false);
        }

        _movementDirection = 1;

        CalculatePrioritySum();
        var levelManager = GameLogic.GetInstance().Level;
        _minPosition = levelManager.MinPosition;
        _maxPosition = levelManager.MaxPosition;
        Initiate();
    }

    private void OnEnable()
    {
        _dead = false;
    }

    /// <summary>
    /// Should be called whenever the enemy is reinserted into the scene.
    /// </summary>
    public void Initiate(bool flipDirection = false)
    {
        _internalCurvePosition = 0.0f;
        _logicCoroutine = EnemyLogic();
        _movementDirection = 1;
        _currentHealth = health;
        if (flipDirection)
        {
            _movementDirection = -1;
        }

        StartCoroutine(_logicCoroutine);
    }

    private IEnumerator EnemyLogic()
    {
        void ExecuteAllCustomScripts()
        {
            customMechanicScripts.ForEach(customMechanicScript => customMechanicScript.PerformCustomMechanic(this));
        }

        while (_currentHealth > 0)
        {
            if (priorities.alwaysUseCustomMechanic)
            {
                ExecuteAllCustomScripts();
            }

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
                    if (_hasShield)
                    {
                        shieldBehavior.gameObject.SetActive(true);
                        shieldBehavior.TurnShieldOn();
                    }

                    break;
                case EnemyPriorityEnum.Moving:
                    var position = transform.position;
                    _internalCurvePosition += movementIntensity;
                    var newPosition = _movementDirection * movementCurve.Evaluate(_internalCurvePosition);
                    var moveTo = new Vector3(position.x + newPosition, position.y - movementOnY);
                    moveTo.x = Mathf.Clamp(moveTo.x, _minPosition.x, _maxPosition.x);
                    _internalTween = transform.DOMove(moveTo, actTimer).OnComplete(() =>
                    {
                        if (transform.position.y < GameLogic.GetInstance().Level.LimitY)
                        {
                            Die();
                        }
                    });
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

            yield return new WaitForSeconds(actTimer);
        }
    }

    /// <summary>
    /// Invoked when there is not specific AttackScript
    /// </summary>
    public void PerformSimpleAttack()
    {
        gunPlacements.ForEach(placement =>
        {
            var bullet = LeanPool.Spawn(defaultBullet, placement.transform.position, Quaternion.identity,
                GameLogic.GetInstance().EnemyBulletsTransform());
            bullet.SetDirection(placement.GetDirection());
            attackSound.Play();
        });
    }

    public void TakeDamage()
    {
        if (!IsShieldActivate())
        {
            _currentHealth -= _player.GetPlayerBulletDamage();
            if (damageParticles != null)
            {
                var particles = LeanPool.Spawn(damageParticles, transform.position, Quaternion.identity,
                    GameLogic.GetInstance().DamageParticlesTransform());
                particles.Play();
                LeanPool.Despawn(particles, 2.0f);

                GameLogic.GetInstance().CameraShake(0.4f, 1.0f, 1.0f, Vector3.zero);
                hitSound.Play();
            }

            if (_currentHealth <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        _internalTween.Kill();
        StopCoroutine(_logicCoroutine);
        destructionSound.Play();
        LeanPool.Despawn(this, 0.8f);

        var particles = LeanPool.Spawn(destructionParticles, transform.position, Quaternion.identity,
            GameLogic.GetInstance().DestructionParticlesTransform());
        particles.Play();
        LeanPool.Despawn(particles, 2.0f);

        _dead = true;
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
        if (!_dead && other.gameObject.CompareTag("PlayerBullet"))
        {
            TakeDamage();
        }
    }

    private bool IsShieldActivate() => _hasShield && shieldBehavior.IsShieldOn();
}