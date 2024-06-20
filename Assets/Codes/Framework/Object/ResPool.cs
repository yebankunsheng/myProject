using System;
using System.Collections.Generic;

namespace QFramework
{
    public class ResPool<T> where T : UnityEngine.Object
    {
        private Dictionary<string, T> mResDict = new Dictionary<string, T>();
        public void Get(string key, Action<T> callBack)
        {
            if (mResDict.TryGetValue(key, out T data))
            {
                callBack(data);
                return;
            }
            mResDict.Add(key, null);
            ResHelper.AsyncLoad<T>(key, o =>
            {
                callBack(o);
                mResDict[key] = o;
            });
        }

        // 清理资源
        public void Clear() => mResDict.Clear();
    }
}
