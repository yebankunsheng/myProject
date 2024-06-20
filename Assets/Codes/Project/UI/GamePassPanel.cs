using UnityEngine.SceneManagement;

namespace PlatformShoot
{
    public class GamePassPanel : PlatformShootUIController
    {
        protected override void OnClick(string name)
        {
            if (name == "ResetGameBtn")
            {
                SceneManager.LoadScene("SampleScene");
            }
        }
    }
}