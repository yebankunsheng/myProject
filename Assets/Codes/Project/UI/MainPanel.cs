using QFramework;
using UnityEngine.UI;
using UnityEngine;

namespace PlatformShoot
{
    public class MainPanel : PlatformShootUIController
    {
        private Text mScoreTex;
        private void Start()
        {
            mScoreTex = GetControl<Text>("ScoreTex");
            this.GetModel<IGameModel>().Score.RegisterWithInitValue(score => mScoreTex.text = score.ToString()).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        protected override void OnClick(string name)
        {
            if (name == "SettingBtn")
            {
                ResHelper.AsyncLoad<GameObject>("Item/SettingPanel", o =>
                {
                    o.transform.SetParent(GameObject.Find("Canvas").transform);
                    (o.transform as RectTransform).anchoredPosition = Vector2.zero;
                    Time.timeScale = 0;
                });
            }
        }
    }
}

