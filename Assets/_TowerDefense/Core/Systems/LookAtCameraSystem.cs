using DCFApixels.DragonECS;
using Karpik.Engine.Shared.DragonECS;
using UnityEngine;

namespace TowerDefense.Core
{
    public class LookAtCameraSystem : IEcsLateRunProcess, IEcsInject<EcsDefaultWorld>, IEcsRunOnEvent<SceneLoadedEvent>
    {
        class Aspect : EcsAspect
        {
            public EcsPool<TransformReference> transform = Inc;
            public EcsTagPool<LookAtCamera> lookAtCamera = Inc;
        }
        
        private Camera _camera;
        private EcsDefaultWorld _world;
        
        public void LateRun()
        {
            foreach (var e in _world.Where(out Aspect aspect))
            {
                ref var transform = ref aspect.transform.Get(e);
                transform.Transform.forward = _camera.transform.forward;
            }
        }

        public void Inject(EcsDefaultWorld obj)
        {
            _world = obj;
        }

        public void RunOnEvent(ref SceneLoadedEvent evt)
        {
            _camera = Camera.main;
        }
    }
}