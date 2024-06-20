using UnityEngine;

namespace QFramework
{
    public class MonoSingle<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T mInstance;
        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    var o = new GameObject(typeof(T).Name);
                    mInstance = o.AddComponent<T>();
                    DontDestroyOnLoad(o);
                }
                return mInstance;
            }
        }
    }
}
