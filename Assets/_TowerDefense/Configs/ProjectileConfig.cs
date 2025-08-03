using System.Collections.Generic;
using DCFApixels.DragonECS;
using TowerDefense.Core;
using UnityEngine;

namespace TowerDefense
{
    [CreateAssetMenu(fileName = "Projectile", menuName = "TowerDefence/Projectile Config", order = -201)]
    public class ProjectileConfig : RequiredEntityTemplate
    {
        public GameObject Prefab;
        public string Name;
        
        private IComponentTemplate[] _requiredComponents =
        {
            new ProjectileTemplate(),
            new TransformReferenceTemplate(),
            new MoveSpeedTemplate()
        };
        
        protected override IEnumerable<IComponentTemplate> GetRequiredComponents()
        {
            return _requiredComponents;
        }
    }
}