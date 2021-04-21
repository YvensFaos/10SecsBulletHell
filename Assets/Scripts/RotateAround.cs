using UnityEngine;

public class RotateAround : MonoBehaviour
{
    [SerializeField]
    private Vector3 rotateAxis;
    [SerializeField]
    private float speed;

    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    private void Update()
    {
        _transform.Rotate(rotateAxis, speed);
    }
}
