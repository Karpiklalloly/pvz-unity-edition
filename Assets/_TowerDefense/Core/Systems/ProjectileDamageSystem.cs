using DCFApixels.DragonECS;
using Karpik.Engine.Shared.DragonECS;

namespace TowerDefense.Core
{
    public class ProjectileDamageSystem : IEcsInject<EcsDefaultWorld>, IEcsInject<EcsEventWorld>, IEcsRunOnEvent<CollisionEvent>
    {
        private EcsDefaultWorld _world;
        private EcsTagPool<Zombie> _zombiePool;
        private EcsTagPool<Projectile> _projectilePool;
        private EcsPool<Damage> _damagePool;
        private EcsTagPool<IsDead> _deadPool;
        private EcsEventWorld _eventWorld;

        public void RunOnEvent(ref CollisionEvent evt)
        {
            if (evt.Source == 0 || evt.Target == 0) return;

            if (!_zombiePool.Has(evt.Target) || !_projectilePool.Has(evt.Source)) return;
            if (_deadPool.Has(evt.Target)) return;

            _eventWorld.SendEvent(new DamageEvent()
            {
                Source = evt.Source,
                Target = evt.Target,
                Damage = _damagePool.Get(evt.Source).Value
            });
            
            Spawner.Destroy(evt.Source);
        }

        public void Inject(EcsDefaultWorld obj)
        {
            _world = obj;
            _zombiePool = _world.GetPool<Zombie>();
            _projectilePool = _world.GetPool<Projectile>();
            _damagePool = _world.GetPool<Damage>();
            _deadPool = _world.GetPool<IsDead>();
        }

        public void Inject(EcsEventWorld obj)
        {
            _eventWorld = obj;
        }
    }
}