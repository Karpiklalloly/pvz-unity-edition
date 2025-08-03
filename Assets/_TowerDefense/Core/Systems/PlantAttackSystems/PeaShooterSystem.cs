using DCFApixels.DragonECS;
using UnityEngine;

namespace TowerDefense.Core.PlantAttackSystems
{
    public class PeaShooterSystem : PlantSystem<PeaShooter>, IEcsInit
    {
        private ProjectileConfig _pea;
        public void Init()
        {
            _pea = _projectileDataCenter["Pea"];
        }
        
        protected override void Shoot(int damage, Vector3 position)
        {
            var (go, entity) = Spawner.Spawn(_pea.Prefab, _pea);
            go.transform.position = position;
            go.transform.forward = go.transform.right;
            entity.Add<Damage>().Value = damage;
            entity.Get<MovementDirection>().Direction = go.transform.forward;
        }


    }
}