using System.Collections.Generic;
using System.Linq;
using DCFApixels.DragonECS;
using Karpik.Engine.Shared.DragonECS;
using Karpik.Engine.Shared.EcsRunners;
using UnityEngine;
using Random = System.Random;

namespace TowerDefense.Core
{
    public class ZombieSpawnSystem : IEcsPausableRun, IEcsInject<EcsDefaultWorld>, IEcsInject<EcsEventWorld>, IEcsRunOnEvent<SceneLoadedEvent>
    {
        private EcsDefaultWorld _world;
        private EcsEventWorld _eventWorld;
        private Random _random = new();
        private float _timer;
        private EcsPool<MovementDirection> _movementDirectionPool;

        public void PausableRun()
        {
            ref var config = ref _world.Get<CurrentLevel>().Congig;
            if (config == null) return;
            ref var playerData = ref _world.Get<PlayerData>();
            if (playerData.CurrentWaveIndex == -1 || playerData.CurrentWaveIndex >= config.Waves.Count) return;
            ref var flow = ref _world.Get<LevelFlow>();
            _timer += GameTime.DeltaTime;
            if (_timer >= config.Waves[playerData.CurrentWaveIndex].DelayBeforeStart
                || (flow.Waves.TryGetValue(playerData.CurrentWaveIndex - 1, out var zz) && zz.Count == 0))
            {
                
                if (flow.Waves == null) flow.Waves = new Dictionary<int, List<int>>();
                flow.Waves.TryAdd(playerData.CurrentWaveIndex, new List<int>());
                var z = flow.Waves[playerData.CurrentWaveIndex];
                var zombies = GetZombies(config.AvailableZombies, config.Waves[playerData.CurrentWaveIndex].Budget);
                foreach (var zombie in zombies)
                {
                    var entity = SpawnZombie(zombie);
                    z.Add(entity);
                }
                
                _eventWorld.SendEvent(new NewWaveEvent()
                {
                    WaveIndex = playerData.CurrentWaveIndex,
                    LastIndex = config.Waves.Count - 1
                });
                flow.CurrentWaveIndex = playerData.CurrentWaveIndex;
                playerData.CurrentWaveIndex++;
                _timer = 0;
            }
            // TODO: Спавнить, если все прошлые зомби умерли (ПРОВЕРИТЬ В ДЕЙСТВИИ)

        }

        public void Inject(EcsDefaultWorld obj)
        {
            _world = obj;
            _movementDirectionPool = _world.GetPool<MovementDirection>();
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
        
        private int SpawnZombie(ZombieConfig zombieConfig)
        {
            var (go, entity) = Spawner.Spawn(zombieConfig.Prefab, zombieConfig);
            go.transform.position = GetSpawnPosition(ref _world.Get<CurrentLevel>());
            entity.Get<MoveSpeed>().Speed *= ((float)_random.NextDouble() * 0.2f + 0.9f);
            go.transform.forward = -go.transform.right;
            _movementDirectionPool.TryAddOrGet(entity.ID).Direction = go.transform.forward;
            return entity.ID;
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

        public void Inject(EcsEventWorld obj)
        {
            _eventWorld = obj;
        }
    }
}