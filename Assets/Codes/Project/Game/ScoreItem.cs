using QFramework;

namespace PlatformShoot
{
    public class ScoreItem : PlatformShootGameController, IInteractiveItem
    {
        void IInteractiveItem.Trigger()
        {
            Destroy(gameObject);
            this.GetModel<IGameModel>().Score.Value++;
            this.GetSystem<IAudioMgrSystem>().PlaySound("นึ");
        }
    }
}

