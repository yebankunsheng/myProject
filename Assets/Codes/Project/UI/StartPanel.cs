using QFramework;
using UnityEngine;

namespace PlatformShoot
{
    public class StartPanel : PlatformShootUIController
    {
        // 这就是按钮的绑定使用 其他类型
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
            // 当Toggle发生变更并返回true时
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

