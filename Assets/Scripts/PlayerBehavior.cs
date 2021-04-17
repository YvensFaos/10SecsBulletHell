using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class PlayerBehavior : MonoBehaviour
{
    [Header("Player Info")] [SerializeField]
    private int health = 1;

    [SerializeField] private float velocity = 1;

    [Header("Player Configuration")]
    [SerializeField] private BulletBehavior defaultBullet;
    [SerializeField] private List<Transform> gunPlacement;

    [SerializeField] private AttackScript attackScript;
    [SerializeField] private List<CustomMechanicScript> customMechanicScripts;


    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (IsAlive())
        {
            var horizontal = Input.GetAxis("Horizontal"); 
            var vertical =Input.GetAxis("Vertical");
            
            var newMovement = new Vector3(horizontal * velocity, vertical * velocity, 0.0f);
            transform.Translate(newMovement);

            if (Input.GetButtonUp("Fire1"))
            {
                if (attackScript != null)
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
                ExecuteAllCustomScripts();
            }

            _rigidbody.velocity = Vector3.zero;
        }
    }
    
    private void ExecuteAllCustomScripts()
    {
        customMechanicScripts.ForEach(customMechanicScript => customMechanicScript.PerformCustomMechanic());
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

    public int GetPlayerBulletDamage()
    {
        return 1;
    }
    
    private void TakeDamage()
    {
        health -= 1;
        if (!IsAlive())
        {
            GameLogic.GetInstance().NotifyPlayerIsDead();
        }
    }

    public bool IsAlive() => health > 0;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("EnemyBullet") || other.gameObject.CompareTag("Enemy"))
        {
            TakeDamage();
        }
    }
}