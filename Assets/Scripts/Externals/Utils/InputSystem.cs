using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace Utils
{
    public static class InputSystem
    {
        public static PlayerControls Controls;
    
        public enum DeviceType
        {
            KeyboardMouse,
            Xbox,
            PlayStation,
            Switch,
            GenericGamepad,
            Unknown
        }

        private static Dictionary<Type, DeviceType> _baseDeviceMap = new()
        {
            { typeof(Keyboard), DeviceType.KeyboardMouse },
            { typeof(Mouse), DeviceType.KeyboardMouse },
            { typeof(Gamepad), DeviceType.GenericGamepad }
        };

        
        private static InputUser _user;
        private static InputDevice _lastDevice;

        public static DeviceType CurrentDeviceType;
        public static UnityEvent OnDeviceChanged;
    
        static InputSystem()
        {
            Controls = new PlayerControls();
            Controls.Enable();

        }
    
        public static Vector2 Move => Controls.Player.Move.ReadValue<Vector2>();
        public static Vector2 Look => Controls.Player.Look.ReadValue<Vector2>();

        public static bool Attack => Controls.Player.Attack.IsPressed() && !EventSystem.current.IsPointerOverGameObject();
        public static bool SubAttack => Controls.Player.Subattack.IsPressed() && !EventSystem.current.IsPointerOverGameObject();
        public static bool Attack_OneTime => Controls.Player.Attack.WasPressedThisFrame() && !EventSystem.current.IsPointerOverGameObject();
        public static bool SubAttack_OneTime => Controls.Player.Subattack.WasPressedThisFrame() && !EventSystem.current.IsPointerOverGameObject();
        public static bool SideAttack_OneTime => Controls.Player.Sideattack.WasPressedThisFrame() && !EventSystem.current.IsPointerOverGameObject();
        public static bool Jump => Controls.Player.Jump.IsPressed();
        public static bool Crouch => Controls.Player.Crouch.IsPressed();

        public static Vector2 Scroll => Controls.Player.Scroll.ReadValue<Vector2>();

        public static bool NavigateBack => Controls.UI.NavigateBack.WasPressedThisFrame();
        public static bool NavigateNext => Controls.UI.NavigateNext.WasPressedThisFrame();


    }
}
