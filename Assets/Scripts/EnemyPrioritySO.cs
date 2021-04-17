using UnityEngine;

namespace UnityTemplateProjects
{
    [CreateAssetMenu(fileName = "EnemyPriority", menuName = "EnemyPriority", order = 0)]
    public class EnemyPrioritySO : ScriptableObject
    {
        public EnemyPriority priority;
        public bool alwaysUseCustomMechanic = false;
    }
}