using UnityEngine;

//namespace QFramework
//{
//    public struct StopBgmEvent 
//    {
//        public bool isStop;
//    }
//    public interface IAudioMgrSystem : ISystem
//    {
//        void PlayBgm(string name);
//        void PlaySound(string name);
//        void GetSound(string name, Action<AudioSource> callback);
//        void StopSound(AudioSource source);
//        BindableProperty<float> BgmVolume {  get; }
//        BindableProperty<float> SoundVolume { get; }
//    }

//    // 实现音频池和音效管理组件
//    public class AudioMgrSystem : AbstractSystem, IAudioMgrSystem
//    {
//        public BindableProperty<float> BgmVolume { get; } = new BindableProperty<float>(1);
//        public BindableProperty<float> SoundVolume { get; } = new BindableProperty<float>(1);

//        // 背景音乐播放组件
//        private AudioSource mBGM;

//        // 临时音效播放组件，方便引用
//        private AudioSource tempSource;

//        // 音量渐变工具
//        private FadeNum mFade;

//        // 资源加载系统
//        private ResPool<AudioClip> mClipPool;

//        // AudioSource组件池
//        private ComponentPool<AudioSource> mSourcePool;

//        // 初始化系统
//        protected override void OnInit()
//        {
//            mSourcePool = new ComponentPool<AudioSource>("GameSound");
//            mClipPool = new ResPool<AudioClip>();
//            mFade = new FadeNum();
//            mFade.SetMinMax(0f, BgmVolume.Value);
//            this.RegisterEvent<StopBgmEvent>(OnStopBgm);
//            BgmVolume.RegisterWithInitValue(OnBgmVolumeChanged);
//            SoundVolume.RegisterWithInitValue(OnSoundVolumeChanged);
//        }

//        // 更新音量
//        private void Update()
//        {
//            if (!mFade.IsEnable) return;
//            mFade.Update(Time.deltaTime);
//            mBGM.volume = mFade.CurrentValue;
//        }

//        // 当背景音乐音量改变时
//        private void OnBgmVolumeChanged(float v)
//        {
//            mFade.SetMinMax(0, v);
//            if (mBGM == null) return;
//            mBGM.volume = v;
//        }

//        // 当音效音量被改变
//        private void OnSoundVolumeChanged(float v)
//        {
//            // 当有音效在播调节所有在播音效音量
//            mSourcePool.SetAllEnabledComponent(source => source.volume = v);
//        }

//        // 当停止背景音乐
//        private void OnStopBgm(StopBgmEvent e)
//        {
//            if(mBGM == null || !mBGM.isPlaying) return;
//            PublicMono.Instance.OnUpdate += Update;
//            mFade.SetState(FadeState.FadeOut, () =>
//            {
//                PublicMono.Instance.OnUpdate -= Update;
//                if(e.isStop) mBGM.Stop();// 停止背景音乐
//                else mBGM.Pause();// 暂停背景音乐
//            });
//        }

//        // 播放音效
//        public void PlaySound(string name)
//        {
//            // 先自动回收音频源组件
//            mSourcePool.AutoPush(cp => !cp.isPlaying);

//            // 从组件池获取一个组件
//            mSourcePool.Get(out tempSource);

//            // 从资源池获取一个音效
//            mClipPool.Get("Audio/Sound/" + name, clip =>
//            {
//                tempSource.clip = clip;
//                tempSource.loop = false;
//                tempSource.volume = SoundVolume.Value;
//                tempSource.Play();
//            });
//        }

//        // 获取音效
//        public void GetSound(string name, Action<AudioSource> callback)
//        {
//            // 先自动回收音频源组件
//            mSourcePool.AutoPush(cp => !cp.isPlaying);

//            // 从组件池获取一个组件
//            mSourcePool.Get(out tempSource);

//            // 从资源池获取一个音效
//            mClipPool.Get("Audio/Sound/" + name, clip =>
//            {
//                tempSource.clip = clip;
//                tempSource.loop = true;
//                tempSource.volume = SoundVolume.Value;
//                callback(tempSource);
//            });
//        }

//        // 播放BGM
//        public void PlayBgm(string name)
//        {
//            if (mBGM == null)
//            {
//                var o = new GameObject("GameBGM");
//                GameObject.DontDestroyOnLoad(o);
//                mBGM = o.AddComponent<AudioSource>();
//                mBGM.loop = true;
//                mBGM.volume = 0;
//            }
//            mClipPool.Get("Audio/Sound/" + name, audioClip =>
//            {
//                PublicMono.Instance.OnUpdate += Update;

//                // 如果没有BGM正在播放
//                if(!mBGM.isPlaying)
//                {
//                    mFade.SetState(FadeState.FadeIn, () =>
//                    {
//                        PublicMono.Instance.OnUpdate -= Update;// FadeIn结束时就把这个帧更新音量移除掉
//                    });
//                    mBGM.clip = audioClip;
//                    mBGM.Play();// 此时播放时切片已经是最大音量
//                }
//                else
//                {
//                    // 如果有BGM正在播放就先把音量降下来1 - 0，再播放当前音效0 - 1
//                    mFade.SetState(FadeState.FadeOut, () =>
//                    {
//                        mFade.SetState(FadeState.FadeIn, () =>
//                        {
//                            PublicMono.Instance.OnUpdate -= Update;
//                        });
//                        mBGM.clip = audioClip;
//                        mBGM.Play();
//                    });
//                }
//            });
//        }

//        // 停止循环音效
//        public void StopSound(AudioSource source)
//        {
//            mSourcePool.Push(source, source.Stop);
//        }
//    }
//}

namespace QFramework
{
    public interface IAudioMgrSystem : ISystem
    {
        void PlayBgm(string name);
        void PlaySound(string name);
        void StopBgm(bool isPause);
        AudioSource GetSound(string name);
        void RecoverySound(AudioSource source);
        void Clear();
    }

    // 实现音频池和音效管理组件
    public class AudioMgrSystem : AbstractSystem, IAudioMgrSystem
    {
        // 背景音乐播放组件
        private AudioSource mBGM;

        // 临时音效播放组件，方便引用
        private AudioSource tempSource;

        // 音量渐变工具
        private FadeNum mFade;

        // 资源加载系统
        private ResPool<AudioClip> mClipPool;

        // AudioSource组件池
        private ComponentPool<AudioSource> mSourcePool;

        // 游戏音频数据
        private IGameAudioModel mAudioModel;

        // 初始化系统
        protected override void OnInit()
        {
            mClipPool = new ResPool<AudioClip>();
            mSourcePool = new ComponentPool<AudioSource>("GameSound");

            mAudioModel = this.GetModel<IGameAudioModel>();

            mFade = new FadeNum();
            mFade.SetMinMax(0f, mAudioModel.BgmVolume.Value);

            mAudioModel.BgmVolume.Register(OnBgmVolumeChanged);
            mAudioModel.SoundVolume.Register(OnSoundVolumeChanged);

            PublicMono.Instance.OnUpdate += UpdateVolume;
        }

        void IAudioMgrSystem.PlaySound(string name)
        {
            InitSource();
            mClipPool.Get("Audio/Sound/" + name, clip =>
            {
                tempSource.clip = clip;
                tempSource.loop = false;
                tempSource.Play();
            });
        }

        AudioSource IAudioMgrSystem.GetSound(string name)
        {
            InitSource();
            mClipPool.Get("Audio/Sound/" + name, clip =>
            {
                tempSource.clip = clip;
                tempSource.loop = true;
            });
            return tempSource;
        }

        void IAudioMgrSystem.RecoverySound(AudioSource source)
        {
            mSourcePool.Push(source, source.Stop);
        }

        void IAudioMgrSystem.PlayBgm(string name)
        {
            mClipPool.Get("Audio/Sound/" + name, PlayBgm);
        }

        void IAudioMgrSystem.StopBgm(bool isPause)
        {
            if (mBGM == null || !mBGM.isPlaying) return;
            mFade.SetState(FadeState.FadeOut, () =>
            {
                if (isPause) mBGM.Pause();
                else mBGM.Stop();
            });
        }

        void IAudioMgrSystem.Clear()
        {
            mClipPool.Clear();
        }

        private void PlayBgm(AudioClip clip)
        {
            if (mBGM == null)
            {
                var o = new GameObject("GameBGM");
                GameObject.DontDestroyOnLoad(o);
                mBGM = o.AddComponent<AudioSource>();
                mBGM.loop = true;
                mBGM.volume = 0;
            }
            mBGM.clip = clip;
            if (!mBGM.isPlaying) PlayBgm();
            else mFade.SetState(FadeState.FadeOut, PlayBgm);
        }

        private void PlayBgm()
        {
            mFade.SetState(FadeState.FadeIn, () =>
            {
                PublicMono.Instance.OnUpdate -= UpdateVolume;
            });
            mBGM.Play();
        }

        private void OnBgmVolumeChanged(float v)
        {
            if (mBGM == null) return;
            mFade.SetMinMax(0, v);
            mBGM.volume = v;
        }

        private void UpdateVolume()
        {
            if (!mFade.IsEnable) return;
            mBGM.volume = mFade.Update(Time.deltaTime);
        }

        private void InitSource()
        {
            mSourcePool.AutoPush(cp => !cp.isPlaying);
            mSourcePool.Get(out tempSource);
            tempSource.volume = mAudioModel.SoundVolume.Value;
        }

        private void OnSoundVolumeChanged(float v)
        {
            mSourcePool.SetAllEnabledComponent(source => source.volume = v);
        }
    }
}