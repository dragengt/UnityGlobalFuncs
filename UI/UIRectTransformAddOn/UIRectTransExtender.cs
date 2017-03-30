using UnityEngine;
using System.Collections;

namespace Game.UI
{
	public static class UIRectTransExtender 
	{
        //为RectTransform增加一个扩展函数
        /// <summary>
        /// 根据id获得对应的rectTransform
        /// </summary>
        public static RectTransform GetChildById(this RectTransform parent, string childId)
        {
            UIRectTransManager rectTransManager = parent.GetComponent<UIRectTransManager>();
            RectTransform childGot = null;

            //没有管理器就null：
            if (rectTransManager == null)
            {
                return null;
            }
            else
            {
                //--尝试获取child
                rectTransManager.GetChildById(childId);
                return childGot;
            }
        }
        
	}
}