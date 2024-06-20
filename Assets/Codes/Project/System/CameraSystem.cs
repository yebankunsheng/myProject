using QFramework;
using UnityEngine;

namespace PlatformShoot
{
    public interface ICameraSystem : ISystem
    {
        void SetTarget(ICamTarget target);
    }
    public interface ICamTarget
    {
        Vector2 Pos { get; }
    }

    public class CameraSystem : AbstractSystem, ICameraSystem
    {
        private ICamTarget mTarget;

        /// <summary>
        /// 临时坐标 用于缓存需要计算的坐标
        /// </summary>
        private Vector3 mTargetPos;

        /// <summary>
        /// 跟随的范围 上下左右四个极限的长度
        /// </summary>
        private float minX = -100f, minY = -100f, maxX = 100f, maxY = 100f;

        /// <summary>
        /// 缓动速度
        /// </summary>
        private float mSmoothTime = 2;

        protected override void OnInit()
        {
            // 初始化时就注册好帧更新
            PublicMono.Instance.OnLateUpdate += Update;

            // 因为摄像机的Z轴在2D平台中坐标会保持为-10，所以可以在初始化中 可以将临时坐标的Z轴直接赋值为-10
            mTargetPos.z = -10;
        }

        void ICameraSystem.SetTarget(ICamTarget target) => mTarget = target;

        private void Update()
        {
            // 避免空目标
            if (mTarget.Equals(null)) return;

            // 给坐标的XY轴进行计算 并且对其进行限制 使用Clamp函数进行限制
            mTargetPos.x = Mathf.Clamp(mTarget.Pos.x, minX, maxX);
            mTargetPos.y = Mathf.Clamp(mTarget.Pos.y, minY, maxY);

            // 给摄像机赋值 使用Lerp可以让摄像机的运动有缓入缓出的效果 这里需要缓存一下摄像机
            var cam = Camera.main.transform;

            // 这里计算两个坐标之间的模长 也可以理解为两点的距离 如果距离接近0.01f 就不再对摄像机进行赋值
            if ((cam.position - mTargetPos).sqrMagnitude < 0.01f) return;

            // 后面需要填一下插值速度 给一个变量去管理 因为插值的算法特点 我们需要对最小值进行一个限制
            cam.localPosition = Vector3.Lerp(cam.position, mTargetPos, mSmoothTime * Time.deltaTime);

            //Camera.main.transform.localPosition = new Vector3(mTargets.position.x, mTargets.position.y, -10);
        }
    }
}
