using System.Collections.Generic;
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
        }
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