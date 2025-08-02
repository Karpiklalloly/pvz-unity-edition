using System.Collections.Generic;
using DCFApixels.DragonECS;
using TowerDefense.Core;
using UnityEngine;

namespace TowerDefense
{
    [CreateAssetMenu(fileName = "Sun Config", menuName = "TowerDefence/Sun Config", order = 0)]
    public class SunConfig : RequiredEntityTemplate
    {
        public GameObject Prefab;
        private static IComponentTemplate[] _templates = new IComponentTemplate[]
        {
            new TransformReferenceTemplate(),
            new MoveSpeedTemplate(),
            new SunTemplate(),
            new LookAtCameraTemplate(),
            new StopAfterSecondsTemplate()
        };

        protected override IEnumerable<IComponentTemplate> GetRequiredComponents()
        {
            return _templates;
        }
    }
}