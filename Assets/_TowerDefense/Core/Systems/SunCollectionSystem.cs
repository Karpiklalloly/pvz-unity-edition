using DCFApixels.DragonECS;
using Karpik.Engine.Shared.DragonECS;
using UnityEngine;

namespace TowerDefense.Core
{
    public class SunCollectionSystem : IEcsInject<EcsDefaultWorld>, IEcsRunOnEvent<ClickedEvent>
    {
        private EcsDefaultWorld _world;
        private readonly int _layer = LayerMask.NameToLayer(MaskConstants.Default);
        private EcsPool<MoveSpeed> _moveSpeedPool;
        private EcsPool<Sun> _sunPool;
        private const float SPEED = 5f;

        public void RunOnEvent(ref ClickedEvent evt)
        {
            
            int id = evt.Target;
            if (!_sunPool.Has(evt.Target)) return;
            
            var targetPosition = Camera.main.GetCameraPosition(new Vector2(0.1f, 0.9f));
            var target =  _world.GetEntityLong(evt.Target);
            var transform = target.GetTransform();
            _moveSpeedPool.Get(id).Speed = SPEED;
            var time = target.MoveTo(targetPosition);
            transform.gameObject.layer = _layer;
                
            Bootstrap.Instance.RunOn(() => OnCollect(id), new WaitForSeconds(time));
        }

        public void Inject(EcsDefaultWorld obj)
        {
            _world = obj;
            _moveSpeedPool = _world.GetPool<MoveSpeed>();
            _sunPool = _world.GetPool<Sun>();
        }

        private void OnCollect(int id)
        {
            ref var playerData = ref _world.Get<PlayerData>();
            playerData.SunAmount += _sunPool.Get(id).Amount;
            Debug.Log($"Солнышек: {playerData.SunAmount.ToString()}");
            Spawner.Destroy(id);
        }
    }
}