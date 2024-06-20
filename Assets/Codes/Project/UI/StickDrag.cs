using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PlatformShoot
{
    public class StickDrag : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        private RectTransform mStickBgTrans;
        private Transform mStickCtrlTrans;
        private float maxRange;
        private Action<int, int> mStickChange;

        private void Start()
        {
            mStickBgTrans = transform.Find("StickBg").GetComponent<RectTransform>();
            mStickCtrlTrans = mStickBgTrans.Find("StickCtrl");
            maxRange = mStickBgTrans.rect.width * 0.5f;
        }
        void IDragHandler.OnDrag(PointerEventData data)
        {
            // 获取摇杆控件相对于StickBg的坐标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
            // 相对于某个父对象的坐标，当前鼠标的位置，当前UI摄像机，返回一个相对坐标(localPos相当于前面的条件转化成信息存储在localPos中)
                mStickBgTrans, data.position, data.pressEventCamera, out Vector2 localPos);
            Vector2 dir = localPos.normalized;

            // 该委托的目的是为了更容易的增加扩展
            mStickChange?.Invoke(dir.x > 0 ? 1 : -1, dir.y > 0 ? 1 : -1);

            // 限制并更新摇杆的坐标
            mStickCtrlTrans.localPosition = localPos.magnitude > maxRange ? dir * maxRange : localPos;
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData data)
        {
            
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData data)
        {
            mStickCtrlTrans.localPosition = Vector2.zero;
            mStickChange?.Invoke(0, 0);
        }

        public void Register(Action<int, int> stickChange) => mStickChange += stickChange;
    }
}