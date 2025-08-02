using DCFApixels.DragonECS;
using Karpik.Engine.Shared.EcsRunners;
using UnityEngine;

namespace TowerDefense.Core
{
    public class MovementSystem : IEcsInject<EcsDefaultWorld>, IEcsPausableRun
    {
        class Aspect : EcsAspect
        {
            public EcsPool<TransformReference> transform = Inc;
            public EcsPool<MovementDirection> direction = Inc;
            public EcsPool<MoveSpeed> speed = Inc;
            public EcsTagPool<IsDead> isDead = Exc;
            public EcsPool<EatingState> eating = Exc;
        }
        
        private EcsDefaultWorld _world;

        public void Inject(EcsDefaultWorld obj)
        {
            _world = obj;
        }

        public void PausableRun()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var transform = ref a.transform.Get(e);
                ref var direction = ref a.direction.Get(e);
                transform.Transform.position += direction.Direction * GameTime.DeltaTime * a.speed.Get(e).Speed;
            }
        }
    }
}