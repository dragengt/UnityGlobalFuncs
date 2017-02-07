using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Game.UI.Inner;

namespace Game.UI.Component
{
    public class GameUIClickable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler ,IPointerClickHandler
	{
        CallbackFunc 
            m_cbOnClick,
            m_cbOnClickDown, 
            m_cbOnClickUp;

        public void SetOnClickListener(CallbackFunc cbOnClick, CallbackFunc cbOnClickDown = null,CallbackFunc cbOnClickUp = null)
        {
            m_cbOnClick = cbOnClick;
            m_cbOnClickDown = cbOnClickDown;
            m_cbOnClickUp = cbOnClickUp;
        }

        public void OnPointerUp(PointerEventData data)
        {
            //回调
            if (m_cbOnClickUp != null)
                m_cbOnClickUp();
        }


        public void OnPointerDown(PointerEventData data)
        {
            //回调：
            if (m_cbOnClickDown != null)
            {
                m_cbOnClickDown();
            }

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (m_cbOnClick != null)
                m_cbOnClick();
        }
        

        public void RemoveListeners()
        {
            m_cbOnClickDown = null;
            m_cbOnClickUp = null;
            m_cbOnClick = null;

        }
	}
}