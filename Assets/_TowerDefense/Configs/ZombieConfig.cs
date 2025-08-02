using System;
using System.Collections.Generic;
using DCFApixels.DragonECS;
using TowerDefense.Core;
using UnityEngine;

namespace TowerDefense
{
    [CreateAssetMenu(fileName = "Zombie", menuName = "TowerDefence/Zombie", order = -201)]
    public class ZombieConfig : RequiredEntityTemplate
    {
        public GameObject Prefab;
        private static IComponentTemplate[] _templates = {
            new TransformReferenceTemplate(),
            new HealthTemplate(),
            new MoveSpeedTemplate(),
            new DamageTemplate(),
            new ZombieTemplate(),
            new AttackRangeTemplate(),
            new ZombieCostTemplate(),
            new EatingRateTemplate()
        };

        public Transform Transform => Get<TransformReference>().Transform;
        public Health Health => Get<Health>();
        public MoveSpeed MoveSpeed => Get<MoveSpeed>();
        public Damage Damage => Get<Damage>();
        public AttackRange AttackRange => Get<AttackRange>();
        public ZombieCost ZombieCost => Get<ZombieCost>();
        
        protected override IEnumerable<IComponentTemplate> GetRequiredComponents()
        {
            return _templates;
        }
    }
}