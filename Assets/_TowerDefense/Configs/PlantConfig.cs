using System;
using System.Collections.Generic;
using DCFApixels.DragonECS;
using TowerDefense.Core;
using UnityEngine;

namespace TowerDefense
{
    [CreateAssetMenu(fileName = "PlantConfig", menuName = "TowerDefence/Plant Config", order = 0)]
    public class PlantConfig : RequiredEntityTemplate
    {
        public GameObject Prefab;
        public PlantCardConfig PlantCardConfig;
        public string Name;
        
        private static IComponentTemplate[] _templates = {
            new TransformReferenceTemplate(),
            new PlantTemplate(),
            new HealthTemplate(),
            new AttackCooldownTemplate(),
            new DamageTemplate()
        };

        protected override IEnumerable<IComponentTemplate> GetRequiredComponents()
        {
            return _templates;
        }
    }
    
}