using System.Linq;
using DCFApixels.DragonECS;
using Karpik.Engine.Shared.DragonECS;
using TowerDefense.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TowerDefense
{
    public class Clickable : MonoBehaviour
    {
        private InputSystem_Actions _actions;
        private Camera _camera;
        private int _mask;
        private EcsDefaultWorld _world;
        private EcsEventWorld _eventWorld;

        private void Awake()
        {
            _actions = new InputSystem_Actions();
            _camera = Camera.main;
            _eventWorld = EcsEventWorldSingletonProvider.Instance.Get();
            _world = EcsDefaultWorldSingletonProvider.Instance.Get();
            _mask = LayerMask.GetMask(MaskConstants.PointerMask);
        }

        private void OnEnable()
        {
            _actions.Player.Enable();
            _actions.Player.Click.performed += OnClickPerformed;
        }
        

        private void OnDisable()
        {
            _actions.Player.Click.performed -= OnClickPerformed;
            _actions.Player.Disable();
        }

        private void OnClickPerformed(InputAction.CallbackContext obj)
        {
            var screenPosition = Pointer.current.position.ReadValue();
            Ray ray = _camera.ScreenPointToRay(screenPosition);
            int mask = _mask;
            ref var w = ref _world.Get<IgnorePointerMask>();
            if (w.Masks?.Count > 0)
            {
                var except = MaskConstants.PointerMask.Except(w.Masks);
                mask = LayerMask.GetMask(except.ToArray());
            }
            Physics.Raycast(ray, out var hit, Mathf.Infinity, mask);
            if (hit.collider != null)
            {
                var go = hit.collider.gameObject;
                if (go.TryGetComponent(out Provider provider))
                {
                    _eventWorld.SendEvent(new ClickedEvent()
                    {
                        Target = provider.Entity.ID
                    });
                }
            }
        }
    }
}