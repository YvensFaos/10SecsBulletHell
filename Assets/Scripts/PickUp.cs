using Lean.Pool;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private int xp;
    [SerializeField] private AudioSource pickUpSound;

    private bool _collected;

    private void OnEnable()
    {
        _collected = false;
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
        if (!_collected)
        {
            if (health > 0)
            {
                GameLogic.GetInstance().Player.RecoverHealth(health);
            }

            if (xp > 0)
            {
                GameLogic.GetInstance().GainXp(xp);
            }
            if (pickUpSound != null)
            {
                pickUpSound.Play();
            }
            _collected = true;
            LeanPool.Despawn(gameObject, 0.1f);
        }
    }
}