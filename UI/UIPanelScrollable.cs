using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// 于UI物体，使之成为一个可以滚动的物体（如滚动多页窗口）
    /// </summary>
	public class UIPanelScrollable : MonoBehaviour 
	{
        RectTransform m_rectTrans;
        //Panel起始位置：
        Vector3 m_panelStartPos;
        
        /// <summary>
        /// 当前是否在滚动中，如果滚ing，则拒绝动作；
        /// </summary>
        bool m_isScrolling = false;

        /// <summary>
        /// 如果非滚动时，当前的状态；
        /// </summary>
        enum CurrStatus { Hiding ,Showing };
        CurrStatus m_currStatus = CurrStatus.Showing;

        /// <summary>
        /// 将滚动的方向：
        /// </summary>
        enum ScrollDirection { ToLeft, ToRight, ToUp, ToDown };
        [SerializeField] ScrollDirection m_scrollDirectionToHide;
        [SerializeField] Button m_buttonScroll;
		void Start () 
		{
            //检错：
            if (this.GetComponent<Button>())
            {
                Debug.LogError("No button allowed at father obj>");
            }

            //获取Canvas目前的大小（如果有）：
            float canvasSizeX = MyUICanvasScaler.g_preferSizeX;
            float canvasSizeY = MyUICanvasScaler.g_preferSizeY;
            
            //获得RectTransform
            m_rectTrans = this.GetComponent<RectTransform>();
            m_panelStartPos = m_rectTrans.position;
            
            //获得当前Pannel大小：
            float panelWidth = m_rectTrans.rect.width;
            float panelHeight = m_rectTrans.rect.height;

            //获得Scroll的按钮：
            //--获得其Rect：
            Rect scrollButtonRect = m_buttonScroll.GetComponent<RectTransform>().rect;
            
            //为了隐藏Panel所需要滑动的位移值：
            float scrollHideDeltaX = 0, scrollHideDeltaY = 0;

            //--计算出滑入、滑出所需要的位移：（画布的，非World坐标中的实际位移）
            scrollHideDeltaX = panelWidth- scrollButtonRect.width;
            scrollHideDeltaY = panelHeight - scrollButtonRect.height;

            //--再根据当前画布大小，得出实际所需要移动的像素点单位：
            scrollHideDeltaX = scrollHideDeltaX / canvasSizeX * Screen.width;
            scrollHideDeltaY = scrollHideDeltaY / canvasSizeY * Screen.height;
            
            //--根据选择的滑动方向，去掉不必要的差值，和给定方向：
            switch(m_scrollDirectionToHide)
            {
                case ScrollDirection.ToLeft:
                    //----负方向移动
                    scrollHideDeltaX *= -1;
                    scrollHideDeltaY = 0;
                    break;

                case ScrollDirection.ToRight:
                    scrollHideDeltaY = 0;
                    break;

                case ScrollDirection.ToUp:
                    scrollHideDeltaX= 0;
                    break;

                case ScrollDirection.ToDown:
                    scrollHideDeltaX = 0;    
                
                    //----负方向移动
                    scrollHideDeltaY *= -1;
                    break;
            }

            //给Scroll按钮添加回滚功能：
            m_buttonScroll.onClick.AddListener(() =>
            {
                //拒绝滚动
                if(m_isScrolling)
                    return;

                m_isScrolling = true;

                Vector3 targetMoveDelta = Vector3.zero;

                //判断当前要收起还是要显示：
                if(m_currStatus== CurrStatus.Hiding)
                {
                    //显示之：无操作，因为位移已经是0了；
                    
                    //翻转状态：
                    m_currStatus = CurrStatus.Showing;
                }
                else if (m_currStatus == CurrStatus.Showing)
                {
                    //藏ing，则显示之：
                    targetMoveDelta.x = scrollHideDeltaX;
                    targetMoveDelta.y = scrollHideDeltaY;
                    
                    //翻转状态：
                    m_currStatus = CurrStatus.Hiding;
                }

                
                //--计算目标位置
                //Vector3 targetPos = new Vector3(
                //    m_isMenuScrolledToHide ? 1F : -1F *//左移，或者往右移动
                //    Screen.width * 0.25F, 0, 0);//200(menuW)/800(screenW) = 0.25

                //--移动到该位置
                StartCoroutine(GameUIFuncs.IMoveUIToRectTransPos(m_rectTrans,
                    m_panelStartPos + targetMoveDelta,
                    () =>
                    {
                        //移动完成后：
                        m_isScrolling = false;
                    }
                    ));
            });
        }
		
	}
}