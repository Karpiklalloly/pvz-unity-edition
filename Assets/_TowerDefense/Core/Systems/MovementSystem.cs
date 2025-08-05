using DCFApixels.DragonECS;
using Karpik.Engine.Shared.EcsRunners;
using UnityEngine;

namespace TowerDefense.Core
{
    public class MovementSystem : IEcsInject<EcsDefaultWorld>, IEcsPausableRun
    {
        class TransformAspect : EcsAspect
        {
            public EcsPool<TransformReference> transform = Inc;
            public EcsPool<MovementDirection> direction = Inc;
            public EcsPool<MoveSpeed> speed = Inc;
            public EcsTagPool<IsDead> isDead = Exc;
            public EcsPool<EatingState> eating = Exc;
            public EcsPool<RigidBodyReference> rigidBody = Exc;
        }
        
        class RigidBodyAspect : EcsAspect
        {
            public EcsPool<RigidBodyReference> rigidBody = Inc;
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
            foreach (var e in _world.Where(out TransformAspect a))
            {
                ref var transform = ref a.transform.Get(e);
                ref var direction = ref a.direction.Get(e);
                transform.Transform.position += GameTime.DeltaTime * a.speed.Get(e).Speed * direction.Direction ;
            }
            
            foreach (var e in _world.Where(out RigidBodyAspect a))
            {
                ref var transform = ref a.rigidBody.Get(e);
                ref var direction = ref a.direction.Get(e);
                var pos = transform.RigidBody.position;
                transform.RigidBody.MovePosition(pos + GameTime.DeltaTime * a.speed.Get(e).Speed * direction.Direction);
            }
        }
    }
}