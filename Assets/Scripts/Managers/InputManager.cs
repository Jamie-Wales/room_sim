#nullable enable

using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    public class InputManager : MonoBehaviour
    {
        private InputSystem _playerControls = null!;
        public static InputManager Instance { get; private set; } = null!;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            _playerControls = new InputSystem();
            _playerControls.Enable();
        }

        public float GetForward()
        {
            return _playerControls.Player.Move.ReadValue<Vector2>().y;
        }

        public float GetStrafe()
        {
            return _playerControls.Player.Move.ReadValue<Vector2>().x;
        }

        public Vector2 GetPitchYaw()
        {
            return _playerControls.Player.Look.ReadValue<Vector2>();
        }

        public void SetOnInteractPressed(UnityAction action)
        {
            _playerControls.Player.Interact.performed += _ => action.Invoke();
        }


        public void SetOnJumpPressed(UnityAction action)
        {
            _playerControls.Player.Jump.performed += _ => action.Invoke();
        }

        public void SetOnSprintPress(UnityAction action)
        {
            _playerControls.Player.Sprint.performed += _ => action.Invoke();
        }

        public void SetOnSprintRelease(UnityAction action)
        {
            _playerControls.Player.Sprint.canceled += _ => action.Invoke();
        }
    }
}