using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoveRandomlyMechanic : CustomMechanicScript
{
    private Vector3 _previousLocation;
    private bool _cameBack;
    
    private void Awake()
    {
        _previousLocation = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0.0f);
    }

    public override void PerformCustomMechanic(DefaultEnemyScript enemy)
    {
        if (!_cameBack)
        {
            _previousLocation.x = Random.Range(-0.5f, 0.5f);
            _previousLocation.y = Random.Range(-0.5f, 0.5f);
            _cameBack = true;
        }
        else
        {
            _previousLocation.x *= -1.0f;
            _previousLocation.y *= -1.0f;
            _cameBack = false;
        }
        enemy.transform.DOMove(_previousLocation, 1.0f);
        
    }
}
