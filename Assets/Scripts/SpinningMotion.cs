using DG.Tweening;
using UnityEngine;

public class SpinningMotion : CustomMechanicScript
{
    [SerializeField] private float rotationPerCall = 10.0f;

    private float _currentRotation;

    public override void PerformCustomMechanic(DefaultEnemyScript enemy)
    {
        _currentRotation += rotationPerCall;
        transform.DOLocalRotate(new Vector3(0.0f, 0.0f, _currentRotation), 0.1f)
            .OnComplete(enemy.PerformSimpleAttack);
    }
}