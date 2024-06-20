using QFramework;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformShoot
{
    public class AudioPlay : MonoBehaviour
    {
        private List<AudioSource> mPlayingList;
        public static AudioPlay Instance;
        private void Awake() => Instance = this;
        private void Start()
        {
            mPlayingList = new List<AudioSource>();

            // 保证音效在过场景时不被强制切断，对当前对象做一个转换保护
            DontDestroyOnLoad(gameObject);
        }
        public void PlaySound(string name)
        {
            var source = gameObject.AddComponent<AudioSource>();

            // 异步加载音效
            ResHelper.AsyncLoad<AudioClip>("Audio/Sound/" + name, clip =>
            {
                source.clip = clip;
                source.Play();
                mPlayingList.Add(source);
            }); 
        }
        private void Update()
        {
            //检测音频是否播放完成，完成的移除并销毁，由于移除音效会减少总Count，正向循环会导致索引越界，因而反向遍历
            for (int i = mPlayingList.Count - 1; i >= 0; i--)
            {
                var source = mPlayingList[i];
                if (!source.isPlaying)
                {
                    mPlayingList.RemoveAt(i);
                    Destroy(source);
                }
            }
        }
    }
}
