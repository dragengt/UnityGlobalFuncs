using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace Game
{
	public class SimpleAnimation : MonoBehaviour 
	{
        
        [SerializeField]float m_aniFrameDeltaTime = 0.3f;
        [SerializeField]Sprite [] m_imgsToSwitch;

        enum AniObjType { None, UI, Sprite };
        AniObjType m_aniObjType;

        SpriteRenderer m_spRenderer;
        Image m_uiRenderer;

		// Use this for initialization
		void OnEnable () 
		{
            
            if (m_imgsToSwitch.Length <= 0)
            {
                Debug.LogError("No imgs specified.");
                return;
            }

            m_spRenderer = this.GetComponent<SpriteRenderer>();
            m_uiRenderer = this.GetComponent<Image>();

            if (m_spRenderer != null)
            {
                m_aniObjType = AniObjType.Sprite;
            }
            else if (m_uiRenderer != null)
            {
                m_aniObjType = AniObjType.UI;
            }
            else
            {
                m_aniObjType = AniObjType.None;
            }

            //Has ani:
            if (m_aniObjType != AniObjType.None)
            {
                //启动ani更新：
                StartCoroutine(
                    GameObjFunc.IUpdateDo(DoUpdateAni, m_aniFrameDeltaTime));
            }
		}
		
		// DoUpdate is called once per deltaTime
        int m_currImgFrame = 0;
		void DoUpdateAni () 
		{
            m_currImgFrame = (m_currImgFrame + 1) % m_imgsToSwitch.Length;

		    //间隔更新，则不断切换图片
            switch (m_aniObjType)
            {
                case AniObjType.Sprite:
                    m_spRenderer.sprite = m_imgsToSwitch[m_currImgFrame];
                    break;

                case AniObjType.UI:
                    
                    m_uiRenderer.sprite = m_imgsToSwitch[m_currImgFrame];

                    break;

                case AniObjType.None:
                    Debug.LogError("Illegal param:");
                    break;
            }
		}
	}

}