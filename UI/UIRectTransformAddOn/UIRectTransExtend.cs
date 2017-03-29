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
        /// <summary>
        /// 用于管理子物体的Dict
        /// </summary>
        [SerializeField]Dictionary<string, RectTransform> m_uiChildren = new Dictionary<string, RectTransform>();

        /// <summary>
        /// 获得用于管理子物体的Dict：只读
        /// </summary>
        public Dictionary<string, RectTransform> uiChildren { get { return m_uiChildren; } }
	}

}