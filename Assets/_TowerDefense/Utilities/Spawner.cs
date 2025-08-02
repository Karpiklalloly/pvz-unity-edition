using DCFApixels.DragonECS;
using TowerDefense.Core;
using UnityEngine;

namespace TowerDefense
{
    public static class Spawner
    {
        public static GameObject Spawn(GameObject gameObject)
        {
            return Object.Instantiate(gameObject);
        }

        public static (GameObject, entlong) Spawn(GameObject gameObject, params IEntityTemplate[] templates)
        {
            var o = Spawn(gameObject);
            var world = Bootstrap.Instance.World;
            var transformPool = world.GetPool<TransformReference>();
            var entity = world.NewEntityLong();
            foreach (var template in templates)
            {
                template.Apply(world.ID, entity.ID);
            }
            
            transformPool.TryAddOrGet(entity.ID).Transform = o.transform;
            
            if (o.TryGetComponent<Renderer>(out var renderer))
            {
                world.GetPool<RendererReference>().TryAddOrGet(entity.ID).Renderer = renderer;
            }

            o.AddComponent<Provider>().Entity = entity;
            return (o, entity);
        }
        
        public static void Destroy(GameObject gameObject)
        {
            if (gameObject == null) return;
            Object.Destroy(gameObject);
        }

        public static void Destroy(int entity)
        {
            var world = Bootstrap.Instance.World;
            var transformPool = world.GetPool<TransformReference>();
            if (transformPool.Has(entity)) Destroy(transformPool.Get(entity).Transform.gameObject);
            world.DelEntity(entity);
        }
    }
}