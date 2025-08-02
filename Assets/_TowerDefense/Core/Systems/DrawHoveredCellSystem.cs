using DCFApixels.DragonECS;
using Karpik.Engine.Shared.DragonECS;
using Karpik.Engine.Shared.EcsRunners;
using UnityEngine;

namespace TowerDefense.Core
{
    public class DrawHoveredCellSystem : IEcsInject<EcsDefaultWorld>, IEcsRunOnEvent<PointerIsAbove>
    {
        private EcsDefaultWorld _world;
        private int _last = 0;
        private EcsPool<CellColor> _cellColorPool;
        private EcsPool<RendererReference> _rendererPool;
        private EcsPool<GridCell> _gridCellPool;

        public void RunOnEvent(ref PointerIsAbove evt)
        {
            if (evt.Target == 0)
            {
                if (_last == evt.Target) return;
                _rendererPool.Get(_last).Renderer.material.color = _cellColorPool.Get(_last).Color;
                _last = evt.Target;
                return;
            }
            
            var isCell = _gridCellPool.Has(evt.Target);
            if (!isCell) return;
            if (_last == evt.Target) return;
            
            if (_last != 0)
            {
                _rendererPool.Get(_last).Renderer.material.color = _cellColorPool.Get(_last).Color;
            }

            var color = _cellColorPool.Get(evt.Target);
            _rendererPool.Get(evt.Target).Renderer.material.color = color.HoverColor;
            _last = evt.Target;
        }

        public void Inject(EcsDefaultWorld obj)
        {
            _world = obj;
            _cellColorPool = _world.GetPool<CellColor>();
            _rendererPool = _world.GetPool<RendererReference>();
            _gridCellPool = _world.GetPool<GridCell>();
        }
    }
}