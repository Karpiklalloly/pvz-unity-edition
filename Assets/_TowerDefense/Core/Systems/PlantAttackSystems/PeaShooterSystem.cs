using DCFApixels.DragonECS;
using Karpik.Engine.Shared.EcsRunners;
using UnityEngine;

namespace TowerDefense.Core.PlantAttackSystems
{
    public class PeaShooterSystem : PlantSystem<PeaShooter>
    {
        protected override void Shoot(int damage, Vector3 position)
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