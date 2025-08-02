using System.Collections.Generic;
using System.Linq;
using DCFApixels.DragonECS;
using Karpik.Engine.Shared.DragonECS;
using Karpik.Engine.Shared.EcsRunners;
using UnityEngine;
using Random = System.Random;

namespace TowerDefense.Core
{
    public class ZombieSpawnSystem : IEcsPausableRun, IEcsInject<EcsDefaultWorld>, IEcsRunOnEvent<SceneLoadedEvent>
    {
        private EcsDefaultWorld _world;
        private Random _random = new();
        private float _timer;
        private EcsPool<MovementDirection> _movementDirectioPool;

        public void PausableRun()
        {
            ref var config = ref _world.Get<CurrentLevel>().Congig;
            if (config == null) return;
            ref var playerData = ref _world.Get<PlayerData>();
            if (playerData.CurrentWaveIndex == -1 || playerData.CurrentWaveIndex >= config.Waves.Count) return;

            _timer += GameTime.DeltaTime;
            if (_timer >= config.Waves[playerData.CurrentWaveIndex].DelayBeforeStart)
            {
                var zombies = GetZombies(config.AvailableZombies, config.Waves[playerData.CurrentWaveIndex].Budget);
                foreach (var zombie in zombies)
                {
                    SpawnZombie(zombie);
                }

                playerData.CurrentWaveIndex++;
                _timer = 0;
            }
            // TODO: Спавнить, если все прошлые зомби умерли

        }

        public void Inject(EcsDefaultWorld obj)
        {
            _world = obj;
            _movementDirectioPool = _world.GetPool<MovementDirection>();
        }
        
        private IEnumerable<ZombieConfig> GetZombies(IEnumerable<ZombieConfig> available, int budget)
        {
            var zombies = new List<ZombieConfig>();
            while (budget > 0)
            {
                var zombie = available.ElementAt(_random.Next(available.Count()));
                var cost = zombie.Get<ZombieCost>().Value;
                if (cost <= budget)
                {
                    zombies.Add(zombie);
                    budget -= cost;
                }
            }
            return zombies;
        }

        public void RunOnEvent(ref SceneLoadedEvent evt)
        {
            if (evt.LevelConfig != default)
            {
                _world.Get<PlayerData>().CurrentWaveIndex = 0;
                _timer = 0;
            }
        }
        
        private void SpawnZombie(ZombieConfig zombieConfig)
        {
            var (go, entity) = Spawner.Spawn(zombieConfig.Prefab, zombieConfig);
            // TODO: спавнить на разных линиях
            go.transform.position = GetSpawnPosition(ref _world.Get<CurrentLevel>());
            entity.Get<MoveSpeed>().Speed *= ((float)_random.NextDouble() * 0.2f + 0.9f);
            go.transform.forward = -go.transform.right;
            _movementDirectioPool.TryAddOrGet(entity.ID).Direction = go.transform.forward;
        }

        private Vector3 GetSpawnPosition(ref CurrentLevel level)
        {
            var rowsCount = (int)level.Congig.Grid.GridSize.y;
            var row = _random.Next(rowsCount);
            return new Vector3(
                level.Congig.Grid.OriginPosition.x + level.Congig.Grid.GridSize.x * level.Congig.Grid.CellSize.x + 1,
                0,
                level.Congig.Grid.OriginPosition.y + row * level.Congig.Grid.CellSize.y);
            
        }
    }
}