using System.Linq;
using DCFApixels.DragonECS;
using Karpik.Engine.Shared.DragonECS;
using Karpik.Engine.Shared.EcsRunners;
using UnityEngine;

namespace TowerDefense.Core
{
    public class WinLoseSystem : IEcsInject<EcsDefaultWorld>, IEcsInject<EcsEventWorld>, IEcsPausableRun, IEcsRunOnEvent<ZombieDiedEvent>
    {
        private EcsDefaultWorld _world;
        private EcsEventWorld _eventWorld;
        private EcsPool<TransformReference> _transformsPool;

        public void RunOnEvent(ref ZombieDiedEvent evt)
        {
            ref var levelFlow = ref _world.Get<LevelFlow>();
            var indexes = levelFlow.Waves.SelectMany(x => x.Value);
            if (!indexes.Any())
            {
                _eventWorld.SendEvent(new WinEvent());
                Debug.Log("WIN");
                return;
            }
        }

        public void Inject(EcsDefaultWorld obj)
        {
            _world = obj;
            _transformsPool = _world.GetPool<TransformReference>();
        }

        public void Inject(EcsEventWorld obj)
        {
            _eventWorld = obj;
        }

        public void PausableRun()
        {
            ref var levelFlow = ref _world.Get<LevelFlow>();
            if (levelFlow.Waves == null || levelFlow.Waves.Count == 0) return;
            var indexes = levelFlow.Waves.SelectMany(x => x.Value);
            if (!indexes.Any()) return;
            
            var most = indexes.Min(x => _transformsPool.Get(x).Transform.transform.position.x);
            ref var g = ref _world.Get<CurrentLevel>().Congig.Grid;
            var left = g.OriginPosition.x - g.CellSize.x / 2;
            if (most < left)
            {
                _eventWorld.SendEvent(new LoseEvent());
                Debug.Log("LOSE");
                return;
            }
        }
    }
}