using System;
using DCFApixels.DragonECS;
using Karpik.Engine.Shared.DragonECS;
using TowerDefense.Core;
using UnityEngine;

namespace TowerDefense
{
    public class CollisionDetector : MonoBehaviour
    {
        private EcsDefaultWorld _world;
        private EcsEventWorld _eventWorld;

        private void Awake()
        {
            _world = EcsDefaultWorldSingletonProvider.Instance.Get();
            _eventWorld = EcsEventWorldSingletonProvider.Instance.Get();
        }

        private void OnTriggerEnter(Collider other)
        {
            _eventWorld.SendEvent(new CollisionEvent
            {
                Source = TryGetComponent<Provider>(out var provider1) ? provider1.Entity.ID : 0,
                Target = other.TryGetComponent<Provider>(out var provider2) ? provider2.Entity.ID : 0
            });
        }
    }
}