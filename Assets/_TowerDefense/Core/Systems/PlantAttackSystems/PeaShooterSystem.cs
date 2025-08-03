using DCFApixels.DragonECS;
using Karpik.Engine.Shared.EcsRunners;
using UnityEngine;

namespace TowerDefense.Core.PlantAttackSystems
{
    public class PeaShooterSystem : IEcsInject<EcsDefaultWorld>, IEcsInject<ProjectileDataCenter>, IEcsPausableRun
    {
        class Aspect : EcsAspect
        {
            public EcsTagPool<PeaShooter> peaShooter = Inc;
            public EcsPool<AttackCooldown> attackCooldown = Inc;
            public EcsPool<TransformReference> transform = Inc;
            public EcsPool<Damage> damage = Inc;
            public EcsTagPool<IsDead> isDead = Exc;
        }
        
        private EcsDefaultWorld _world;
        private ProjectileDataCenter _projectileDataCenter;
        
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
                    Debug.Log("Shooting Pea");
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
        
        private void Shoot(int damage, Vector3 position)
        {
            var pea = _projectileDataCenter["Pea"];
            var (go, entity) = Spawner.Spawn(pea.Prefab, pea);
            go.transform.position = position;
            go.transform.forward = go.transform.right;
            entity.Add<Damage>().Value = damage;
            entity.Get<MovementDirection>().Direction = go.transform.forward;
        }
    }
}