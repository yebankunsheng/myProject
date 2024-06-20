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
            // ʵ�����ؼ�
            mControls = new();

            // Ѱ���������е����иÿؼ���ͨ��ί�л�ȡ�ؼ�ʵ�ֹ���
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

        // FindChildrenControlҲ��GetControlһ���õ����ÿؼ�������
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
            // �����Component�Ǽ�s��
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
