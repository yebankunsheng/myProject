using System;

namespace QFramework
{
    public enum FadeState
    {
        Close,// 关闭
        FadeIn,
        FadeOut
    }

    public class FadeNum
    {
        // 淡入状态
        private FadeState mFadeState = FadeState.Close;

        // 是否启动
        public bool IsEnable => mFadeState != FadeState.Close;

        // 调用委托的时候避免提前结束
        private bool mInit = false;// 判断状态是中间插入还是初始就有的

        // 淡入结束后要做的事情
        private Action mOnEvent;

        // 当前值
        private float mCurrentValue;
        public float CurrentValue => mCurrentValue;

        // 最大最小范围
        private float mMin = 0, mMax = 1;

        // 设置范围
        public void SetMinMax(float min, float max)
        {
            mMin = min;
            mMax = max;
        }

        // 设置状态
        public void SetState(FadeState state, Action action = null)
        {
            mOnEvent = action;
            mFadeState = state;
            mInit = false;
        }

        // 需要在Update中持续检测
        public float Update(float step)
        {
            switch (mFadeState)
            {
                // 如果是渐入状态 0 - 1
                case FadeState.FadeIn:
                    if (!mInit)
                    {
                        mCurrentValue = mMin;
                        mInit = true;
                    }
                    if (mCurrentValue < mMax)
                    {
                        mCurrentValue += step;
                    }
                    else OnFinish(mMax);
                    break;

                // 如果是渐出状态 1 - 0
                case FadeState.FadeOut:
                    if (!mInit)
                    {
                        mCurrentValue = mMax;
                        mInit = true;
                    }
                    if (mCurrentValue > mMin)
                    {
                        mCurrentValue -= step;
                    }
                    else OnFinish(mMin);
                    break;
            }
            return mCurrentValue;
        }

        private void OnFinish(float value)
        {
            mOnEvent?.Invoke();
            mCurrentValue = value;
            if (!mInit) return; 
            mFadeState = FadeState.Close;
        }
    }
}
