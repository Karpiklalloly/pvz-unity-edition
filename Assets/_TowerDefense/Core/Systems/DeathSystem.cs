using DCFApixels.DragonECS;
using Karpik.Engine.Shared.EcsRunners;

namespace TowerDefense.Core
{
    public class DeathSystem : IEcsInject<EcsDefaultWorld>, IEcsPausableRun
    {
        private EcsDefaultWorld _world;

        class Aspect : EcsAspect
        {
            public EcsTagPool<IsDead> isDead = Inc;
        }

        public void PausableRun()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                Spawner.Destroy(e);
            }
        }

        public void Inject(EcsDefaultWorld obj)
        {
            _world = obj;
        }
    }
}