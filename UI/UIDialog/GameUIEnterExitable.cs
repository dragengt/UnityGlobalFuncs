using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI.Component
{
    /// <summary>
    /// UI的 Enter,Exit 事件监听器；提供给UIFuncs 
    /// </summary>
    class GameUIEnterExitable: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
        CallbackFunc cb_onEnter;
        CallbackFunc cb_onExit;

        internal void SetOnEnterListener( CallbackFunc onEnter)
        {
            cb_onEnter = onEnter;
        }

        internal void SetOnExitListener(CallbackFunc onExit)
        {
            cb_onExit = onExit;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (cb_onEnter != null)
                cb_onEnter();
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (cb_onExit != null) 
                cb_onExit( );
        }
        
	}
}
