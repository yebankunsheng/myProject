using UnityEngine;
using QFramework;
using UnityEngine.InputSystem;

namespace PlatformShoot
{
    public interface IPlayerInputSystem : ISystem
    {
        void Enable();
        void Disable();
    }

    public struct DirInputEvent
    {
        public int inputX;
        public int inputY;
    }

    public struct JumpInputEvent { }

    public struct ShootInputEvent
    {
        public bool isTrigger;
    }

    public enum E_InputDevice
    {
        Keyboard, Gamepad, Pointer
    }

    public class PlayerInputSystem : AbstractSystem, IPlayerInputSystem, GameControls.IGamePlayActions
    {
        private GameControls mControls = new();
        private DirInputEvent mInputEvent = new();
        private ShootInputEvent shootInput = new();
        private float sensitivity = 0.2f;
        private E_InputDevice mCurInputDevice;

        protected override void OnInit()
        {
            mControls.GamePlay.SetCallbacks(this);
            var deviceMgr = this.GetSystem<IInputDeviceMgrSystem>();
            deviceMgr.RegisterDeviceChange<Pointer>(() => mCurInputDevice = E_InputDevice.Pointer);
            deviceMgr.RegisterDeviceChange<Keyboard>(() => mCurInputDevice = E_InputDevice.Keyboard);
            deviceMgr.RegisterDeviceChange<Gamepad>(() => mCurInputDevice = E_InputDevice.Gamepad);
        }

        void IPlayerInputSystem.Disable()
        {
            mControls.GamePlay.Disable();
        }

        void IPlayerInputSystem.Enable()
        {
            mControls.GamePlay.Enable();
        }

        void GameControls.IGamePlayActions.OnJump(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            this.SendEvent<JumpInputEvent>();
        }

        void GameControls.IGamePlayActions.OnMove(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                // 获取方向输入 -1或 1
                Vector2 input = context.ReadValue<Vector2>();

                // 考虑摇杆的手感，防止摇杆的误触
                switch(mCurInputDevice)
                {
                    case E_InputDevice.Keyboard:
                        mInputEvent.inputX = (int)input.x;
                        mInputEvent.inputY = (int)input.y;
                        break;
                    case E_InputDevice.Gamepad:
                        mInputEvent.inputX = Mathf.Abs(input.x) < sensitivity ? 0 : input.x < 0 ? -1 : 1;
                        mInputEvent.inputY = Mathf.Abs(input.y) < sensitivity ? 0 : input.y < 0 ? -1 : 1;
                        break;
                }
                this.SendEvent(mInputEvent);
            }
            else if (context.canceled)
            {
                switch(mCurInputDevice)
                {
                    case E_InputDevice.Keyboard:
                        var board = Keyboard.current;
                        switch (mInputEvent.inputX)
                        {
                            case -1: mInputEvent.inputX = board.dKey.wasPressedThisFrame || board.rightArrowKey.wasPressedThisFrame ? 1 : 0;
                                break;
                            case 1: mInputEvent.inputX = board.aKey.wasPressedThisFrame || board.leftArrowKey.wasPressedThisFrame ? -1 : 0;
                                break;
                        }
                        switch (mInputEvent.inputY)
                        {
                            case -1: mInputEvent.inputY = board.wKey.wasPressedThisFrame || board.upArrowKey.wasPressedThisFrame ? 1 : 0;
                                break;
                            case 1: mInputEvent.inputY = board.sKey.wasPressedThisFrame || board.downArrowKey.wasPressedThisFrame? -1 : 0;
                                break;
                        }
                        break;
                    default:
                        mInputEvent.inputX = 0;
                        mInputEvent.inputY = 0;
                        break;
                }
                this.SendEvent(mInputEvent);
            }

            // performed相当于按键按下，canceled相当于按键抬起
            // 当一个方向键被按住的时候，按下另一个相反的方向键
            // 新输入系统会返回canceled状态默认是返回0，键盘的获
            // 取可以使用Keyboard.current.wKey.wasPressedThisFrame
            // || Keyboard.current.upArrowKey.wasPressedThisFrame
            // 参考猫叔方向键优化视频思考着实现一下方向键优化
        }

        void GameControls.IGamePlayActions.OnShoot(InputAction.CallbackContext context)
        {
            // 两种写法第一种
            if (context.started)
            {
                shootInput.isTrigger = context.ReadValueAsButton();
            }
            else if (context.canceled)
            {
                shootInput.isTrigger = false;
            }

            // 第二种可能会发送两次事件
            //shootInput.isTrigger = context.ReadValueAsButton();

            this.SendEvent(shootInput);
        }
    }
}
