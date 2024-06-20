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

            // ��֤��Ч�ڹ�����ʱ����ǿ���жϣ��Ե�ǰ������һ��ת������
            DontDestroyOnLoad(gameObject);
        }
        public void PlaySound(string name)
        {
            var source = gameObject.AddComponent<AudioSource>();

            // �첽������Ч
            ResHelper.AsyncLoad<AudioClip>("Audio/Sound/" + name, clip =>
            {
                source.clip = clip;
                source.Play();
                mPlayingList.Add(source);
            }); 
        }
        private void Update()
        {
            //�����Ƶ�Ƿ񲥷���ɣ���ɵ��Ƴ������٣������Ƴ���Ч�������Count������ѭ���ᵼ������Խ�磬����������
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
