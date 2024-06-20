using QFramework;

namespace PlatformShoot
{
    public class NextLevel : PlatformShootGameController, IInteractiveItem
    {
        void Start()
        {
            this.RegisterEvent<ShowPassDoorEvent>(e => gameObject.SetActive(true)).UnRegisterWhenGameObjectDestroyed(gameObject);
            gameObject.SetActive(false);
        }

        void IInteractiveItem.Trigger()
        {
            this.SendCommand(new NextLevelCommand("GamePassScene"));
            this.GetSystem<IAudioMgrSystem>().PlaySound("¾ªã¤Ð¦Éù");
        }
    }
}

