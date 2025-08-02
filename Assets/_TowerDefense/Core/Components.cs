using System;
using System.Collections.Generic;
using DCFApixels.DragonECS;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TowerDefense.Core
{
    [Serializable]
    public struct TransformReference : IEcsComponent
    {
        public Transform Transform;
    }

    [Serializable]
    public class TransformReferenceTemplate : IComponentTemplate
    {
        [SerializeField]
        protected TransformReference component;
        public Type Type => typeof(TransformReference);
        public bool IsUnique => true;

        public void Apply(short worldID, int entityID)
        {
            EcsWorld.GetPoolInstance<EcsPool<TransformReference>>(worldID).TryAddOrGet(entityID) = component;
        }
        public object GetRaw() => component;
        public void SetRaw(object raw) => component = (TransformReference)raw;
        public void OnGizmos(Transform transform, IComponentTemplate.GizmosMode mode) { }

        public void OnValidate(Object obj)
        {
            if (component.Transform != null) return;
            component.Transform = obj switch
            {
                GameObject go => go.transform,
                Component comp => comp.transform,
                _ => component.Transform
            };
        }
    }


    [Serializable]
    public struct RendererReference : IEcsComponent
    {
        public Renderer Renderer;
    }

    public class RendererReferenceTemplate : ComponentTemplate<RendererReference>
    {
    }
    [Serializable]
    public struct Health : IEcsComponent
    {
        public int CurrentHeath;
        public int MaxHealth;
    }
    public class HealthTemplate : ComponentTemplate<Health> {}

    [Serializable]
    public struct MoveSpeed : IEcsComponent
    {
        public float Speed;
    }
    public class MoveSpeedTemplate : ComponentTemplate<MoveSpeed> {}

    [Serializable]
    public struct Damage : IEcsComponent
    {
        public int Value;
    }
    public class DamageTemplate : ComponentTemplate<Damage> {}

    [Serializable]
    public struct Cost : IEcsComponent
    {
        public int Value;
    }

    [Serializable]
    public struct EatingRate : IEcsComponent
    {
        public float Rate;
    }

    public class EatingRateTemplate : ComponentTemplate<EatingRate>
    {
    }

    public class CostTemplate : ComponentTemplate<Cost>
    {
    }

    [Serializable]
    public struct Plant : IEcsTagComponent
    {
    }
    public class PlantTemplate : TagComponentTemplate<Plant> {}

    [Serializable]
    public struct Zombie : IEcsTagComponent
    {
    }
    public class ZombieTemplate : TagComponentTemplate<Zombie> {}

    [Serializable]
    public struct Projectile : IEcsTagComponent
    {
    }
    public class ProjectileTemplate : TagComponentTemplate<Projectile> {}

    [Serializable]
    public struct Sun : IEcsComponent
    {
        public int Amount;
    }
    public class SunTemplate : ComponentTemplate<Sun> {}

    [Serializable]
    public struct GridCell : IEcsComponent
    {
        public entlong EntityInside;
    }
    public class GridCellTemplate : ComponentTemplate<GridCell> {}

    [Serializable]
    public struct AttackCooldown : IEcsComponent
    {
        public float Rate;
        public float Timer;
    }
    public class AttackCooldownTemplate : ComponentTemplate<AttackCooldown> {}

    [Serializable]
    public struct AttackRange : IEcsComponent
    {
        public int Value;
    }
    public class AttackRangeTemplate : ComponentTemplate<AttackRange> {}

    [Serializable]
    public struct ProjectilePrefab : IEcsComponent
    {
        public entlong Value;
    }
    public class ProjectilePrefabTemplate : ComponentTemplate<ProjectilePrefab> {}

    [Serializable]
    public struct EatingState : IEcsComponent
    {
        public float EatingRate;
        public float Timer;
        public entlong TargetPlant;
    }
    public class EatingStateTemplate : ComponentTemplate<EatingState> {}
    
    [Serializable]
    public struct SunProducer : IEcsComponent
    {
        public float ProductionRate;
        public float Timer;
        public int SunAmount;
    }
    public class SunProducerTemplate : ComponentTemplate<SunProducer> {}
    
    [Serializable]
    public struct GridCoordinates : IEcsComponent
    {
        public Vector2 Position;
    }
    public class GridCoordinatesTemplate : ComponentTemplate<GridCoordinates> {}

    [Serializable]
    public struct IsDead : IEcsTagComponent
    {
    }
    public class IsDeadTemplate : TagComponentTemplate<IsDead> {}

    [Serializable]
    public struct Target : IEcsComponent
    {
        public entlong Value;
    }
    public class TargetTemplate : ComponentTemplate<Target> {}

    [Serializable]
    public struct CurrentLevel : IEcsComponent
    {
        public LevelConfig Congig;
    }
    public class CurrentLevelTemplate : ComponentTemplate<CurrentLevel> {}

    [Serializable]
    public struct ZombieCost : IEcsComponent
    {
        public int Value;
    }

    public class ZombieCostTemplate : ComponentTemplate<ZombieCost>
    {
    }

    [Serializable]
    public struct LookAtCamera : IEcsTagComponent
    {
    }

    public class LookAtCameraTemplate : TagComponentTemplate<LookAtCamera>
    {
    }

    [Serializable]
    public struct StopAfterSeconds : IEcsComponent
    {
        public float Seconds;
    }

    public class StopAfterSecondsTemplate : ComponentTemplate<StopAfterSeconds>
    {
    }

    [Serializable]
    public struct MovementDirection : IEcsComponent
    {
        public Vector3 Direction;
    }

    public class MovementDirectionTemplate : ComponentTemplate<MovementDirection>
    {
    }

    public struct PlayerData : IEcsComponent
    {
        public int SunAmount;
        public int CurrentWaveIndex;
    }

    [Serializable]
    public struct CellColor : IEcsComponent
    {
        public Color Color;
        public Color HoverColor;
        public Color SelectedColor;
    }

    public class CellColorTemplate : ComponentTemplate<CellColor>
    {
    }

    [Serializable]
    public struct IgnorePointerMask : IEcsComponent
    {
        public List<string> Masks;
    }
}