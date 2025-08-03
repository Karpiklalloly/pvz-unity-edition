using UnityEngine;

namespace TowerDefense.Core.PlantAttackSystems
{
    public class SunflowerSystem : PlantSystem<Sunflower>
    {
        protected override void Shoot(int damage, Vector3 position)
        {
            ref var config = ref _world.Get<CurrentLevel>().Congig;
            SpawnSun(config.DefaultSun, position);
        }
        
        private void SpawnSun(SunConfig config, Vector3 flowerPosition)
        {
            var (go, entity) = Spawner.Spawn(config.Prefab, config);
            var camera = Camera.main;
            
            var inCameraPosition = camera.WorldToViewportPoint(flowerPosition);
            go.transform.position = camera.GetCameraPosition(inCameraPosition);
        }
    }
}