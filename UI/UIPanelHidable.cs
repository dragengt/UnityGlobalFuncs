using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    [RequireComponent(typeof(LayoutElement))]
    [RequireComponent(typeof(Toggle))]

    /// <summary>
    /// 利用Toggle实现的抽屉式Panel，点击时可变换大小。
    /// </summary>
    ///<remarks>如果是手工创建的，则需要确保registClick勾选，才会默认产生点击时的隐藏/显示事件</remarks>
	class UIPanelHidable:MonoBehaviour
	{
        public enum HideDirection { Vertical, Horizontal, };

        //--子物体：需要隐藏的panel部分
        [SerializeField]RectTransform m_hidablePanel;

        [SerializeField]HideDirection m_hideDirection = HideDirection.Vertical;

        //--默认（隐藏内容panel）时的大小（忽略height的拼写）
        [SerializeField]int m_defaultSize;
        //--展开Panel时的大小（忽略height）
        [SerializeField]int m_expandSize;

        [Tooltip("Regist click event of toggle when enable this component")]
        [SerializeField]bool m_registClick = false;

        /// <summary>
        /// Expand detail panel
        /// </summary>
        public void ShowPanel()
        {
            SwitchPanelTo(true);
        }

        /// <summary>
        /// Hide detail panel
        /// </summary>
        public void HidePanel()
        {
            SwitchPanelTo(false);
        }

        /// <summary>
        /// 在需要时对Toggle的valueChanged进行事件的注册，以实现勾选时的显示/隐藏效果
        /// </summary>
        private void Awake()
        {
            if (m_registClick)
            {
                this.GetComponent<Toggle>().onValueChanged.AddListener((isOn) =>
                    {
                        if (isOn) 
                            ShowPanel();
                        else 
                            HidePanel();
                    });
            }
        }

        private void SwitchPanelTo(bool isOn)
        {
            if (isOn)
            {
                //switch on
                SetLayoutHeight(m_expandSize);
            }
            else
            {
                //off
                SetLayoutHeight(m_defaultSize);
            }

            m_hidablePanel.gameObject.SetActive(isOn);
        }

        private void SetLayoutHeight(int size)
        {
            LayoutElement layout = GetComponent<LayoutElement>();

            switch(m_hideDirection)
            {
                case HideDirection.Vertical:
                    layout.preferredHeight = size;
                    layout.minHeight = size;
                    break;

                case HideDirection.Horizontal:
                    layout.preferredWidth = size;
                    layout.minWidth = size;
                    break;
            }
        }
	}
}
