using UnityEngine;
using System.Collections;

namespace Game.UI.Component
{
    [RequireComponent(typeof(Canvas))]

    /// <summary>
    /// 游戏中的主要画布，用于游戏play时的主界面服务；
    /// </summary>
    /// <remarks>挂载在主Canvas上即可实现单例获取</remarks>
	public class MainGameCanvas : MonoBehaviour 
	{
        //如果有指定UIContainer，则此将作为Canvas的RectTransform，否则Canvas根为Canvas
        [SerializeField] RectTransform m_uiContainner;

        static RectTransform g_UIContainer;
        
        internal static RectTransform canvasRectTransform
        {
            get
            {
                #if UNITY_EDITOR
                if (g_UIContainer == null)
                {
                    Debug.LogError("NO GAME CAVAS ASSIGNED IN CURRENT SCENE!");
                }
                #endif

                return g_UIContainer;
            }
        }

		void Awake () 
		{
            //如果指定Canvas的Container为另外物体，则指定之为其他UI悬挂的Parent
            if (m_uiContainner == null)
            {
                Debug.Log("No contain panel specifyed.So Canvas used as panel.");
                g_UIContainer = this.GetComponent<RectTransform>();
            }
            else
            {
                g_UIContainer = m_uiContainner;
            }
		}

	}
}