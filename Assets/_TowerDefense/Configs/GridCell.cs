using System.Collections.Generic;
using DCFApixels.DragonECS;
using TowerDefense.Core;
using UnityEngine;

namespace TowerDefense
{
    [CreateAssetMenu(fileName = "GridCell", menuName = "TowerDefence/GridCell", order = 0)]
    public class GridCell : RequiredEntityTemplate
    {
        public GameObject Prefab;
        private static IComponentTemplate[] _templates = {
            new TransformReferenceTemplate(),
            new GridCellTemplate(),
            new GridCoordinatesTemplate(),
            new CellColorTemplate()
        };
        
        protected override IEnumerable<IComponentTemplate> GetRequiredComponents()
        {
            return _templates;
        }
    }
}