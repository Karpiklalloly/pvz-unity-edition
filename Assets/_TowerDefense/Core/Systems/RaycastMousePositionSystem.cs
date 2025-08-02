using System.Buffers;
using System.Linq;
using DCFApixels.DragonECS;
using Karpik.Engine.Shared.DragonECS;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TowerDefense.Core
{
    public class RaycastMousePositionSystem : IEcsInject<EcsEventWorld>, IEcsInject<EcsDefaultWorld>, IEcsRun, IEcsRunOnEvent<SceneLoadedEvent>
    {
        private Camera _camera;
        private EcsEventWorld _eventWorld;
        private int _mask = LayerMask.GetMask(MaskConstants.PointerMask);
        private EcsDefaultWorld _world;
        private const float _maxDistance = 100f;

        public void Run()
        {
            if (_camera is null) return;
            
            var screenPosition = Pointer.current.position.ReadValue();
            Ray ray = _camera.ScreenPointToRay(screenPosition);
            int mask = _mask;
            ref var w = ref _world.Get<IgnorePointerMask>();
            if (w.Masks?.Count > 0)
            {
                var except = MaskConstants.PointerMask.Except(w.Masks);
                mask = LayerMask.GetMask(except.ToArray());
            }
            Physics.Raycast(ray, out var hit, _maxDistance, mask);
            if (hit.collider != null)
            {
                var go = hit.collider.gameObject;
                _eventWorld.SendEvent(new PointerIsAbove()
                {
                    Target = go.TryGetComponent<Provider>(out var provider) ? provider.Entity.ID : 0,
                    WorldPosition = hit.point,
                    Object = go
                });
            }
            else
            {
                _eventWorld.SendEvent(new PointerIsAbove());
            }
            
        }

        public void RunOnEvent(ref SceneLoadedEvent evt)
        {
            _camera = Camera.main;
        }

        public void Inject(EcsEventWorld obj)
        {
            _eventWorld = obj;
        }

        public void Inject(EcsDefaultWorld obj)
        {
            _world = obj;
        }
    }
}