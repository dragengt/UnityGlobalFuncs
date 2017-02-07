using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    [RequireComponent(typeof(LayoutElement))]
    [RequireComponent(typeof(Toggle))]

	class UIPanelHidable:MonoBehaviour
	{
        //--子物体：需要隐藏的panel部分
        [SerializeField]RectTransform m_hidablePanel;
        
        //--默认（隐藏内容panel）时的大小
        [SerializeField]int m_defaultHeight;
        //--展开Panel时的大小
        [SerializeField]int m_expandHeight;

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

        private void SwitchPanelTo(bool isOn)
        {
            if (isOn)
            {
                //switch on
                SetLayoutHeight(m_expandHeight);
            }
            else
            {
                //off
                SetLayoutHeight(m_defaultHeight);
            }

            m_hidablePanel.gameObject.SetActive(isOn);
        }

        private void SetLayoutHeight(int size)
        {
            LayoutElement layout = GetComponent<LayoutElement>();

            layout.preferredHeight = size;
            layout.minHeight = size;
        }
	}
}
