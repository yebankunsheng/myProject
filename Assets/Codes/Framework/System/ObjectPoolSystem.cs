using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public interface IObjectPoolSystem : ISystem
    {
        GameObject Get(string name);
        void Get(string name, Action<GameObject> callback = null);
        void Recovery(GameObject obj);
        void Dispose();
    }

    public class ObjectPoolSystem : AbstractSystem, IObjectPoolSystem
    {
        // 缓存池字典容器
        private Dictionary<string, PoolData> mPoolDict;

        // 缓存池根对象
        private Transform mPoolRoot;

        protected override void OnInit()
        {
            mPoolDict = new Dictionary<string, PoolData>();
        }

        void IObjectPoolSystem.Dispose()
        {
            mPoolDict.Clear();
            mPoolRoot = null;
        }

        GameObject IObjectPoolSystem.Get(string name)
        {
            return mPoolDict.TryGetValue(name, out PoolData data) && data.CanGet ? data.Get() : new GameObject(name);
        }

        void IObjectPoolSystem.Get(string name, Action<GameObject> callback)
        {
            // 如果有对应的格子，如果格子有东西，就取出来
            if (mPoolDict.TryGetValue(name, out PoolData data) && data.CanGet)
            {
                if (callback == null) data.Get();
                else callback(data.Get());
                return;
            }
            // 异步加载资源 创建对象给外部用 如果回调函数不为空 则抛出该对象
            ResHelper.AsyncLoad<GameObject>(name, o =>
            {
                o.name = name;// 设置名字的原因，由于加载游戏对象时引擎是以克隆的形式加载的，此时回收时会受到阻碍，因而给它更改一个名字，回收时知道它是谁，会更容易回收
                callback(o);
            });
            
        }

        // 把加载的资源放入缓存池中
        void IObjectPoolSystem.Recovery(GameObject obj)
        {
            if (mPoolDict.TryGetValue(obj.name, out var data))
            {
                data.Push(obj);
                return;
            }
            // 判断是否有根对象，没有就创建一个
            if (mPoolRoot == null)
            {
                var o = new GameObject("PoolRoot");
                GameObject.DontDestroyOnLoad(o);
                mPoolRoot = o.transform;
            }
            mPoolDict.Add(obj.name, new PoolData(obj, mPoolRoot));
        }
    }

    // 游戏对象缓存池
    public class PoolData
    {
        // 可激活对象的队列
        private Queue<GameObject> mActivatableObject = new();

        // 可获取对象标识
        public bool CanGet => mActivatableObject.Count > 0;

        // 对象挂载的父节点
        private Transform mFatherObj;
        public PoolData(GameObject obj, Transform root) 
        {
            mFatherObj = new GameObject(obj.name).transform;
            mFatherObj.SetParent(root.transform);
            Push(obj);
        }

        public void Push(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(mFatherObj.transform);
            mActivatableObject.Enqueue(obj);
        }

        public GameObject Get()
        {
            GameObject obj = mActivatableObject.Dequeue();
            GameObject.DontDestroyOnLoad(obj);
            obj.SetActive(true);
            obj.transform.SetParent(null);
            return obj;
        }
    }
}
