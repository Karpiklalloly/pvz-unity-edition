using DCFApixels.DragonECS;
using Karpik.Engine.Shared.DragonECS;
using Karpik.Engine.Shared.EcsRunners;
using UnityEngine;

namespace TowerDefense.Core
{
    public class SunSpawnSystem : IEcsPausableRun, IEcsRunOnEvent<SceneLoadedEvent>, IEcsInject<EcsDefaultWorld>
    {
        private float _timer;
        private EcsDefaultWorld _world;
        private const float SunSpawnInterval = 2f;
        
        public void PausableRun()
        {
            _timer += GameTime.DeltaTime;
            if (_timer > SunSpawnInterval)
            {
                ref var config = ref _world.Get<CurrentLevel>().Congig;
                SpawnSun(config.DefaultSun);
                _timer = 0;
            }
        }

        public void RunOnEvent(ref SceneLoadedEvent evt)
        {
            if (evt.LevelConfig != default)
            {
                _timer = 0;
            }
        }

        public void Inject(EcsDefaultWorld obj)
        {
            _world = obj;
        }

        private void SpawnSun(SunConfig config)
        {
            var (go, entity) = Spawner.Spawn(config.Prefab, config);
            var camera = Camera.main;
            
            var x = Random.Range(0.1f, 0.9f);
            go.transform.position = camera.GetCameraPosition(new Vector2(x, 1.1f));
            entity.MoveTo(camera.GetCameraPosition(new Vector2(x, Random.Range(0.1f, 0.9f))));
        }
    }
}