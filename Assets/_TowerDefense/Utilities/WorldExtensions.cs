using DCFApixels.DragonECS;
using TowerDefense.Core;

namespace TowerDefense
{
    public static class WorldExtensions
    {
        public static void Clear(this EcsWorld world, bool withGO = false)
        {
            var entities = world.Entities;
            foreach (var entity in entities)
            {
                world.Get<LevelFlow>().CurrentWaveIndex = 0;
                world.Get<LevelFlow>().Waves = null;
                world.Get<CurrentLevel>().Congig = null;
                world.Get<PlayerData>().CurrentWaveIndex = -1;
                world.Get<PlayerData>().SunAmount = 0;
                world.Get<IgnorePointerMask>().Masks.Clear();
                if (withGO)
                {
                    Spawner.Destroy(entity);
                }
                else
                {
                    world.DelEntity(entity);
                }
            }
        }
    }
}