using UnityEngine;
using QFramework;
using UnityEngine.UI;

namespace PlatformShoot
{
    public class SettingPanel : PlatformShootUIController
    {
        private IGameAudioModel mGameAudio;
        private Slider mBgmSlider;
        private Slider mSoundSlider;
        private void Start()
        {
            mGameAudio = this.GetModel<IGameAudioModel>();
            mBgmSlider = GetControl<Slider>("BgmVolume");
            mBgmSlider.value = mGameAudio.BgmVolume.Value;
            mSoundSlider = GetControl<Slider>("SoundVolume");
            mSoundSlider.value = mGameAudio.SoundVolume.Value;
        }

        protected override void OnClick(string name)
        {
            if (name == "CloseBtn")
            {
                Destroy(gameObject);
                Time.timeScale = 1.0f;
            }
        }

        protected override void OnSliderValueChange(string name, float value)
        {
            switch (name)
            {
                case "BgmVolume": mGameAudio.BgmVolume.Value = value; break;
                case "SoundVolume": mGameAudio.SoundVolume.Value = value; break;
            }
        }
    }
}

