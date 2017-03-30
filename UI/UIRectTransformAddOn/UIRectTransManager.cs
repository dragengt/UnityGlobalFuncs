using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game.UI
{
    [RequireComponent(typeof(RectTransform))]
    /// <summary>
    /// 用于增强RectTransform管理子UI的行为：将这个脚本添加到对应的UI上，并设置《string，transform》，则RectTransform将可以获得对应string下的transform
    /// </summary>
	public class UIRectTransExtend : MonoBehaviour 
	{
        [Tooltip("link to child with id")]
        /// <summary>
        /// 用于管理子物体的Dict
        /// </summary>
        [SerializeField]Dictionary<string, RectTransform> m_uiChildren = new Dictionary<string, RectTransform>();

        /// <summary>
        /// 获得用于管理子物体的Dict：只读
        /// </summary>
        public Dictionary<string, RectTransform> uiChildren { get { return m_uiChildren; } }

        //为RectTransform增加一个扩展函数
        /// <summary>
        /// 根据id获得对应的rectTransform
        /// </summary>
        public RectTransform GetChildById(string childId)
        {
            RectTransform childGot = null;

            //--尝试获取child
            uiChildren.TryGetValue(childId, out childGot);
            return childGot;
        }
        
	}

}