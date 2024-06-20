using QFramework;

namespace PlatformShoot
{
    public interface IInteractiveItem
    {
        void Trigger();
    }

    public class Trap : PlatformShootGameController, IInteractiveItem
    {
        void IInteractiveItem.Trigger()
        {
            this.SendCommand(new NextLevelCommand("GamePassScene"));
        }
    }
}

