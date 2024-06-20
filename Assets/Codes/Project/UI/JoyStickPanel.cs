using QFramework;
using UnityEngine;

namespace PlatformShoot
{
    public class JoyStickPanel : PlatformShootGameController, ICanSendEvent
    {
        private void Awake()
        {
            transform.Find("LeftStickArea").GetComponent<StickDrag>().Register((x, y) =>
            {
                this.SendEvent(new DirInputEvent() { inputX = x, inputY = y});
            });
            transform.Find("RightStickArea").GetComponent<StickDrag>().Register((x, y) =>
            {
                Debug.Log($"”““°∏ÀÕœ“∑Œª÷√{x}, {y}");
            });
        }
    }
}

