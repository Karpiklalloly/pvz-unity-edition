using DCFApixels.DragonECS;
using Karpik.Engine.Shared.DragonECS;

namespace TowerDefense.Core
{
    public class DamageSystem : IEcsInject<EcsDefaultWorld>, IEcsRunOnEvent<DamageEvent>
    {
        private EcsDefaultWorld _world;
        private EcsPool<Health> _healthPool;
        private EcsTagPool<IsDead> _deadPool;

        public void RunOnEvent(ref DamageEvent evt)
        {
            if (_healthPool.Has(evt.Target))
            {
                ref var health = ref _healthPool.Get(evt.Target);
                health.CurrentHeath -= evt.Damage;

                if (health.CurrentHeath <= 0)
                {
                    _deadPool.Add(evt.Target);
                }
            }
        }

        public void Inject(EcsDefaultWorld obj)
        {
            _world = obj;
            _healthPool = _world.GetPool<Health>();
            _deadPool = _world.GetPool<IsDead>();
        }
    }
}