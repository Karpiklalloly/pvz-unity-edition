using DCFApixels.DragonECS;
using Karpik.Engine.Shared.DragonECS;

namespace TowerDefense.Core
{
    public class TargetingSystem : IEcsInject<EcsDefaultWorld>, IEcsRunOnEvent<CollisionEvent>
    {
        private EcsDefaultWorld _world;
        private EcsPool<EatingState> _eatingStatePool;
        private EcsTagPool<Zombie> _zombiePool;
        private EcsTagPool<Plant> _plantPool;
        private EcsPool<EatingRate> _eatingRatePool;

        public void RunOnEvent(ref CollisionEvent evt)
        {
            if (evt.Source == 0 || evt.Target == 0) return;

            if (!_zombiePool.Has(evt.Source) || !_plantPool.Has(evt.Target)) return;
            
            if (!_eatingStatePool.Has(evt.Source))
            {
                _eatingStatePool.Add(evt.Source) = new EatingState()
                {
                    TargetPlant = _world.GetEntityLong(evt.Target),
                    Timer = 0
                };
            }
        }

        public void Inject(EcsDefaultWorld obj)
        {
            _world = obj;
            _eatingStatePool = _world.GetPool<EatingState>();
            _zombiePool = _world.GetPool<Zombie>();
            _plantPool = _world.GetPool<Plant>();
            _eatingRatePool = _world.GetPool<EatingRate>();
        }
    }
}