using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class ComponentPool<T> where T : Behaviour// 继承自Behaviour的目的是要使用Enable属性
    {
        // 组件根对象
        private GameObject mRoot;

        // 指定组件的名字
        private string mRootName;

        // 储存所有激活组件
        private List<T> mOpenList = new List<T>();

        // 存储所有未使用组件
        private Queue<T> mCloseList = new Queue<T>();
        public ComponentPool(string rootObjName)
        {
            mRootName = rootObjName;
        }

        // 设置所有已激活组件的相同参数
        public void SetAllEnabledComponent(Action<T> callBack)
        {
            foreach (T component in mOpenList) callBack(component);
        }

        // 获取一个可使用的组件
        public void Get(out T component)
        {
            if (mCloseList.Count > 0)
            {
                component = mCloseList.Dequeue();// 获取一个未使用的组件
                component.enabled = true;// 激活组件
            }
            else
            {
                // 如果关闭列表没有东西的时候，可能是第一次使用
                if(mRoot == null)
                {
                    mRoot = new GameObject(mRootName);
                    GameObject.DontDestroyOnLoad(mRoot);
                }
                component = mRoot.AddComponent<T>();
            }
            // 把激活的组件放入开启列表
            mOpenList.Add(component);
        }

        // 自动回收组件
        public void AutoPush(Func<T, bool> condition)
        {
            // 可能开启列表有东西可以回收，逆向遍历，回收满足条件的组件
            for(int i = mOpenList.Count - 1; i >= 0; i--)
            {
                // 如果为true就回收组件
                if (condition(mOpenList[i]))
                {
                    mOpenList[i].enabled = false;
                    mCloseList.Enqueue(mOpenList[i]);
                    mOpenList.RemoveAt(i);
                }
            }
        }

        // 回收单个组件
        public void Push(T component, Action callBack = null)
        {
            if (mOpenList.Contains(component))
            {
                callBack?.Invoke();
                mOpenList.Remove(component);
                component.enabled = false;
                mCloseList.Enqueue(component);
            }
        }
    }
}
