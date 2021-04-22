using System;
using System.Collections;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ShieldBehavior : MonoBehaviour
{
    [SerializeField] private Vector3 regularShieldSize;
    [SerializeField] private float shieldAnimationTimer;
    [SerializeField] private int shieldStrength;
    [SerializeField] private bool playerShield;

    private bool _shieldIsOn;
    private int _shieldCurrentStrength;
    private IEnumerator _rechargeRoutine;

    [SerializeField] private bool rechargeable;
    [SerializeField] private float cooldown;
    [SerializeField] private AudioSource shieldSound;

    private void Awake()
    {
        _shieldCurrentStrength = shieldStrength;
    }

    private void OnEnable()
    {
        TurnShieldOn();
    }

    public void TurnShieldOn()
    {
        if (!_shieldIsOn)
        {
            _shieldCurrentStrength = shieldStrength;
            _shieldIsOn = true;
            transform.localScale = Vector3.zero;
            transform.DOScale(regularShieldSize, shieldAnimationTimer);

            if (playerShield)
            {
                SetupRechargingCoroutine();
            }
        }
    }

    private void SetupRechargingCoroutine()
    {
        if (_rechargeRoutine != null)
        {
            StopCoroutine(_rechargeRoutine);
        }

        _rechargeRoutine = RechargeCouroutine();
        StartCoroutine(_rechargeRoutine);
    }

    private void TurnShieldOff()
    {
        _shieldIsOn = false;
        transform.localScale = Vector3.zero;
        if (rechargeable)
        {
            StartCoroutine(ShieldCooldown());
        }
    }

    private IEnumerator ShieldCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        TurnShieldOn();
    }

    public void TakeDamage(bool fromEnemy = false)
    {
        var damage = (fromEnemy) ? 1 : GameLogic.GetInstance().Player.GetPlayerBulletDamage();
        _shieldCurrentStrength -= damage;

        if (shieldSound != null)
        {
            shieldSound.Play();
        }

        if (_shieldCurrentStrength <= 0)
        {
            TurnShieldOff();
        }
    }

    private IEnumerator RechargeCouroutine()
    {
        while (true)
        {
            yield return new WaitUntil(() => _shieldCurrentStrength < shieldStrength);
            yield return new WaitForSeconds(cooldown);
            _shieldCurrentStrength = Mathf.Clamp(_shieldCurrentStrength + 1, 0, shieldStrength);
            if (shieldSound != null)
            {
                shieldSound.Play();
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        ResolveCollision(other.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        ResolveCollision(other.gameObject);
    }

    private void ResolveCollision(GameObject other)
    {
        if (!_shieldIsOn)
        {
            return;
        }

        bool fromEnemy = other.CompareTag("EnemyBullet");
        bool damage = (playerShield) ? fromEnemy : other.CompareTag("PlayerBullet");
        if (damage)
        {
            LeanPool.Despawn(other);
            TakeDamage(fromEnemy);
        }
    }

    public void IncreaseShieldStrenght(int increment) => shieldStrength += increment;
    public void ReduceShieldCoolddown(float decrement)
    {
        cooldown -= decrement;
        SetupRechargingCoroutine();
    }

    public bool IsShieldOn() => _shieldIsOn;

    public int GetCurrentStrength() => _shieldCurrentStrength;
}