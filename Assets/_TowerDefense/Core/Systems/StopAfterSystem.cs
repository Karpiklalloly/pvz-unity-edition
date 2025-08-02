using DCFApixels.DragonECS;
using Karpik.Engine.Shared.EcsRunners;

namespace TowerDefense.Core
{
    public class StopAfterSystem : IEcsPausableRun, IEcsInject<EcsDefaultWorld>
    {
        class Aspect : EcsAspect
        {
            public EcsPool<StopAfterSeconds> stopAfter = Inc;
            public EcsPool<MovementDirection> direction = Inc;
            public EcsPool<MoveSpeed> speed = Inc;
        }
        
        private EcsDefaultWorld _world;

        public void PausableRun()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var stopAfter = ref a.stopAfter.Get(e);
                stopAfter.Seconds -= GameTime.DeltaTime;
                if (stopAfter.Seconds <= 0)
                {
                    a.direction.Del(e);
                    a.stopAfter.Del(e);
                }
            }
        }

        public void Inject(EcsDefaultWorld obj)
        {
            _world = obj;
        }
    }
}