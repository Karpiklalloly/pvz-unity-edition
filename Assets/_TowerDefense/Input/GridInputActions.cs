using UnityEngine;
using UnityEngine.InputSystem;

namespace TowerDefense.Input
{
    /// <summary>
    /// Wrapper для работы с Input System в контексте сетки
    /// </summary>
    public class GridInputActions : MonoBehaviour
    {
        private Mouse _mouse;
        private Keyboard _keyboard;

        // События для подписки
        public System.Action<Vector2> OnMouseMove;
        public System.Action<Vector2> OnMouseClick;

        private void Awake()
        {
            _mouse = Mouse.current;
            _keyboard = Keyboard.current;
        }

        private void Update()
        {
            if (_mouse == null) return;

            // Отслеживание движения мыши
            var mousePosition = _mouse.position.ReadValue();
            OnMouseMove?.Invoke(mousePosition);

            // Отслеживание кликов
            if (_mouse.leftButton.wasPressedThisFrame)
            {
                OnMouseClick?.Invoke(mousePosition);
            }
        }

        public Vector2 GetMousePosition()
        {
            return _mouse?.position.ReadValue() ?? Vector2.zero;
        }

        public bool IsMouseButtonPressed()
        {
            return _mouse?.leftButton.isPressed ?? false;
        }

        public bool WasMouseButtonPressedThisFrame()
        {
            return _mouse?.leftButton.wasPressedThisFrame ?? false;
        }

        public bool WasMouseButtonReleasedThisFrame()
        {
            return _mouse?.leftButton.wasReleasedThisFrame ?? false;
        }
    }
}