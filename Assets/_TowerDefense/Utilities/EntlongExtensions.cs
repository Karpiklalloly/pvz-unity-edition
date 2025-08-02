using DCFApixels.DragonECS;
using TowerDefense.Core;
using UnityEngine;

namespace TowerDefense
{
    public static class EntlongExtensions
    {
        public static Transform GetTransform(this entlong entity)
        {
            var pool = entity.World.GetPool<TransformReference>();
            if (pool.Has(entity.ID))
            {
                return pool.Get(entity.ID).Transform;
            }
            return null;
        }
    }
}