using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    // 基础计时器
    public class Timer
    {
        private Action OnFinished;
        private float mFinishTime;
        private float mDelayTime;
        private bool mLoop;
        private bool mIsFinish;
        public bool IsFinish => mIsFinish;

        // 开始计时
        public void Start(Action onFinished, float delayTime, bool isLoop)
        {
            OnFinished = onFinished;
            mFinishTime = Time.time + delayTime;
            mDelayTime = delayTime;
            mLoop = isLoop;
            mIsFinish = false;
        }

        // 提供给外部手动停止的方法
        public void Stop() => mIsFinish = true;

        // 更新计时器
        public void Update()
        {
            if (mIsFinish) return;
            if(Time.time < mFinishTime) return;
            if (!mLoop) Stop();
            else mFinishTime = Time.time + mDelayTime;
            OnFinished?.Invoke();
        }
    }

    public interface ITimerSystem : ISystem
    {
        Timer AddTimer(float delayTime, Action onFinished, bool isLoop = false);
    }

    // 计时器系统
    public class TimerSystem : AbstractSystem, ITimerSystem
    {
        private List<Timer> mUpdateList = new List<Timer>();
        private Queue<Timer> mAvailableQueue = new Queue<Timer>();
        protected override void OnInit()
        {
            PublicMono.Instance.OnUpdate += Update;
        }

        // 添加计时器
        Timer ITimerSystem.AddTimer(float delayTime, Action onFinished, bool isLoop)
        {
            var timer = mAvailableQueue.Count == 0 ? new Timer() : mAvailableQueue.Dequeue();
            timer.Start(onFinished, delayTime, isLoop);
            mUpdateList.Add(timer);
            return timer;
        }

        // 更新计时器
        public void Update()
        {
            if(mUpdateList.Count == 0) return;
            for(int i = mUpdateList.Count - 1; i >= 0; i--)
            {
                if (mUpdateList[i].IsFinish)
                {
                    mAvailableQueue.Enqueue(mUpdateList[i]);
                    mUpdateList.RemoveAt(i);
                    continue;
                }
                mUpdateList[i].Update();
            }
        }
    }
}
