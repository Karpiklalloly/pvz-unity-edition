using DCFApixels.DragonECS;
using TowerDefense.Core;
using UnityEngine;

namespace TowerDefense
{
    public static class MovementUtils
    {
        public static float MoveTo(this entlong entity, Vector3 targetPosition)
        {
            var world = Bootstrap.Instance.World;
            var transformPool = world.GetPool<TransformReference>();
            var movementPool = world.GetPool<MovementDirection>();
            var speedPool = world.GetPool<MoveSpeed>();
            if (transformPool.Has(entity.ID))
            {
                var entityPosition = transformPool.Get(entity.ID).Transform.position;
                var direction = targetPosition - entityPosition;
                float speed = speedPool.Has(entity.ID) ? speedPool.Get(entity.ID).Speed : 1f;
                var time = direction.magnitude / speed;
                movementPool.TryAddOrGet(entity.ID).Direction = direction.normalized;
                world.GetPool<StopAfterSeconds>().TryAddOrGet(entity.ID).Seconds = time;
                return time;
            }

            return 0;
        }
    }
}