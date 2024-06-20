using QFramework;

namespace PlatformShoot
{
    public class PlayerInputHandle : PlatformShootGameController
    {
        public int InputX { get; private set; }
        public int InputY { get; private set; }
        public bool JumpInput { get; set; }
        public bool AttackInput { get; private set; }
        private void Start()
        {
            this.RegisterEvent<DirInputEvent>(e =>
            {
                InputX = e.inputX;
                InputY = e.inputY;
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<ShootInputEvent>(e =>
            {
                AttackInput = e.isTrigger;
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<JumpInputEvent>(e =>
            {
                JumpInput = true;
                //if (mJumpCount > 0)
                //{
                //    JumpInput = true;
                //    isJumping = true;
                //}
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
    }
}
