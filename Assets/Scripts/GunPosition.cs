using UnityEngine;

public class GunPosition : MonoBehaviour
{
    [SerializeField]
    private Transform aimPosition;
    [SerializeField]
    private bool available;

    public void TurnOn()
    {
        available = true;
    }
    
    public Vector3 GetDirection()
    {
        var direction = aimPosition.position - transform.position;
        direction.Normalize();
        return direction;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, aimPosition.position);
        Gizmos.DrawWireCube(aimPosition.position, new Vector3(0.3f, 0.3f, 0.3f));
    }

    public bool Available() => available;
}
