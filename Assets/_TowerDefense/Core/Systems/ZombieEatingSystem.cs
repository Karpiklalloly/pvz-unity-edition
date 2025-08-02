using DCFApixels.DragonECS;
using Karpik.Engine.Shared.DragonECS;
using Karpik.Engine.Shared.EcsRunners;
using UnityEngine;

namespace TowerDefense.Core
{
    public class ZombieEatingSystem : IEcsInject<EcsDefaultWorld>, IEcsInject<EcsEventWorld>, IEcsPausableRun
    {
        class Aspect : EcsAspect
        {
            public EcsPool<EatingState> eating = Inc;
            public EcsPool<EatingRate> rate = Inc;
            public EcsPool<Damage> damage = Inc;
            public EcsTagPool<IsDead> isDead = Exc;
        }
        
        private EcsDefaultWorld _world;
        private EcsEventWorld _eventWorld;

        public void PausableRun()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var eating = ref a.eating.Get(e);
                if (!eating.TargetPlant.IsAlive)
                {
                    a.eating.Del(e);
                    return;
                }
                var damage = a.damage.Get(e).Value;
                var rate = a.rate.Get(e).Rate;
                
                eating.Timer += GameTime.DeltaTime;
                if (rate < eating.Timer)
                {
                    eating.Timer -= rate;
                    _eventWorld.SendEvent(new DamageEvent()
                    {
                        Source = e,
                        Target = eating.TargetPlant.ID,
                        Damage = damage
                    });
                }
            }
        }

        public void Inject(EcsDefaultWorld obj)
        {
            _world = obj;
        }

        public void Inject(EcsEventWorld obj)
        {
            _eventWorld = obj;
        }
    }
}