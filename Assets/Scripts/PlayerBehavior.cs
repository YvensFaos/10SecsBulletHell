using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{

    [SerializeField] private List<Transform> gunPlacement;
    
    public int GetPlayerBulletDamage()
    {
        return 1;
    }
}
