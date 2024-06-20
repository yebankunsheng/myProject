using QFramework;
using UnityEngine;

namespace PlatformShoot
{
    public class PlatformShootGame : Architecture<PlatformShootGame>
    {
        protected override void Init()
        {
            RegisterModel<IGameModel>(new GameModel());
            RegisterModel<IGameAudioModel>(new GameAudioModel());

            RegisterSystem<ITimerSystem>(new TimerSystem());
            RegisterSystem<ICameraSystem>(new CameraSystem());
            RegisterSystem<IAudioMgrSystem>(new AudioMgrSystem());
            RegisterSystem<IObjectPoolSystem>(new ObjectPoolSystem());
            RegisterSystem<IInputDeviceMgrSystem>(new InputDeviceMgrSystem());
            RegisterSystem<IPlayerInputSystem>(new PlayerInputSystem());
            RegisterSystem<IGunSystem>(new GunSystem());
            
        }
    }

    public class PlatformShootGameController : MonoBehaviour, IController
    {
        IArchitecture IBelongToArchitecture.GetArchitecture() => PlatformShootGame.Interface;
    }

    public class PlatformShootUIController : UIPanel, IController
    {
        IArchitecture IBelongToArchitecture.GetArchitecture() => PlatformShootGame.Interface;
    }
}