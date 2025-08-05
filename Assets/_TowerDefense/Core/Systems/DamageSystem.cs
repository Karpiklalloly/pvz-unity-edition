using DCFApixels.DragonECS;
using Karpik.Engine.Shared.DragonECS;

namespace TowerDefense.Core
{
    public class DamageSystem : IEcsInject<EcsDefaultWorld>, IEcsInject<EcsEventWorld>, IEcsRunOnEvent<DamageEvent>
    {
        private EcsDefaultWorld _world;
        private EcsPool<Health> _healthPool;
        private EcsTagPool<Zombie> _zombiePool;
        private EcsTagPool<IsDead> _deadPool;
        private EcsEventWorld _eventWorld;

        public void RunOnEvent(ref DamageEvent evt)
        {
            if (_healthPool.Has(evt.Target))
            {
                ref var health = ref _healthPool.Get(evt.Target);
                health.CurrentHeath -= evt.Damage;

                if (health.CurrentHeath <= 0)
                {
                    _deadPool.TryAdd(evt.Target);
                    if (_zombiePool.Has(evt.Target))
                    {
                        _eventWorld.SendEvent(new ZombieDiedEvent()
                        {
                            Target = evt.Target
                        });
                    }
                    ref var levelFlow = ref _world.Get<LevelFlow>();
                    for (int i = levelFlow.CurrentWaveIndex - 3; i <= levelFlow.CurrentWaveIndex; i++)
                    {
                        if (levelFlow.Waves.TryGetValue(i, out var zombies))
                        {
                            var index = zombies.IndexOf(evt.Target);
                            if (index != -1)
                            {
                                zombies.RemoveAt(index);
                            }
                        }
                    }
                }
            }
        }

        public void Inject(EcsDefaultWorld obj)
        {
            _world = obj;
            _healthPool = _world.GetPool<Health>();
            _deadPool = _world.GetPool<IsDead>();
            _zombiePool = _world.GetPool<Zombie>();
        }

        public void Inject(EcsEventWorld obj)
        {
            _eventWorld = obj;
        }
    }
}