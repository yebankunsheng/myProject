using System;
using System.Collections;
using UnityEngine;

namespace QFramework
{
    public class ResHelper
    {
        public static T SyncLoad<T>(string name) where T : UnityEngine.Object
        {
            T res = Resources.Load<T>(name);
            return res is GameObject ? GameObject.Instantiate(res) : res;
        }
        private static IEnumerator AsyncLoadRes<T>(string name, Action<T> callBack) where T : UnityEngine.Object
        {
            var r = Resources.LoadAsync<T>(name);
            while (r.isDone) yield return null;// ��Դδ������ɾͽ��еȴ�
            callBack(r.asset is GameObject ? GameObject.Instantiate(r.asset) as T : r.asset as T);
        }
        public static void AsyncLoad<T>(string name, Action<T> callBack) where T : UnityEngine.Object
        {
            // MonoBehaviour���ܿ���Э��
            PublicMono.Instance.StartCoroutine(AsyncLoadRes(name, callBack)); 
        }
    }
}

