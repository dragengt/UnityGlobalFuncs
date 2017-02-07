using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Game.UI
{
    [RequireComponent(typeof (VerticalLayoutGroup))]
    [RequireComponent(typeof (ToggleGroup))]
    /// <summary>
    /// 可隐藏Panel的管理器
    /// </summary>
	class UIPanelHidableGroup:MonoBehaviour
	{
        //--当前group下选择中的id
        int m_currSelectId = -1;

        /// <summary>
        /// Add up a number of panels & set it hidable.
        /// </summary>
        public void AddBatchChildren(UIPanelHidable[] panels,IntCallbackFuncWithInt cbOnExpandPanel_i = null)
        {
            int childCount = panels.Length;
            Transform m_transform = this.transform;
            ToggleGroup m_toggleGroup = this.GetComponent<ToggleGroup>();
            Toggle tmpCurrToggle;
            GameObject tmpCurrChild;

            //遍历所有即将添加的panel：
            for (int i = 0; i < childCount; i++)
            {
                tmpCurrChild = panels[i].gameObject;
                
                //--attach panel:
                GameUIFuncs.AttachRectTrans(tmpCurrChild.transform, m_transform);

                //--Get toggle:
                tmpCurrToggle = tmpCurrChild.GetComponent<Toggle>();
                tmpCurrToggle.group = m_toggleGroup;
                

                //--Listenner:
                #region add listenner to panel
                //创建会暂时持有的临时数据
                int toggleIndex = i;
                UIPanelHidable currPanel = panels[i];
                tmpCurrToggle.onValueChanged.AddListener((isOn) =>
                {
                    if (isOn)
                    {
                        //Current selection:
                        m_currSelectId = toggleIndex;

                        //Expand panel
                        currPanel.ShowPanel();
                        
                    }
                    else
                    {
                        //Cancel Toggle:
                        //--Shrink toggle
                        //Debug.Log("Shrink."+btnIndex);
                        currPanel.HidePanel();

                        //取消的是当前toggle，则需要将当前buildingSelected置-1，防止再次shrink
                        if (toggleIndex == m_currSelectId)
                        {
                            m_currSelectId = -1;
                        }
                    }

                    if (cbOnExpandPanel_i != null)
                    {
                        cbOnExpandPanel_i(toggleIndex);
                    }
                });
                #endregion
            }

        }
        
        

        
	}
}
