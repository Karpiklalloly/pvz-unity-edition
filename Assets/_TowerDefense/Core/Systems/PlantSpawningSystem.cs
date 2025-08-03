using DCFApixels.DragonECS;
using Karpik.Engine.Shared.DragonECS;
using TowerDefense.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace TowerDefense.Core
{
    public class PlantSpawningSystem : IEcsInject<EcsDefaultWorld>, IEcsRunOnEvent<CardClickedEvent>, IEcsRunOnEvent<PointerIsAbove>, IEcsRunOnEvent<ClickedEvent>
    {
        private EcsDefaultWorld _world;
        private EcsPool<RendererReference> _rendererPool;
        private GameObject _lastGO;
        private entlong _lastEntity = entlong.NULL;
        private GameUIController.PlantCard _lastCard;
        private EcsPool<GridCell> _cellPool;
        private EcsPool<TransformReference> _transformPool;

        public void RunOnEvent(ref CardClickedEvent evt)
        {
            if (_lastEntity != entlong.NULL)
            {
                Spawner.Destroy(_lastEntity.ID);
                _lastEntity = entlong.NULL;
                _lastGO = null;
                _lastCard = null;
                return;
            }
            (_lastGO, _lastEntity) = Spawner.Spawn(evt.Card.Config.Prefab, evt.Card.Config);
            _lastGO.transform.position = new Vector3(10, 10, 10);
            _lastGO.transform.forward = _lastGO.transform.right;
            _lastCard = evt.Card;
            var renderer = _rendererPool.Get(_lastEntity.ID).Renderer;
            var color = renderer.material.color;
            renderer.material.color = new Color(color.r, color.g, color.b, color.a / 2);
            ref var mask = ref _world.Get<IgnorePointerMask>();
            if (mask.Masks == null) mask.Masks = new ();
            if (mask.Masks.Contains(MaskConstants.Sun)) return;
            mask.Masks.Add(MaskConstants.Sun);
        }

        public void RunOnEvent(ref ClickedEvent evt)
        {
            if (_lastEntity == entlong.NULL) return;
            if (evt.Target == 0) return;
            
            if (_cellPool.Has(evt.Target))
            {
                _lastGO.transform.position = _transformPool.Get(evt.Target).Transform.position;
                _lastCard.CooldownTimer = _lastCard.Config.PlantCardConfig.Cooldown;
                _world.Get<PlayerData>().SunAmount -= _lastCard.Config.PlantCardConfig.Cost;
                ref var cell = ref _cellPool.Get(evt.Target);
                cell.EntityInside = _lastEntity;
                //TODO: Добавить скрещивание

                _lastGO = null;
                _lastCard = null;
                _lastEntity = entlong.NULL;
                ref var mask = ref _world.Get<IgnorePointerMask>();
                if (mask.Masks.Contains(MaskConstants.Sun)) mask.Masks.Remove(MaskConstants.Sun);
            }
        }
        
        public void RunOnEvent(ref PointerIsAbove evt)
        {
            if (_lastEntity == entlong.NULL) return;
            if (evt.Target == 0)
            {
                _lastGO.transform.position = new Vector3(10, 10, 10);
                return;
            }
            
            if (_cellPool.Has(evt.Target))
            {
                _lastGO.transform.position = _transformPool.Get(evt.Target).Transform.position;
            }
            else
            {
                _lastGO.transform.position = new Vector3(10, 10, 10);
            }
        }
        
        public void Inject(EcsDefaultWorld obj)
        {
            _world = obj;
            _rendererPool = _world.GetPool<RendererReference>();
            _cellPool = _world.GetPool<GridCell>();
            _transformPool = _world.GetPool<TransformReference>();
        }


    }
}