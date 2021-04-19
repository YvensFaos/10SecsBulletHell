using System.Collections;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ShieldBehavior : MonoBehaviour
{
    [SerializeField]
    private Vector3 regularShieldSide;
    [SerializeField]
    private float shieldAnimationTimer;
    [SerializeField] 
    private int shieldStrength;
    
    private bool _shieldIsOn;
    private int _shieldCurrentStrength;
    
    [SerializeField]
    private bool rechargeable;
    [SerializeField]
    private float cooldown;

    private void Awake()
    {
        _shieldCurrentStrength = shieldStrength;
    }

    public void TurnShieldOn()
    {
        if (!_shieldIsOn)
        {
            _shieldCurrentStrength = shieldStrength;
            _shieldIsOn = true;
            transform.localScale = Vector3.zero;
            transform.DOScale(regularShieldSide, shieldAnimationTimer);
        }
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
    
    private void TakeDamage()
    {
        _shieldCurrentStrength -= GameLogic.GetInstance().Player.GetPlayerBulletDamage();
        if (_shieldCurrentStrength <= 0)
        {
            TurnShieldOff();
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
        
        if (other.CompareTag("PlayerBullet"))
        {
            TakeDamage();
            LeanPool.Despawn(other);
        }
    }

    public void IncreaseShieldStrenght(int increment) => shieldStrength += increment;
    public void ReduceShieldCoolddown(float decrement) => cooldown -= decrement;

}
