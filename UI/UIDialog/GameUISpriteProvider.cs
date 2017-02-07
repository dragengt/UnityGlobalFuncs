using UnityEngine;
using System.Collections;

/// <summary>
/// 游戏风格化的Sprite
/// </summary>
[System.Serializable]
public struct GameUISkin
{
    public Sprite m_UIBtnNormal;
    public Sprite m_UIBtnDisable;
}

namespace Game.UI.Inner
{
    [ExecuteInEditMode]
	public class GameUISpriteProvider : MonoBehaviour 
	{
        public static GameUISpriteProvider instance;
        public static GameUISpriteProvider g_instance
        {
            get 
            {
                if (instance == null)
                {
                    Debug.LogError("No provider found , so i create one for you with no any sprite attached.");
                    GameObject go = new GameObject("UISpriteProvider");
                    instance = go.AddComponent<GameUISpriteProvider>();
                }
                return instance;
            }
        }

        /// <summary>
        /// 无阴影背景图
        /// </summary>
        public Sprite m_UISprite;
        
        /// <summary>
        /// 圆形图片
        /// </summary>
        public Sprite m_UIKnob;

        /// <summary>
        /// 输入框背景图
        /// </summary>
        public Sprite m_UIInputFieldBG;

        /// <summary>
        /// 向下箭头
        /// </summary>
        public Sprite m_UIDownArrow;

        /// <summary>
        /// 打勾标记图
        /// </summary>
        public Sprite m_UICheckMark;

        /// <summary>
        /// 较灰色背景图
        /// </summary>
        public Sprite m_UIBG;

        /// <summary>
        /// 游戏风格化定制UI
        /// </summary>
        public GameUISkin m_gameStyleUI;

        void Awake()
        {
            Debug.Log(this.gameObject.name + "awake");
            instance = this;
        }

	}
}