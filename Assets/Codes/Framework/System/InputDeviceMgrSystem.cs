using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

namespace QFramework
{
    public interface IInputDeviceMgrSystem : ISystem
    {
        void OnEnable();
        void OnDisable();
        void RegisterDeviceChange<T>(Action action);
    }

    /// <summary>
    /// 输入设备检测器
    /// InputSystem新输入系统包含如下：
    /// 1. ActionMap(包含Action，用户可自定义Action，
    /// 此Action不是委托，内部可包含多个输入功能，以
    /// 方法的形式存在)
    /// 2. onDeviceChange事件
    /// 3. onActionChange事件--最终还是会回到ActionMap
    /// 的整个系统中，因此两个系统同时出现时输入设备系
    /// 统要在ActionMap类型的系统之前注册，因为ActionMap
    /// 要使用这个输入设备的系统
    /// </summary>
    public class InputDeviceMgrSystem: AbstractSystem, IInputDeviceMgrSystem
    {
        /// <summary>
        /// 绑定输入设备切换事件
        /// </summary>
        private Dictionary<Type, Action> mDeviceSwitchTable;
        private InputDevice mCurDevice;

        protected override void OnInit()
        {
            mDeviceSwitchTable = new();
        }

        void IInputDeviceMgrSystem.OnEnable()
        {
            InputSystem.onActionChange += DetectCurrentInputDevice;
            InputSystem.onDeviceChange += OnDeviceChanged;
        }

        void IInputDeviceMgrSystem.OnDisable()
        {
            InputSystem.onActionChange -= DetectCurrentInputDevice;
            InputSystem.onDeviceChange -= OnDeviceChanged;
            mDeviceSwitchTable.Clear();
        }

        /// <summary>
        /// 注册设备切换事件
        /// </summary>
        void IInputDeviceMgrSystem.RegisterDeviceChange<T>(Action action)
        {
            var type = typeof(T);
            if (mDeviceSwitchTable.ContainsKey(type))
            {
                mDeviceSwitchTable[type] += action;
            }
            else
            {
                mDeviceSwitchTable.Add(type, action);
            }
        }

        /// <summary>
        /// 检测当前设备
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="change"></param>
        private void DetectCurrentInputDevice(object obj, InputActionChange change)
        {
            if (change != InputActionChange.ActionPerformed) return;
            SwitchingDevice((obj as InputAction).activeControl.device);
            #region 注解
            // InputAction用于获取当前设备，找到activateControl激活的控制器，最终获取device设备，将设备传递给SwitchingDevice方法，用于更改设备
            #endregion
        }

        /// <summary>
        /// 设备变更事件onDeviceChange会传递两个
        /// 参数InputDevice要变更的输入设备和
        /// InputDeviceChange输入设备改变的枚举，
        /// 枚举有多种情况
        /// </summary>
        /// <param name="device"></param>
        /// <param name="change"></param>
        private void OnDeviceChanged(InputDevice device, InputDeviceChange change)
        {
            switch (change)
            {
                case InputDeviceChange.Reconnected:
                    Debug.Log(device + "重新连接");

                    // 切换为当前设备
                    SwitchingDevice(device);
                    break;
                case InputDeviceChange.Disconnected:
                    Debug.Log(device + "断开连接");

                    // 切换为当前设备以外的设备 暂时先用键盘代替
#if UNITY_STANDALONE || UNITY_EDITOR // 这个宏定义是在Windows和编辑器中检测设备
                    if (device is Gamepad) device = InputSystem.GetDevice<Keyboard>(); // 也可以写成这种形式device = Keyboard.current;
                    SwitchingDevice(device);
#endif
                    break;
            }
        }

        /// <summary>
        /// 切换设备
        /// </summary>
        /// <param name="device"></param>
        private void SwitchingDevice(InputDevice device)
        {
            if(device == null || mCurDevice == device) return;
            Type type = null;
            if(device is Keyboard) type = typeof(Keyboard);
            else if(device is Gamepad) type = typeof(Gamepad);
            else if(device is Pointer) type = typeof(Pointer);
            else if(device is Joystick) type = typeof(Joystick);
            if (type == null || !mDeviceSwitchTable.TryGetValue(type, out var callBack)) return;
            mCurDevice = device;
            callBack?.Invoke();
        }
    }
}
