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
            // ��ȡҡ�˿ؼ������StickBg������
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
            // �����ĳ������������꣬��ǰ����λ�ã���ǰUI�����������һ���������(localPos�൱��ǰ�������ת������Ϣ�洢��localPos��)
                mStickBgTrans, data.position, data.pressEventCamera, out Vector2 localPos);
            Vector2 dir = localPos.normalized;

            // ��ί�е�Ŀ����Ϊ�˸����׵�������չ
            mStickChange?.Invoke(dir.x > 0 ? 1 : -1, dir.y > 0 ? 1 : -1);

            // ���Ʋ�����ҡ�˵�����
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