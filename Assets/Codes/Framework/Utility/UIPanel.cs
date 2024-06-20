using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace QFramework
{
    public class UIPanel : MonoBehaviour
    {
        Dictionary<string, List<UIBehaviour>> mControls;

        protected virtual void Awake()
        {
            // 实例化控件
            mControls = new();

            // 寻找子物体中的所有该控件，通过委托获取控件实现功能
            FindChildrenControl<Button>((name, control) => control.onClick.AddListener(() => OnClick(name)));
            FindChildrenControl<Toggle>((name, control) => control.onValueChanged.AddListener(isSelect => OnToggleValueChange(name, isSelect)));
            FindChildrenControl<Image>();
            FindChildrenControl<Text>();
            FindChildrenControl<RectMask2D>();
            FindChildrenControl<Slider>((name, control) => control.onValueChanged.AddListener(value => OnSliderValueChange(name, value)));
        }

        protected virtual void OnSliderValueChange(string name, float value) { }

        protected virtual void OnToggleValueChange(string name, bool isSelect) { }

        protected virtual void OnClick(string name) { }

        // FindChildrenControl也有GetControl一样拿到可用控件的能力
        protected T GetControl<T>(string name) where T : UIBehaviour
        {
            if (mControls.TryGetValue(name, out var controls))
            {
                for(int i = 0; i < controls.Count; i++)
                {
                    if (controls[i] is T) return controls[i] as T;
                }
            }
            return null;
        }

        protected void FindChildrenControl<T>(Action<string, T> callBack = null) where T : UIBehaviour
        {
            // 这里的Component是加s的
            T[] controls = GetComponentsInChildren<T>();
            for(int i = 0; i < controls.Length; i++)
            {
                T control = controls[i];
                string name = control.gameObject.name;
                callBack?.Invoke(name, control);
                if(mControls.ContainsKey(name))
                {
                    mControls[name].Add(control);
                }
                else
                {
                    mControls.Add(name, new List<UIBehaviour>() { control });
                }
            }
        }
    }
}
