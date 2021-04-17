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
        gameObject.SetActive(false);
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
        if (other.CompareTag("PlayerBullet"))
        {
            TakeDamage();
            LeanPool.Despawn(other);
        }
    }
}
