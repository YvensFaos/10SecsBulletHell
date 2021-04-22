using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

public class DropGameObjectWhenDead : MonoBehaviour
{
    [SerializeField] private List<GameObject> dropWhenDeadObjects;

    [SerializeField, Range(0.0f, 1.0f)] private float dropChance;

    [SerializeField] private bool dropIndividually;

    [SerializeField] private bool poolable;

    private void OnDisable()
    {
        if (!dropIndividually)
        {
            if (ShouldDrop())
            {
                dropWhenDeadObjects.ForEach(obj =>
                {
                    if (poolable)
                    {
                        LeanPool.Spawn(obj, transform.position, Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(obj,
                            transform.position, Quaternion.identity);
                    }
                });
            }
        }
        else
        {
            dropWhenDeadObjects.ForEach(obj =>
            {
                if (ShouldDrop())
                {
                    if (poolable)
                    {
                        LeanPool.Spawn(obj, transform.position, Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(obj,
                            transform.position, Quaternion.identity);
                    }
                }
            });
        }
    }

    private bool ShouldDrop() => Random.Range(0.0f, 1.0f) <= dropChance;
}