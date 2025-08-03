using DCFApixels.DragonECS;
using Karpik.Engine.Shared.EcsRunners;
using UnityEngine;

namespace TowerDefense.Core.PlantAttackSystems
{
    public abstract class PlantSystem<T> : IEcsInject<EcsDefaultWorld>, IEcsInject<ProjectileDataCenter>, IEcsPausableRun
        where T : struct, IEcsTagComponent
    {
        protected class Aspect : EcsAspect
        {
            public EcsTagPool<T> peaShooter = Inc;
            public EcsPool<AttackCooldown> attackCooldown = Inc;
            public EcsPool<TransformReference> transform = Inc;
            public EcsPool<Damage> damage = Inc;
            public EcsTagPool<IsDead> isDead = Exc;
        }
        
        protected EcsDefaultWorld _world;
        protected ProjectileDataCenter _projectileDataCenter;
        
        public void PausableRun()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                var transform = a.transform.Get(e).Transform;
                ref var cooldown = ref a.attackCooldown.Get(e);
                cooldown.Timer += GameTime.DeltaTime;
                Debug.DrawRay(transform.position, transform.forward * 10, Color.red);
                if (cooldown.Timer >= cooldown.Rate)
                {
                    Shoot(a.damage.Get(e).Value, transform.position);
                    cooldown.Timer = 0;
                }
            }
        }

        public void Inject(EcsDefaultWorld obj)
        {
            _world = obj;
        }

        public void Inject(ProjectileDataCenter obj)
        {
            _projectileDataCenter = obj;
        }

        protected abstract void Shoot(int damage, Vector3 position);
    }
}