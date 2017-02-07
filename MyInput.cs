using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 自定义的触摸屏操作：
/// 给定移动范围、在移动范围内操作摇杆图片，并更新CrossPlatformInput的轴向值
/// (如果不Attach到物体上，则只有鼠标输入可获取，如果需要虚拟Pad，请Attach到虚拟摇杆触点子物体中)
/// </summary>
public class MyInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    #region --虚拟摇杆用--
    //--虚拟按键中的ID配置用
    [SerializeField]
    int m_idBtnJump = 0;            //跳跃键在Canvas中的序号
    [SerializeField]
    int m_idBtnAttack = 1;            //攻击键在Canvas中的序号
    [SerializeField]
    int m_idBtnThrow = 2;            //投掷键(额外功能键)在Canvas中的序号

    //--虚拟摇杆用
    static Vector2 axisVector;
    //--任何虚拟摇杆中的按键按下了：
    static bool isAnyVirtualAxisDown;
    static bool isJumpButtonDown;
    static bool isThrowButtonDown;
    static bool isAttackButtonDown;

    public enum AxisOption
    {
        // Options for which axes to use
        Both, // Use both
        OnlyHorizontal, // Only horizontal
        OnlyVertical // Only vertical
    }

    public AxisOption axesToUse = AxisOption.Both;                    // The options for the axes that the still will use
    RectTransform moveRangeParent;
    int MoveRadiusRange = 100;                                                    //The radius of movement range.

    Vector2 m_StartAnchoredPos;
    Vector2 m_StartDragPos;
    bool m_UseX = true;                                                                // Toggle for using the x axis
    bool m_UseY = true;                                                                // Toggle for using the Y axis

    RectTransform m_rectTransform;

    #endregion
    //--------------------------------------------------------------------------------------------------------------
    //------------------------------------------------公用静态函数------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------
    #region --虚拟摇杆以及按钮事件
    public static float GetVerticalAxis()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        //PC 下，则尝试尝试键盘的咯
        if (!isAnyVirtualAxisDown)
        {
            //尝试获取键盘的：
            axisVector.y = Input.GetAxis("Vertical");
        }
#endif
        return axisVector.y;
    }
    public static float GetHorizontalAxis()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        //PC 下，还可尝试尝试键盘输入
        if (!isAnyVirtualAxisDown)
        {
            //尝试获取键盘的：
            axisVector.x = Input.GetAxis("Horizontal");
        }
#endif
        return axisVector.x;
    }

    public static bool IsAnyAxisChange()
    {
#if UNITY_EDITOR|| UNITY_STANDALONE
        if (GetHorizontalAxis() != 0 || GetVerticalAxis() != 0 )
        {
            return true;
        }
#endif
        //探测是否按下了虚拟摇杆
        return isAnyVirtualAxisDown;
    }

#if UNITY_EDITOR|| UNITY_STANDALONE
    static Vector3[] m_lastPCMousePoses = new Vector3[3];
    //PC端的中键移动
    static Vector3 GetMouseDelta(int mouseBt = 2)
    {
        if (Input.GetMouseButtonDown(mouseBt))
        {
            m_lastPCMousePoses[mouseBt] = mousePosition;
            return Vector3.zero;
        }
        else if (Input.GetMouseButton(mouseBt))
        {
            Vector2 delta = m_lastPCMousePoses[mouseBt] - mousePosition;
            m_lastPCMousePoses[mouseBt] = mousePosition;
            return delta;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public static Vector2 GetRotateAxis()
    {
        return GetMouseDelta(1);
    }

#endif


    /// <summary>
    /// 获得方向上的移动（如WASD、或者虚拟摇杆）
    /// </summary>
    public static Vector2 GetMoveAxis()
    {
        //PC下：
#if UNITY_EDITOR|| UNITY_STANDALONE
        //没按下虚拟摇杆：
        if (!isAnyVirtualAxisDown)
        {
            //尝试获得WASD移动方向：
            GetHorizontalAxis();
            GetVerticalAxis();
        }
#endif
        return axisVector;
    }

    /// <summary>
    /// 释放所有可按下的功能键
    /// </summary>
    public static void ReleaseFunctionKeys()
    {
        //释放所有锁定（直到获取时才被重置）的bool键：
        isJumpButtonDown = false;
        isAttackButtonDown = false;
        isThrowButtonDown = false;
    }

    public static bool GetJumpButtonDown()
    {
        if (isJumpButtonDown)
        {
            //标记被取过的
            isJumpButtonDown = false;
            return true;
        }
        else
        {
#if  UNITY_EDITOR|| UNITY_STANDALONE
            //PC还能再看看能不能按按键
            return Input.GetButton("Jump");
#else
        return false;
#endif
        }
    }

    /// <summary>
    /// 是否按下了攻击键
    /// </summary>
    /// <returns></returns>
    public static bool GetAttackButtonDown()
    {
        if (isAttackButtonDown)
        {
            isAttackButtonDown = false;
            return true;
        }
        else
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetKeyDown(KeyCode.J))
            {
                return true;
            }
#endif
            return false;
        }
    }

    /// <summary>
    /// 是否按下了投掷键
    /// </summary>
    public static bool GetThrowButtonDown()
    {
        if (isThrowButtonDown)
        {
            //标记被取过的
            isThrowButtonDown = false;
            return true;
        }
        else
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetKeyDown(KeyCode.E))
            {
                return true;
            }
#endif
            return false;
        }
    }
    #endregion

    #region --鼠标或者Touch输入
    //--预判用--：#if UNITY_EDITOR || UNITY_STANDALONE

    
    /// <summary>
    /// 触摸或鼠标按下。（默认鼠标判断左键）
    /// </summary>
    /// <param name="mouseId">0=左键，1=右键，2=中键.对于触控时无参数影响</param>
    public static bool GetMouseDown(int mouseId = 0)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return Input.GetMouseButtonDown(mouseId);
#else

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            return true;
        }
        return false;
#endif


    }

    /// <summary>
    /// 触摸或鼠标按下
    /// </summary>
    /// <param name="mouseId">0=左键，1=右键，2=中键.对于触控时无参数影响</param>
    public static bool GetMouseUp(int mouseId=0)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return Input.GetMouseButtonUp(0);
        
#else

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            return true;
        }
        return false;
#endif


    }


    public static bool IsMouseOnUI()
    {
        if (
#if UNITY_STANDALONE || UNITY_EDITOR
EventSystem.current.IsPointerOverGameObject() == false//false即为在物体上而非UI上
#else
            Input.touchCount==1
            && EventSystem.current.IsPointerOverGameObject(0) == false//false即为在物体上而非UI上
#endif
            )
        {
            //在物体上
            return false;
        }
        else
        {
            return true;
        }
    }

    [System.Obsolete]
    /// <summary>
    /// 获得在鼠标位置的obj，弃用
    /// </summary>
    /// <seealso cref="IsMouseOnObj"/>
    public static bool GetObjOnMouse(out Transform obj)
    {
        return IsMouseOnObj(out obj);
    }
    
    /// <summary>
    /// 获得鼠标当前所指向的物体
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsMouseOnObj(out Transform obj)
    {
        obj = null;
        if (
#if UNITY_STANDALONE || UNITY_EDITOR
EventSystem.current.IsPointerOverGameObject() == false//false即为非UI
#else
            Input.touchCount==1
            && EventSystem.current.IsPointerOverGameObject(0) == false//false即为非UI
#endif
            )
        {
            

            Camera cam = Camera.main;
            Ray ray = cam.ScreenPointToRay(mousePosition);
            RaycastHit hit;
            //在物体上:
            if (Physics.Raycast(ray, out hit))
            {
                obj = hit.transform;

                return true;
            }
            else
            {
                return false;
            }
            
        }
        else
        {
            
            //在ui上：
            return false;
        }
    }

    
    /// <summary>
    /// 触摸或鼠标左键正在按住
    /// </summary>
    public static bool GetMouse()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return Input.GetMouseButton(0);

#else
        return Input.touchCount > 0;
#endif
    }

    /// <summary>
    /// 获得触摸或鼠标位置
    /// </summary>
    public static Vector3 mousePosition
    {
        get
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return Input.mousePosition;
#else
                if (Input.touchCount > 0)
                    return Input.GetTouch(0).position;
                else return Vector3.zero;
#endif
        }
    }

    #region 移动端缩放/移动空白地方操作的静态函数

    static float m_moveViewSpeed = 600F;
    static Vector2 currTouchPos = Vector2.zero;                        //当前触摸/鼠标的位置
    static Vector2 lastTouchPos = Vector2.zero;                        //上一次鼠标/触摸的位置
    static Vector2 secTouchLastTouchPos = Vector2.zero;        //第二根手指头的上一轮的位置
    /// <summary>
    /// PC Only:是否移动了视图（PC：鼠标右键/中键移动）
    /// </summary>
    /// <param name="moveViewDelta">前后移动的差值</param>
    /// <returns>是否移动了</returns>
    public static bool GetMouseDragDelta(out Vector2 moveViewDelta)
    {

        moveViewDelta = Vector2.zero;
        
        #region 测试：是否用鼠标中键进行了移动视图
#if UNITY_STANDALONE || UNITY_EDITOR
        if (Input.GetMouseButtonDown(2) || Input.GetMouseButtonDown(1))
        //第一次的按下
        {
            lastTouchPos = Input.mousePosition;
            //返回无位置移动
            return false;
        }
        else if (Input.GetMouseButton(2) || Input.GetMouseButton(1))                //中键按下且拖拽
        {

            //获得距离差：
            Vector2 mousePos = Input.mousePosition;
            moveViewDelta = lastTouchPos - mousePos;

            if (moveViewDelta.SqrMagnitude() > 1)
            {
                //接受此次的距离差
                lastTouchPos = mousePos;
                return true;
            }
        }

#endif
        #endregion
        return false;
    }

    //#if MOBILE_INPUT
    private static bool beginDragScale = false;                                //用于标记第一帧开始控制时
    private static float lastTouchDistance = 0;                                        //上一帧间的拖拽距离
    private static float distanceNoiseDelta = 64;                                      //距离噪点64pixel以内，拒绝抖动
    private static bool m_acceptControll = false;                                  //是否接受控制
    /// <summary>
    /// 提供判断触摸屏两指缩放操作：返回为是否有两指拖拽操作
    /// </summary>
    /// <param name="scaleFactor">两指拖拽的距离差</param>
    /// <returns>是否有两指拖拽操作</returns>
    public static bool HasScrollViewOp(out float scaleFactor)
    {
        scaleFactor = 0;

#if UNITY_STANDALONE || UNITY_EDITOR
        if (EventSystem.current.IsPointerOverGameObject() == false)
        {
            //PC时，通过滚轮取得缩放值
            scaleFactor = Input.mouseScrollDelta.y * 3;
            return Mathf.Abs(scaleFactor) > 0.1F;
        }
        else
        {
            return false;
        }
#elif MOBILE_INPUT
//Mobile时，通过两根手指间的缩放距离取得缩放：
if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
{
    //开始按下的刹那
    if (EventSystem.current.IsPointerOverGameObject(0) == false)
    {
        //若是指尖触及
        m_acceptControll = true;                                //Not UI
    }
    else
    {
        //还是彷徨
        m_acceptControll = false;                               //Touching UI
        return false;
    }
}


if (m_acceptControll && Input.touchCount >= 2)
{
        #region 指间拖拽了那时轻狂

    if (!beginDragScale)
    {
        //The first status: Record the touches
        lastTouchDistance = Vector2.Distance(
                Input.GetTouch(0).position, Input.GetTouch(1).position);        //Sqrt of Magnitude

        beginDragScale = true;
    }
    else
    {
        //Draging...This is Next Frame:
        float currDistance = Vector2.Distance(
                Input.GetTouch(0).position, Input.GetTouch(1).position);         //Sqrt of Magnitude

        float delta = currDistance - lastTouchDistance;
        if (Mathf.Abs(delta) > distanceNoiseDelta)
        {
            //Accept change:
            scaleFactor = delta*0.1F;

            //Recaculate next.
            lastTouchDistance = currDistance;

            return true;
        }

    }
    #endregion
}
else
{
    //却放纵了韶华
    beginDragScale = false;
}

return false;
#endif
    }

    //#endif
    #endregion


    /// <summary>
    /// 判断当前鼠标是否在游戏物体上。无则为Null， 否则返回GO的Transform
    /// </summary>
    public static bool HasMouseOnGameObject(out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(MyInput.mousePosition);

        //检测
        if (Physics.Raycast(ray, out hit))
        {
            //Get到物体
            return true;
        }

        //--什么都没碰到，则返回Null
        return false;
    }
    #endregion
    //--------------------------------------------------------------------------------------------------------------

    void OnEnable()
    {
        if (EventSystem.current == null)
        {
            Debug.LogWarning("NO Event system listener was added.");
        }

        m_rectTransform = GetComponent<RectTransform>();
        m_StartAnchoredPos = m_rectTransform.anchoredPosition;
        moveRangeParent = m_rectTransform.parent.GetComponent<RectTransform>();

        //计算可移动半径范围：
        //--内圆Radius：
        float stickRadius = m_rectTransform.rect.width * 0.5F;
        //--外圆Radius：
        float moveRadius = moveRangeParent.rect.width * 0.5F;
        //--可移动范围
        MoveRadiusRange = (int)(moveRadius - stickRadius);

        //获得按钮
        Button[] buttons = GetComponentInParent<Canvas>().GetComponentsInChildren<Button>();

        if (buttons.Length > m_idBtnJump)
        {
            //注册跳JumpButton：
            buttons[m_idBtnJump].onClick.AddListener(() =>
            {
                isJumpButtonDown = true;
            });
        }

        if (buttons.Length > m_idBtnAttack)
        {
            //注册攻击按钮Attack
            buttons[m_idBtnAttack].onClick.AddListener(() =>
            {
                isAttackButtonDown = true;
            });
        }

        if (buttons.Length > m_idBtnThrow)
        {
            //注册投掷互动ButtonE
            buttons[m_idBtnThrow].onClick.AddListener(() =>
            {
                isThrowButtonDown = true;
            });
        }



    }

    /// <summary>
    /// 传输更新虚拟轴数据到输入管理中
    /// </summary>
    void UpdateVirtualAxes(Vector2 value)
    {
        axisVector = m_StartAnchoredPos - value;
        //Debug.Log("delta:" + delta);
        axisVector.y = -axisVector.y;
        axisVector /= MoveRadiusRange;
        if (m_UseX)
        {
            axisVector.x = -axisVector.x;
        }
    }


    //继承函数：Drag时触发
    public void OnDrag(PointerEventData data)
    {

        //标记有移动；
        isAnyVirtualAxisDown = true;

        //新计算方法：
        //获得移动差值
        Vector2 posDelta = data.position - m_StartDragPos;

        //如果不是双向可移动，去掉特定轴
        if (!m_UseX) posDelta.x = 0;
        if (!m_UseY) posDelta.y = 0;

        //差值限制，利用等比三角形：差值过大时，外直角边与目标直角边成比例；
        float r1 = MoveRadiusRange;
        if (Vector2.SqrMagnitude(posDelta) > r1 * r1)
        {
            //--首先去掉0值、近似0情况：
            float absX = Mathf.Abs(posDelta.x);
            float absY = Mathf.Abs(posDelta.y);

            if (absX < 0.01F)
            {
                posDelta.x = 0;
                posDelta.y = r1 * (posDelta.y > 0 ? 1 : -1);
            }
            else if (absY < 0.01F)
            {
                posDelta.x = r1 * (posDelta.x > 0 ? 1 : -1);
                posDelta.y = 0;
            }
            else
            {
                //三角形趋于X边比值较重（超出r的情况
                float rPos = posDelta.magnitude;
                //--使用等比三角的公式：x?/xPos=r1/rPos
                posDelta.x = r1 / rPos * posDelta.x;
                posDelta.y = r1 / rPos * posDelta.y;

            }
        }

        Vector2 newPos = m_StartAnchoredPos + posDelta;
        //否则予以移动：
        //--更新移动视图
        m_rectTransform.anchoredPosition = newPos;

        //更新轴数据：
        UpdateVirtualAxes(m_rectTransform.anchoredPosition);
    }


    public void OnPointerUp(PointerEventData data)
    {
        //标记移动虚拟轴结束：
        isAnyVirtualAxisDown = false;

        m_rectTransform.anchoredPosition = m_StartAnchoredPos;
        UpdateVirtualAxes(m_StartAnchoredPos);
    }


    public void OnPointerDown(PointerEventData data)
    {
        isAnyVirtualAxisDown = true;
        //开始拖拽时记录鼠标位置
        m_StartDragPos = data.position;
    }

}

