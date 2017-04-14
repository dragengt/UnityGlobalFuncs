using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Game.UI.Component
{
    [AddComponentMenu("Layout/UIDragable")]
    public class GameUIDragable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
	{
        [Tooltip("should align to grid when drag over?")]
        /// <summary>
        /// 是否对齐到网格上，对到0时即为不对齐。0值以上，即为位置会吸附到取整的单元格上
        /// </summary>
        [SerializeField]uint m_alignToGrid = 0;

        RectTransform m_rectTransform;

        Vector2 m_rectTransAnchoredPos;

        //开始拖拽时的触点位置
        Vector2 m_DragStartPos;

        /// <summary>
        /// 是否是有效的拖拽行为：只有当从当前物体开始点击，才能在Drag中生效；DragUp之后取消标记
        /// </summary>
        bool m_isValidDrag = false;

        void OnEnable()
        {
            m_rectTransform = GetComponent<RectTransform>();

            if (GetComponent<Image>() == null)
            {
                //Create an img that is invisible:
                Image img=  gameObject.AddComponent<Image>();
                img.color = Color.clear;
            }

            //TEST
            if (GetComponent<Button>())
            {
                Debug.LogWarning("Button attached, so  it will accpet click event that may confuse you when you drag it.");
            }
        }

        //--开始拖拽
        public void OnPointerDown(PointerEventData data)
        {
            //开始拖拽时记录鼠标位置
            m_DragStartPos = data.position;
            //记录当前Rect位置
            m_rectTransAnchoredPos = m_rectTransform.anchoredPosition;
            //允许拖拽
            m_isValidDrag = true;
        }

        //继承函数：Drag时触发
        public void OnDrag(PointerEventData data)
        {
            if (!m_isValidDrag)
                return;

            //新计算方法：
            //获得移动差值
            Vector2 posDelta = data.position - m_DragStartPos;

            Vector2 newPos = m_rectTransAnchoredPos + posDelta;

            //否则予以移动：
            //--更新移动视图
            m_rectTransform.anchoredPosition = newPos;
        }

        //拖拽完成
        public void OnPointerUp(PointerEventData data)
        {
            //标记移动结束：
            m_isValidDrag = false;

            if (m_alignToGrid > 0)
            {
                Vector2 vPos = m_rectTransform.anchoredPosition;
                vPos.x = (int)vPos.x / m_alignToGrid * m_alignToGrid;
                vPos.y = (int)vPos.y / m_alignToGrid * m_alignToGrid;
                m_rectTransform.anchoredPosition = vPos;
            }
        }

	}
}