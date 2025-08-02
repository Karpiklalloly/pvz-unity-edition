using UnityEngine;

namespace TowerDefense
{
    [CreateAssetMenu(fileName = "Projectile", menuName = "Tower Defence/Projectile Config", order = 0)]
    public class ProjectileConfig : ScriptableObject
    {
        public GameObject Prefab;

        public float MoveSpeed;
        public int Damage;
    }
}