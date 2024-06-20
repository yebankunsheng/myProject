using QFramework;
using UnityEngine;

namespace PlatformShoot
{
    public class StartPanel : PlatformShootUIController
    {
        // ����ǰ�ť�İ�ʹ�� ��������
        protected override void OnClick(string name)
        {
            switch (name)
            {
                case "StartBtn": this.SendCommand(new NextLevelCommand("SampleScene")); break;
                case "ExitBtn": Application.Quit(); break;
            }
        }

        protected override void OnToggleValueChange(string name, bool value)
        {
            // ��Toggle�������������trueʱ
            if (value)
            {
                switch (name)
                {
                    case "xxx":
                        // do toggle
                        break;
                    case "yyy":
                        // do toggle
                        break;
                }
            }
        }
    }
}

