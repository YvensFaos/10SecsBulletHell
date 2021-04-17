using System;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletBehavior : MonoBehaviour
{
    private Rigidbody _rigidbody;

    [SerializeField]
    private Vector2 direction;
    [SerializeField]
    private float shootForce;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Impulse();
    }

    private void OnEnable()
    {
        Impulse();
    }

    private void OnDisable()
    {
        _rigidbody.velocity = Vector3.zero;
    }

    private void Impulse()
    {
        _rigidbody.AddForce(direction * shootForce);
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection;
    }

    private void OnDrawGizmos()
    {
        var position = transform.position;
        Gizmos.DrawLine(position, position + new Vector3(direction.x, direction.y, 0.0f) * shootForce);
    }
}
