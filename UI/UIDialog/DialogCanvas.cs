using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Game.UI.Inner
{
    //游戏UI的必备Canvas
    /// <summary>
    /// 游戏对话形式的UI所需的Canvas，将导致游戏界面不可点击
    /// </summary>
	public  class DialogCanvas  
	{

        //静态属性：
        //游戏中的唯一优先画布(将导致游戏界面其他物体不可点击）
        static Canvas g_canvas;
        //画布的遮罩层：用于放置子物体
        static RectTransform g_canvasChildren;

        //画布显示优先级：
        const int C_CANVAS_PRIORITY = 999;
        //画布名字
        const string C_CANVS_NAME = "MainMenuCanvas";
        //画布Size：
        static Vector2 g_canvasSize;
        
        protected static void Init()
        {
            //初始化：
            if (g_canvas == null)
            {
                //创建一个物体，令之成为Canvas：
                //--Create obj
                GameObject go = GameUIFuncs.CreateUIObj(C_CANVS_NAME).gameObject;

                //--Attach canvas
                g_canvas = go.AddComponent<Canvas>();

                //Setting canvas:
                #region 画布自身的设置
                g_canvas.sortingOrder = C_CANVAS_PRIORITY;
                
                //--Attach canvas scaler & others
                CanvasScaler scaler = go.AddComponent<CanvasScaler>();
                go.AddComponent<GraphicRaycaster>();

                g_canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
                g_canvasSize = scaler.referenceResolution;
                #endregion

                #region 创建一个画布遮罩层：用于挡住原本界面的UI行为：
                //--创建Layer物体
                g_canvasChildren = GameUIFuncs.CreateUIObj("MaskLayer");

                //--Img用于遮挡
                Image img = g_canvasChildren.gameObject.AddComponent<Image>();
                img.color = Color.clear;
                //--填充画布
                GameUIFuncs.SetRectTransFullFilled(g_canvasChildren);
                //--挂载到Canvas上
                GameUIFuncs.AttachRectTrans(g_canvasChildren, g_canvas.transform);
                #endregion

                //--Event system:
                if (UnityEngine.EventSystems.EventSystem.current == null)
                {
                    GameObject eventSys = new GameObject("EventSystem");
                    eventSys.AddComponent<UnityEngine.EventSystems.EventSystem>();
                    eventSys.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                    eventSys.AddComponent<UnityEngine.EventSystems.TouchInputModule>();
                }

            }
        }


        //----------------------------------------------------------------------------------------
        //                                              PUBLIC PART 
        //----------------------------------------------------------------------------------------
        //public static Canvas main { get { return g_canvas; } }
        
        //返回mainCanvas的Size
        public static Vector2 canvasResolution
        {
            get { return g_canvasSize; }
        }

        //调用Canvas者的计数，如果多个调用Canvas的人，则在关闭时予以考虑：
        static int g_canvasCallerCount = 0;

        /// <summary>
        /// 显示Canvas
        /// </summary>
        public static void ShowCanvas( )
        {
            Init();

            g_canvasCallerCount++;
            g_canvas.gameObject.SetActive(true);
        }


        /// <summary>
        /// 关闭Canvas
        /// </summary>
        public static void CloseCanvas()
        {
            g_canvasCallerCount--;
            if (g_canvasCallerCount <= 0)
            {
                g_canvas.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 添加子物体控件,需要注意的是这个行为会导致画面其他部分不可点击
        /// </summary>
        public static void AddChild(RectTransform rectTrans)
        {
            //添加到Mask上：
            rectTrans.SetParent(g_canvasChildren, false);
        }


	}
}