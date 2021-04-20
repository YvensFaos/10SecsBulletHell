using System;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class PlayerBehavior : MonoBehaviour
{
    [Header("Player Info")] 
    [SerializeField] private int health = 1;
    [SerializeField] private int bulletInitialDamage = 1;
    [SerializeField] private float velocity = 1;

    [Header("Player Configuration")]
    [SerializeField] private BulletBehavior defaultBullet;
    [SerializeField] private List<GunPosition> gunPlacements;

    [SerializeField] private AttackScript attackScript;
    private bool _hasAttackScript;
    [SerializeField] ShieldBehavior shieldBehavior;

    [Header("Player Particles")]
    [SerializeField] private ParticleSystem damageParticles;
    [SerializeField] private ParticleSystem destructionParticles;

    [Header("Player Controllers")] 
    [SerializeField] private ControlShaderGraphMaterial spriteRenderer;
    
    [Header("Player Sounds")] 
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private AudioSource hitSound;
    [SerializeField] private AudioSource destructionSound;
    [SerializeField] private AudioSource menuSound;
    
    private Rigidbody _rigidbody;
    private bool _controllable;
    
    //Ship Properties
    private int _currentHealth;
    private int _bulletDamage = 1;
    private float _bulletSpeed = 10;
    private int _extraGuns;
    private bool _shieldUnlocked;
    private float _movementSpeed;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _hasAttackScript = attackScript != null;

        _currentHealth = health;
        _movementSpeed = velocity;
        _bulletDamage = bulletInitialDamage;
        _extraGuns = 0;
    }

    private void Update()
    {
        if (IsAlive() && _controllable)
        {
            var horizontal = Input.GetAxis("Horizontal"); 
            var vertical =Input.GetAxis("Vertical");
            
            var newMovement = new Vector3(horizontal * _movementSpeed, vertical * _movementSpeed, 0.0f);
            transform.Translate(newMovement);

            if (Input.GetButtonUp("Fire1"))
            {
                if (_hasAttackScript)
                {
                    attackScript.PerformAttack();
                }
                else
                {
                    PerformSimpleAttack();
                }
            }

            if (Input.GetButtonUp("Fire2"))
            {
                var instance = GameLogic.GetInstance();
                instance.PauseGame();
                instance.OpenUpdateMenu();
                menuSound.Play();
            }

            _rigidbody.velocity = Vector3.zero;
        }
    }

    /// <summary>
    /// Invoked when there is not specific AttackScript
    /// </summary>
    private void PerformSimpleAttack()
    {
        gunPlacements.ForEach(placement =>
        {
            if (placement.Available())
            {
                var bullet = LeanPool.Spawn(defaultBullet, placement.transform.position, Quaternion.identity,
                    GameLogic.GetInstance().PlayerBulletsTransform());
                bullet.SetShootForce(_bulletSpeed);
                bullet.SetDirection(placement.GetDirection());
                attackSound.Play();
            }
        });
    }

    public int GetPlayerBulletDamage()
    {
        return _bulletDamage;
    }
    
    private void TakeDamage()
    {
        _currentHealth -= 1;
        if (!IsAlive())
        {
            GameLogic.GetInstance().NotifyPlayerIsDead();
            GameLogic.GetInstance().CameraShake(1.5f);
            spriteRenderer.SetMaterialValue("AlphaFactor", 1.0f);
            spriteRenderer.AnimateMaterialValue("AlphaFactor", 0.0f, 0.4f);
            destructionParticles.Play();
            _controllable = false;
            destructionSound.Play();
        }
        else
        {
            GameLogic.GetInstance().CameraShake(0.5f);
            if (HasShieldUnlocked())
            {
                shieldBehavior.gameObject.SetActive(false);
            }
            damageParticles.Play();
            hitSound.Play();
        }
    }

    public bool IsAlive() => _currentHealth > 0;
    
    private void OnTriggerEnter(Collider other)
    {
        var isEnemyBullet = other.gameObject.CompareTag("EnemyBullet");
        if (isEnemyBullet || other.gameObject.CompareTag("Enemy"))
        {
            if (IsAlive())
            {
                if(!IsShieldActive())
                {
                    TakeDamage();
                    if (isEnemyBullet)
                    {
                        LeanPool.Despawn(other.gameObject);
                    }
                }
                
            }
        }
    }

    public void AllowControl(bool control) => _controllable = control;

    public void RecoverHealth()
    {
        _currentHealth = health;
    }

    public void RebirthPlayer()
    {
        RecoverHealth();
        spriteRenderer.SetMaterialValue("AlphaFactor", 0.0f);
        spriteRenderer.AnimateMaterialValue("AlphaFactor", 1.0f, 0.4f);
        _controllable = true;
        if (HasShieldUnlocked())
        {
            shieldBehavior.TurnShieldOn();
        }
    }
    
    //Unlock Upgrades 
    public void IncreaseHealth(int increment)
    {
        health += increment;
        _currentHealth = Mathf.Clamp(_currentHealth + increment, 0, health);
    }

    public void IncreaseBulletDamage(int increment) => _bulletDamage += increment;
    public void IncreaseBulletSpeed(float increment) => _bulletSpeed += increment;
    public void IncreaseMovementSpeed(float increment) => _movementSpeed += increment;

    public void UnlockShield()
    {
        _shieldUnlocked = true;
        if (shieldBehavior != null)
        {
            shieldBehavior.gameObject.SetActive(true);    
        }
    }

    public void UnlockNewGun()
    {
        if (++_extraGuns < gunPlacements.Count)
        {
            gunPlacements[_extraGuns].TurnOn();
        }
    }

    public bool HasShieldUnlocked() => _shieldUnlocked && shieldBehavior != null;

    private bool IsShieldActive() => HasShieldUnlocked() && shieldBehavior.IsShieldOn();
    
    public ShieldBehavior GetShield() => shieldBehavior;

    public int GetCurrentHealth() => _currentHealth;
}