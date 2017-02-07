using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;
using UnityEngine.EventSystems;

using Game.UI.Component;
using Game.UI.Inner;

namespace Game.UI
{
    /// <summary>
    /// 提供各种UI操作中常用的函数
    /// </summary>
	public class GameUIFuncs 
	{
        /// <summary>
        /// 将UI挂载到目标物体上，且符合其缩放规则
        /// </summary>
        public static void AttachRectTrans(Transform srcRectTrans, Transform toTarget)
        {
            srcRectTrans.SetParent(toTarget, false);
        }

        #region UI资源创建
        /// <summary>
        /// 创建一个UI物体
        /// </summary>
        public static RectTransform CreateUIObj(string objName)
        {
            GameObject go = new GameObject();
            go.name = objName;
            return go.AddComponent<RectTransform>();
        }

        /// <summary>
        /// 创建一个空白的Panel
        /// </summary>
        public static RectTransform CreatePanelPlain(string objName="panel")
        {   
                return CreateUIObj(objName);   
        }

        /// <summary>
        /// 创建一个Panel（挂载了背景Img）
        /// </summary>
        public static RectTransform CreatePanel(string objName, float alpha = 0.5F,float r = 1F,float g = 1F,float b = 1F)
        {
            Image img = CreateImage(objName, GetImg(GetImageType.UISprite));
            
            //有不透明度的设置需求
            Color col = new Color(r, g, b, alpha);
            
            img.color = col;

            return img.rectTransform;
        }

        public static Text CreateText(string strText, RectTransform attachToParent = null ,TextAnchor alignType = TextAnchor.MiddleLeft,bool isBestFit = true)
        {
            RectTransform rectTrans = CreateUIObj("Text");
            Text text = rectTrans.gameObject.AddComponent<Text>();
            text.text = strText;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.resizeTextMaxSize = 30;

            //Attach if need:
            if (attachToParent != null)
            {
                AttachRectTrans(rectTrans, attachToParent);            
            }

            //--默认自动大小
            text.resizeTextForBestFit = isBestFit;
            //--设置字体颜色大小
            text.color = Color.black;

            //----Font布局：
            text.alignment = alignType;
            return text;
        }

        /// <summary>
        /// 创建Text，同时指定位置和Size
        /// </summary>
        public static Text CreateText(string strText, Vector2 percentPos, Vector2 percentSize, RectTransform attachToParent = null, TextAnchor alignType = TextAnchor.MiddleLeft, bool isBestFit = true)
        {
            Text txt = CreateText(strText, attachToParent, alignType, isBestFit);
            SetRectTransSize(txt.rectTransform, percentPos, percentSize);
            return txt;
        }

        public static Image CreateImage()
        {
            return CreateImage("Image", GetImg(GetImageType.UISprite));
        }

        public static Image CreateImage(Sprite sp,bool fullFillParent = false, Transform attachToTransform = null)
        {
            return CreateImage("Image", sp,fullFillParent,attachToTransform);
        }

        public static Image CreateImage(string objName)
        {
            return CreateImage("Image", GetImg(GetImageType.UISprite));
        }

        public static Image CreateImage(string objName,Sprite sprite ,
             bool fullFillParent = false, Transform attachToTransform = null, Image.Type imgType = Image.Type.Sliced)
        {
            RectTransform innerContent = GameUIFuncs.CreateUIObj(objName);
            Image img = innerContent.gameObject.AddComponent<Image>();
            img.sprite = sprite;
            img.type = imgType;
            
            if (fullFillParent)
            {
                SetRectTransFullFilled(innerContent);
            }

            if (attachToTransform != null)
            {
                AttachRectTrans(innerContent, attachToTransform);
            }
            return img;
        }

        /// <summary>
        /// 创建一个不可交互的Slider
        /// </summary>
        /// <param name="sliderName"></param>
        /// <returns></returns>
        public static Slider CreateSlider(string sliderName = "Slider")
        {
            return CreateSlider(sliderName, Color.white, Color.white);
        }

        /// <summary>
        /// 创建一个滑条，但是默认不推荐用于创建可交互滑条(该slider交互不美观)
        /// </summary>
        public static Slider CreateSlider(string sliderName,Color backgroundColor,Color foregroundColor,int sliderWidth = 128,int sliderHeight = 16, bool isControllable = false,bool addOutLineEffect = true)
        {
            //1.总体Parent
            RectTransform rect_sliderParent = CreateUIObj(sliderName);
            //--设置总体大小：（坐标系大小）
            SetRectTransSize(rect_sliderParent, 0, 0, sliderWidth, sliderHeight, BaseAnchorType.CenterVertHorz);
            //--attach生成物体，并生成Slider
            Slider sliderParent = rect_sliderParent.gameObject.AddComponent<Slider>();

            //2.创建背景，也就是Slider背景槽
            Image imgBackground = CreateImage("background", GetImg(GetImageType.UIBackground),
                                    true);                                       //并将之FullFill 

            //--设置对应的背景槽颜色
            imgBackground.color = backgroundColor;
            
            //--Attach to parent
            AttachRectTrans(imgBackground.rectTransform, rect_sliderParent);
            

            //3.前景槽，也就是Slider具体值显示部分
            //--前景槽父物体：
            RectTransform rect_fillArea = CreateUIObj("FillArea");
            SetRectTransFullFilled(rect_fillArea);
            AttachRectTrans(rect_fillArea, rect_sliderParent);

            //--前景槽子物体：图片创建
            Image imgForeground = CreateImage(GetImg(GetImageType.UISprite),
                                            true,                               //FullFill 父物体
                                            rect_fillArea);                     //attach到该物体上
            imgForeground.color = foregroundColor;

            //--slider的填充区域指定：
            sliderParent.fillRect = rect_fillArea;
            
            //4.Handler，也就是Slider的滑块
            //--可控制滑块时才创建Handler：
            if (isControllable)
            {
                //--创建Handler的父物体:
                RectTransform handlerParent = CreateUIObj("Handler");
                SetRectTransFullFilled(handlerParent);

                //--获得子物体中的Image,并挂载入父物体：
                Image imgHandler = CreateImage("Handler", GetImg(GetImageType.Knob), false, handlerParent, Image.Type.Simple);

                //--计算handler控制块的大小
                int handlerSize = (int)(sliderHeight * 1.2F);
                //--Handler大小改变：
                SetRectTransSize(imgHandler.rectTransform, 0, 0, handlerSize, handlerSize, BaseAnchorType.CenterVertHorz);

                //--将handlerParentAttach回Slider
                AttachRectTrans(handlerParent.transform, rect_sliderParent);

                //--slider设置：
                sliderParent.handleRect = imgHandler.rectTransform;
                //--变色属性
                sliderParent.targetGraphic = imgHandler;

            }
            else
            {
                //--无滑块变色：
                sliderParent.transition = Selectable.Transition.None;
            }
            
            //--是否可交互，可控制
            sliderParent.interactable = isControllable;

            //5.其他UI效果
            if (addOutLineEffect)
            {
                AddEffect(imgBackground.gameObject, UIEffectType.OutLine);

            }
            return sliderParent;
        }
        

        public static Button CreateButton(string btnText,CallbackFunc onClick=null)
        {
            return CreateButton(null, btnText, onClick);
        }

        public static Button CreateButton(Sprite btnSprite = null, string strBtnText = null,CallbackFunc cbOnClick = null)
        {
            //--创建Obj
            RectTransform buttonTrans =  CreateImage("Btn_Yes", GetImg(GetImageType.Button)).rectTransform;
            
            //--加入Button
            Button button = buttonTrans.gameObject.AddComponent<Button>();

            //--设置回调
            if (cbOnClick != null)
            {
                button.onClick.AddListener(() =>
                {
                    cbOnClick();
                });
            }

            //--如果有另外的Img，则为Button添加Img，否则添加Text文字
            #region 如果有另外的Img，则为Button添加Img，否则添加Text文字

            RectTransform innerContent;
            if (btnSprite != null)
            {    
                //Add img obj:
                innerContent = CreateImage(btnSprite).rectTransform;
            }
            else
            {
                //Add text obj:
                innerContent = CreateText(strBtnText,null, TextAnchor.MiddleCenter).rectTransform;
            }

            //--设置填满button
            GameUIFuncs.SetRectTransFullFilled(innerContent);
            //Attach to btn:
            GameUIFuncs.AttachRectTrans(innerContent, buttonTrans);
            
            #endregion

            return button;
        }

        /// <summary>
        /// 设置Recttrans，令之填满画布
        /// </summary>
        public static void SetRectTransFullFilled(RectTransform rectTrans)
        {
            rectTrans.anchorMin = Vector2.zero;
            rectTrans.anchorMax = Vector2.one;
            rectTrans.position = Vector2.zero;

            //Anchors在同一点的时候，sizeDelta相当于设置长宽，但是Anchors不在同一点时，表示的只是比Anchors矩形大或者小多少。
            rectTrans.sizeDelta = Vector2.zero;
        }

        /// <summary>
        /// 设置UI的位置，大小（占屏幕的百分比为单位）
        /// </summary>
        /// <param name="percentPos">位置，相对屏幕的位置</param>
        /// <param name="percentSize">大小，相对屏幕的大小</param>
        public static void SetRectTransSize(RectTransform rectTrans, Vector2 percentPos,Vector2 percentSize)
        {
            SetRectTransSize(rectTrans, percentPos.x, percentPos.y, percentSize.x, percentSize.y);
        }

        public enum BaseAnchorType{
            /// <summary>
            /// 以UI的中心点作为锚点
            /// </summary>
            CenterVertHorz,

            /// <summary>
            /// 基准点以Fill四个边角为准，填充父物体
            /// </summary>
            FillCorner,
        };
        /// <summary>
        /// 设置UI的位置，大小（占屏幕的百分比为单位）
        /// </summary>
        /// <param name="percentPosX">位置，相对屏幕的位置</param>
        /// <param name="percentSizeX">大小，相对屏幕的大小</param>
        /// <param name="anchorType">参考大小类型</param>
        public static void SetRectTransSize(RectTransform rectTrans, float percentPosX, float percentPosY,
                                                                                                     float percentSizeX, float percentSizeY,
                                                                BaseAnchorType anchorType = BaseAnchorType.FillCorner)
        {
            //锚点位置计算：
            #region 锚点位置计算：
            switch (anchorType)
            {
                //    //--设置锚点为左下角
                //    rectTrans.anchorMin = Vector2.zero;
                //    rectTrans.anchorMax = Vector2.zero;
                //    rectTrans.pivot = Vector2.zero;

                //    Vector2 pos = Vector2.zero, size = Vector2.zero;

                //    //--获取画布大小：
                //    Vector2 parentSize = DialogCanvas.canvasResolution;
                //    float scrWidth = parentSize.x, scrHeight = parentSize.y;
            

                //    //--计算位置：
                //    pos.x = scrWidth * percentPosX;
                //    pos.y = scrHeight * percentPosY;
                //    //--计算大小：
                //    size.x = scrWidth * percentSizeX;
                //    size.y = scrHeight * percentSizeY;

                //    //--设置位置、大小
                //    rectTrans.position = pos;
                //    rectTrans.sizeDelta = size;
                //    break;

                case BaseAnchorType.CenterVertHorz:
                    //--锚点为中心点
                    rectTrans.anchorMin = new Vector2(0.5F, 0.5F);
                    rectTrans.anchorMax = new Vector2(0.5F, 0.5F);
                    rectTrans.pivot = new Vector2(0.5F, 0.5F);

                    //--Size
                    rectTrans.sizeDelta = new Vector2(percentSizeX, percentSizeY);
                    //--Position
                    rectTrans.position = new Vector3(percentPosX, percentPosY, 0);
                    break;

                case BaseAnchorType.FillCorner:
                    //锚点为填充父物体型的
                    rectTrans.anchorMin = new Vector2(percentPosX, percentPosY);
                    rectTrans.anchorMax = new Vector2(percentPosX+percentSizeX,percentPosY+percentSizeY);
                    rectTrans.pivot = Vector2.zero;

                    //--SizeDelta保持为0即可：
                    rectTrans.sizeDelta = Vector2.zero;
                    break;
            }
            #endregion

        }

        ///<summary>加上相对位移的位置设置</summary>
        /// <param name="deltaLeft"></param>
        /// <param name="deltaRight"></param>
        /// <param name="deltaUp"></param>
        /// <param name="deltaDown"></param>
        public static void SetRectTransSize(RectTransform rectTrans, float percentPosX, float percentPosY,
                                                                                                     float percentSizeX, float percentSizeY,
                                                                                                     int deltaLeft, int deltaRight,
                                                                                                     int deltaUp, int deltaDown,
                                                                BaseAnchorType anchorType = BaseAnchorType.FillCorner)
        {
            #region 锚点位置计算：
            switch (anchorType)
            {
                //    //--设置锚点为左下角
                //    rectTrans.anchorMin = Vector2.zero;
                //    rectTrans.anchorMax = Vector2.zero;
                //    rectTrans.pivot = Vector2.zero;

                //    Vector2 pos = Vector2.zero, size = Vector2.zero;

                //    //--获取画布大小：
                //    Vector2 parentSize = DialogCanvas.canvasResolution;
                //    float scrWidth = parentSize.x, scrHeight = parentSize.y;


                //    //--计算位置：
                //    pos.x = scrWidth * percentPosX;
                //    pos.y = scrHeight * percentPosY;
                //    //--计算大小：
                //    size.x = scrWidth * percentSizeX;
                //    size.y = scrHeight * percentSizeY;

                //    //--设置位置、大小
                //    rectTrans.position = pos;
                //    rectTrans.sizeDelta = size;
                //    break;

                ////case AnchorType.CenterVertHorz:
                ////    //--锚点为中心点
                ////    rectTrans.anchorMin = new Vector2(0.5F, 0.5F);
                ////    rectTrans.anchorMax = new Vector2(0.5F, 0.5F);
                ////    rectTrans.pivot = new Vector2(0.5F, 0.5F);
                ////    break;

                case BaseAnchorType.FillCorner:
                    //锚点为填充父物体型的
                    rectTrans.anchorMin = new Vector2(percentPosX, percentPosY);
                    rectTrans.anchorMax = new Vector2(percentPosX + percentSizeX, percentPosY + percentSizeY);
                    rectTrans.pivot = Vector2.zero;
                    
                    //--SizeDelta保持为0即可，用于设置大小
                    rectTrans.sizeDelta = Vector2.zero;

                    //--设置大小后的：偏移值
                    rectTrans.offsetMin = new Vector2(deltaLeft, deltaDown);
                    rectTrans.offsetMax = new Vector2(deltaRight, deltaUp);
                    break;
            }
            #endregion
        }

        /// <summary>
        /// 设置UI可拖动
        /// </summary>
        public static void SetUIDragable(RectTransform rectTrans)
        {
            if (rectTrans.GetComponent<GameUIDragable>() == null)
            {
                rectTrans.gameObject.AddComponent<GameUIDragable>();
            }
        }

        /// <summary>
        /// 设置UI可被点击，并设置其回调函数
        /// </summary>
        public static void SetUIClickable(RectTransform uiRectTrans,CallbackFunc onClick, CallbackFunc onClickeDown= null,CallbackFunc onClickUp=null)
        {
            GameUIClickable clickable = uiRectTrans.GetComponent<GameUIClickable>();
            if (clickable == null)
            {
                clickable = uiRectTrans.gameObject.AddComponent<GameUIClickable>();
            }

            //--设置回调：
            clickable.SetOnClickListener(onClick, onClickeDown,onClickUp);
        }

        public static void SetUISubmitListener(InputField input, CallbackFunc onSubmit)
        {
            input.onEndEdit.AddListener(
                (string msg) =>
                {
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
						//Debug.Log ("enter");
                        if(onSubmit!=null)
						{
                            onSubmit();
						}
                    }
                });
        }

        /// <summary>
        /// Destroy RectTransform
        /// </summary>
        public static void DestroyRectTrans(RectTransform trans)
        {
            GameObject.Destroy(trans.gameObject);
        }

        /// <summary>
        /// 删除parent下的所有子节点
        /// </summary>
        /// <param name="startFrom">指定从一个特定位置开始往后删除</param>
        public static void RemoveAllChildren(Transform parent, int startFrom = 0)
        {
            int childCount = parent.childCount;
            for (int i = startFrom; i < childCount; i++)
            {
                GameObject.Destroy(parent.GetChild(i).gameObject);
            }
        }

        #endregion

        #region UI 编辑器扩展

        #if UNITY_EDITOR

        //热键：%-CTRL #Shift & ALT
        [UnityEditor.MenuItem("GameObject/UI/AddPanelFillH %#h",false,14)]  
        /// <summary>
        /// 添加一个水平填充的Panel
        /// </summary>
        private static void AddPanelFillH()
        {
            RectTransform newOne = AddPanelFill(UnityEditor.Selection.activeGameObject.transform,
                LayoutDirection.Horizontal);

            //向编辑器注册物体的生成：
            UnityEditor.Undo.RegisterCreatedObjectUndo(newOne.gameObject, "panel obj");
        }

        /// <summary>
        /// 添加一个垂直布局的子Panel
        /// </summary>
        [UnityEditor.MenuItem("GameObject/UI/AddPanelFillV %#v", false, 15)]
        private static void AddPanelFillV()
        {
            RectTransform newOne = AddPanelFill(UnityEditor.Selection.activeGameObject.transform,
                LayoutDirection.Vertical);
            UnityEditor.Undo.RegisterCreatedObjectUndo(newOne.gameObject, "panel obj");
        }

        /// <summary>
        /// 将子节点全部转换为垂直布局
        /// </summary>
        [UnityEditor.MenuItem("GameObject/UI/Edit/MakeFill_V %#&v", false)]
        private static void ChangeChildrenToFillPanelV()
        {
            GameObject currObj = UnityEditor.Selection.activeGameObject;
            ConvertChildrenPanelLayout(currObj.transform, LayoutDirection.Vertical);
        }

        /// <summary>
        /// 将子节点全部转为水平布局
        /// </summary>
        [UnityEditor.MenuItem("GameObject/UI/Edit/MakeFill_H %#&h", false)]
        private static void ChangeChildrenToFillPanelH()
        {
            GameObject currObj = UnityEditor.Selection.activeGameObject;
            ConvertChildrenPanelLayout(currObj.transform, LayoutDirection.Horizontal);
        }
        #endif

        //------------------------------------------------------------
        //                      辅助函数
        //------------------------------------------------------------
        /// <summary>
        /// 布局方向，私有
        /// </summary>
        enum LayoutDirection 
        {
            Horizontal,
            Vertical,
        }

        /// <summary>
        /// 根据参数生成自动填充布局属性的panel,并返回新生成的panel
        /// </summary>
        /// <param name="isHorizPanel">是否是水平布局的panel</param>
        private static RectTransform AddPanelFill(Transform parentTrans, LayoutDirection layoutDirection)
        {
            int childCount = parentTrans.childCount;

            //生成一个子物体

            RectTransform rtChildCreated = CreatePanel("Panel_" + childCount);
            
            //--添加入父物体中：
            AttachRectTrans(rtChildCreated, parentTrans);
            childCount++;

            //开始转换所有子物体的布局
            ConvertChildrenPanelLayout(parentTrans, layoutDirection);

            

            //返回新生成的物体
            return rtChildCreated;
        }

        private static void ConvertChildrenPanelLayout(Transform parentTrans,LayoutDirection layoutDirection)
        {
            int childCount = parentTrans.childCount;
            if (childCount <= 0)
                return;

            #region 开始遍历所有子物体，并重新设置横向占据的百分比            
            RectTransform currChildTran;
            //--计算panel 占据的百分比%，用int方便控制小数位数为2；
            int sizeDeltaRange = 100 / childCount;
            int currSizeStart = 0;                  //当前占据值起始点

            //--如果是竖行排版，则start和delta方式为逆方式：
            #region 设置vertical排版时的delta和起始位置：
            if (layoutDirection == LayoutDirection.Vertical)
            {
                sizeDeltaRange = -sizeDeltaRange;
                currSizeStart = 100;
            }
            #endregion

            for (int i = 0; i < childCount; i++)
            {
                currChildTran = parentTrans.GetChild(i).GetComponent<RectTransform>();

                //--现在anchor全部采取小数表示
                float startAnchor = currSizeStart * 0.01f;
                float endAnchor = (currSizeStart + sizeDeltaRange) * 0.01f;
                if (i == childCount - 1)
                {
                    //--最后一个anchor计算的时候，终点anchor直接为1/0
                    if (sizeDeltaRange < 0)
                        endAnchor = 0f;                 //负值增长0
                    else
                        endAnchor = 1;                  //正到1
                }

                //根据布局方向不同，设置x/y
                switch (layoutDirection)
                {
                    case LayoutDirection.Horizontal:
                        currChildTran.anchorMin = new Vector2(startAnchor, 0f);
                        currChildTran.anchorMax = new Vector2(endAnchor, 1f);
                        break;

                    case LayoutDirection.Vertical://竖直排版时需要max和min的y值颠倒，因为布局是从下往上的，而坐标相反：
                        currChildTran.anchorMin = new Vector2(0f, endAnchor);
                        currChildTran.anchorMax = new Vector2(1f, startAnchor);

                        break;
                }

                currChildTran.anchoredPosition = Vector2.zero;
                //--内部间距设置0：如果需要调整则为-10，-10
                currChildTran.sizeDelta = new Vector2(0, 0);

                //--起始点自增，或不设置
                currSizeStart += sizeDeltaRange;
            }
            #endregion

        }

#if UNITY_EDITOR
        //填充的验证项：只允许在ui物体上添加
        [UnityEditor.MenuItem("GameObject/UI/AddPanelFillH %#h", true)]
        [UnityEditor.MenuItem("GameObject/UI/AddPanelFillV %#v", true)]
        [UnityEditor.MenuItem("GameObject/UI/Edit/MakeFill_V %#&v", true)]
        [UnityEditor.MenuItem("GameObject/UI/Edit/MakeFill_H %#&h", true)]
        private static bool UIObjValidation()
        {
            return UnityEditor.Selection.activeGameObject.GetComponent<RectTransform>() != null;
        }
#endif

        #endregion

        #region 静态效果区域
        //UI物体的显示管理
        class UIObjHolder
        {
            /// <summary>
            /// 对UI的RectTransform引用
            /// </summary>
            public RectTransform objRectTransform;
            
            /// <summary>
            /// 这个UI被显示次数（为0则为无显示）
            /// </summary>
            int objShowCount = 0;

            //--增加被显示的次数
            public void AddObjShowCount()
            {
                objShowCount++;
            }

            //--减少被显示的次数
            public void SubObjShowCount()
            {
                objShowCount--;
            }

            public bool IsObjCanClose()
            {
                return objShowCount <= 0;
            }

            public bool IsObjShowing()
            {
                return objShowCount > 0;
            }
        }

        static List<UIObjHolder> g_listUIObjShowHolder = new List<UIObjHolder>();
        //------------------------------------------------
        //--        UI show and hide
        //------------------------------------------------
        //--管理部分
        static UIObjHolder HelperFindUIObjInManageList(List<UIObjHolder> list, RectTransform uiRect)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].objRectTransform == uiRect)
                {
                    return g_listUIObjShowHolder[i];
                }
            }

            return null;
        }

        /// <summary>
        /// 显示这个UI，在多协程使用同一个UI时可以有效管理开关
        /// </summary>
        internal static void CrtShowUI(RectTransform uiRect)
        {
            //--在管理列表中找到这个ui
            UIObjHolder objHodler = HelperFindUIObjInManageList(g_listUIObjShowHolder, uiRect);

            //--未找到，则产生一个管理类，并加入管理列表：
            if (objHodler == null)
            {
                objHodler = new UIObjHolder();
                objHodler.objRectTransform = uiRect;

            }

            //--对管理类进行+1
            objHodler.AddObjShowCount();

            //--管理类进入队列：
            g_listUIObjShowHolder.Add(objHodler);

            //--显示这个UI
            uiRect.gameObject.SetActive(true);

        }

        /// <summary>
        /// 尝试关闭这个UI，在多协程使用同一个UI时可以有效管理开关（如果还有其他协程占用，则将保持显示直到所有协程都尝试了关闭UI）
        /// </summary>
        internal static void CrtCloseUI(RectTransform uiRect)
        { 
            //--在管理列表找到这个UI
            UIObjHolder objHodler = HelperFindUIObjInManageList(g_listUIObjShowHolder, uiRect);

            //--减少计数
            objHodler.SubObjShowCount();

            //--探寻是否可关闭
            if (objHodler.IsObjCanClose())
            { 
                //--真正关闭：
                uiRect.gameObject.SetActive(false);
                //--删除这个管理类：
                g_listUIObjShowHolder.Remove(objHodler);
            }
        }

        /// <summary>
        /// 探查这个UI是否显示ing，在多协程使用同一个UI时可以有效管理
        /// </summary>
        internal static bool IsUIShowing(RectTransform uiRect)
        {
            UIObjHolder objHolder = HelperFindUIObjInManageList(g_listUIObjShowHolder, uiRect);
            if (objHolder == null)
            {
                return false; 
            }
            else
            {
                return objHolder.IsObjShowing(); 
            }
        }
        //--         End of UI show & hide

        public static void ChangeUILayoutElment(UIBehaviour uiObj,int height)
        {
            LayoutElement element =  uiObj.GetComponent<LayoutElement>();
            element.minHeight = element.preferredHeight = height;

        }

        public enum UIEffectType { OutLine, Shadow };
        public static void AddEffect(GameObject srcObj, UIEffectType effectType)
        {
            Color effColor = Color.black;
            effColor.a = 0.5f;
            Vector2 effectDistance = new Vector2(1, -1);
            AddEffect(srcObj, effectType, effColor, effectDistance);
        }

        public static void AddEffect(GameObject srcObj, UIEffectType effectType, Color effectColor,Vector2 effectDistance)
        { 
            Shadow eff = null;
            switch(effectType)
            {
                case UIEffectType.OutLine:
                    //--添加OutLine效果：
                    eff = srcObj.AddComponent<Outline>();
                    
                    break;

                case UIEffectType.Shadow:
                    //--添加Shadow效果
                    eff = srcObj.AddComponent<Shadow>();
                    break;
            }

            //--设置对应属性：
            eff.effectColor = effectColor;
            eff.effectDistance = effectDistance;
        }

        #endregion

        #region UI动态效果区域

        public static void PrintText(Text textToPrint, CallbackFunc onPrintOver = null)
        {
            //Start a corotine for text:
            textToPrint.StartCoroutine(IE_PrintText(textToPrint,0.1F,true,onPrintOver));
        }

        //------------------------------------------------------------------------------------------
        //                                         Private Area
        //------------------------------------------------------------------------------------------

        /// <summary>
        /// 直接让UI定位到一个场景中的游戏物体身上
        /// </summary>
        /// <param name="deltaYToTarget">与目标物体Transform在Y轴上的偏移值</param>
        public static void FollowTrans(RectTransform UITrans, Transform target, float deltaYToTarget = 0)
        {
            //Follow position:
            Vector3 tmpV3 = Camera.main.WorldToScreenPoint(target.position);
            tmpV3.y -= deltaYToTarget;
            UITrans.position = tmpV3;
        }


        /// <summary>
        /// 在一定时间内，移动一个UI到一个指定的场景物体的Transform上：比如移动一张Image到一个游戏物体上。
        /// </summary>
        public static IEnumerator IUIMoveToTransform(RectTransform trans, Transform transTarget, Vector3 moveFrom, float deltaYToTarget = 0, bool fallBack = true, float moveTime = 1.5F)
        {
            Vector3 targetPos;
            Vector3 fromPos;
            Vector3 currPos = trans.position;
            float timePass = 0;
            while (timePass < moveTime)
            {
                targetPos = Camera.main.WorldToScreenPoint(transTarget.position);
                fromPos = trans.position;

                currPos.x = Mathf.Lerp(fromPos.x, targetPos.x, timePass);
                currPos.y = Mathf.Lerp(fromPos.y, targetPos.y, timePass) - deltaYToTarget;

                trans.position = currPos;

                timePass += Time.deltaTime;
                yield return 1;
            }

            if (fallBack)
            {
                //Reset back:
                trans.position = moveFrom;
            }
        }

        /// <summary>
        /// 在一定时间内，把一个UI移动到目标V3的位置
        /// </summary>
        public static IEnumerator IMoveUIToRectTransPos(RectTransform srcRT, Vector3 targetPos, CallbackFunc onDone = null)
        {
            float timePassed = 0F;
            Vector3 v3Src = srcRT.position;
            while (timePassed <= 1)
            {
                srcRT.position = Vector3.Lerp(v3Src, targetPos, timePassed);

                timePassed += Time.deltaTime;
                yield return 1;
            }
            if (onDone != null)
            {
                onDone();
            }
        }

        /// <summary>
        /// 在一定时间内，让UI绕着Y轴旋转
        /// </summary>
        public static IEnumerator IRotateUI_Y(RectTransform rectToDo, float speed = 180, float rotateTime = 2)
        {
            while (rotateTime > 0)
            {
                rotateTime -= Time.deltaTime;
                rectToDo.Rotate(0, speed * Time.deltaTime, 0);
                yield return 1;
            }

            rectToDo.rotation = Quaternion.identity;
        }

        /// <summary>
        /// 在一定时间内，将物体平滑缩放到指定大小
        /// </summary>
        public static IEnumerator IScaleUITo(RectTransform rectToDo, Vector3 toSize, float scaleTime = 1)
        {
            Vector3 currSize = rectToDo.localScale;

            //获得目标与当前的每秒间差值，即可通过deltaTime控制缩放速度
            Vector3 deltaPerSecond = (toSize - currSize)/scaleTime;
            while (scaleTime > 0)
            {
                scaleTime -= Time.deltaTime;
                currSize = currSize + deltaPerSecond * Time.deltaTime;
                rectToDo.localScale = currSize;
                yield return 1;
            }

            rectToDo.localScale = toSize;
        }

        /// <summary>
        /// 平滑地滑动Slider到指定值
        /// </summary>
        public static IEnumerator IDoSliderBlinkTo(Slider slider, float slideTo, float blinkTime = 0.5F, CallbackFunc UpdateAction = null, CallbackFunc CallbackAction = null, float callBackWaitTime = 0)
        {
            //Outline effectOutLine = slider.GetComponentInChildren<Outline>();
            Image fillImage = slider.GetComponentsInChildren<Image>()[1];
            Color preColor = fillImage.color;
            float beforeValue = slider.value;

            Color currColor = slideTo > beforeValue ?
                Color.white :                                            //UP Color
                Color.grey;                                            //Down color


            float blinkTimeRevert = 1F / blinkTime;
            float imgAlpha = fillImage.color.a;
            while (blinkTime >= 0)
            {
                slider.value += (slideTo - beforeValue) * Time.deltaTime * blinkTimeRevert;
                currColor += (preColor - currColor) * Time.deltaTime;
                currColor.a = imgAlpha;
                fillImage.color = currColor;

                if (UpdateAction != null)
                    UpdateAction();

                blinkTime -= Time.deltaTime;
                yield return 1;
            }

            slider.value = slideTo;
            fillImage.color = preColor;
            while (callBackWaitTime > 0)
            {
                if (UpdateAction != null)
                    UpdateAction();

                callBackWaitTime -= Time.deltaTime;
                yield return 1;
            }

            if (CallbackAction != null)
                CallbackAction();

        }

        /// <summary>
        /// 让UI上下跳动一次
        /// </summary>
        /// <param name="jumpCount">跳动的次数，默认上下跳动一次。</param>
        public static IEnumerator IDoJumpUI(RectTransform rectToDo, bool recordPrePos = true, float jumpCount = 1F, CallbackFunc callBack = null)
        {

            //if ( m_isDoingUI )
            //{
            //    //新注释的Debug.Log( "too quick." );
            //    yield return null;
            //}
            //else
            //{
            //    m_isDoingUI = true;
            float doingTime = 0;
            Vector3 preLocalPos = rectToDo.localPosition;                   //记录相对位置而不是绝对位置
            //Vector3 currPos = rectToDo.position;
            float totalTimeToDo = 6.2831853F * jumpCount;               //2π=6.28..., 一个周期完毕。指定多少周期就多大咯
            float jumpHighest = 0.0047F * Screen.height;
            //--Do UI part
            do
            {
                rectToDo.Translate(0, Mathf.Sin(doingTime) * jumpHighest, 0);
                doingTime += (Time.deltaTime * 5);

                yield return 1;                                             //Wait For one frame
            } while (doingTime < totalTimeToDo);

            if (recordPrePos)
                rectToDo.localPosition = preLocalPos;
            //--done------

            //    m_isDoingUI = false;
            //}
            if (callBack != null)
            {
                callBack();
            }
        }


        private static IEnumerator IE_PrintText(Text srcText, float charDelayTime, bool clickBreak, CallbackFunc onPrintOver)
        {
            string textStr = srcText.text;
            srcText.text = "";

            int count = textStr.Length;

            #region version 1
            float timePsd = 0;
            int currWord = 1;

            while (true)
            {
                timePsd += Time.deltaTime;

                //TODO:Input时进行Break：
                if (clickBreak)
                {
                    if (MyInput.GetMouseDown())
                    {
                        currWord = count;
                    }
                }

                if (timePsd > charDelayTime)
                {
                    timePsd -= charDelayTime;

                    //开始Sub 字符
                    if (currWord <= count)
                    {
                        srcText.text = textStr.Substring(0, currWord);

                        currWord++;
                    }
                    else
                    {
                        break;
                    }
                }
                yield return 1;
            }

            #endregion

            #region verson 0
            //获得每个字符的延迟
            //WaitForSeconds eachCharWaitTime = new WaitForSeconds(charDelay);

            ////Show it one by one:
            //for (int i = 1; i <= count; i++)
            //{
            //    srcText.text = textStr.Substring(0, i);

            //    //wait for a while
            //    yield return eachCharWaitTime;
            //}
            #endregion

            //Print over:
            if (onPrintOver != null)
                onPrintOver();
        }

        #endregion

        #region UI资源区域
        enum GetImageType { Button, InputField, Knob, DownArrow, CheckMark, UIBackground, UISprite };
        private static Sprite GetImg(GetImageType imgType)
        {
            switch (imgType)
            {
                case GetImageType.Button:
                case GetImageType.UISprite:
                    return GameUISpriteProvider.g_instance.m_UISprite;


                case GetImageType.InputField:
                    return GameUISpriteProvider.g_instance.m_UIInputFieldBG;

                case GetImageType.Knob:
                    return GameUISpriteProvider.g_instance.m_UIKnob;

                case GetImageType.DownArrow:
                    return GameUISpriteProvider.g_instance.m_UIDownArrow;

                case GetImageType.CheckMark:
                    return GameUISpriteProvider.g_instance.m_UICheckMark;

                case GetImageType.UIBackground:
                    return GameUISpriteProvider.g_instance.m_UIBG;
            }

            return GameUISpriteProvider.g_instance.m_UISprite;
        }


        public static GameUISkin GetGameUISkin()
        {
            return GameUISpriteProvider.g_instance.m_gameStyleUI;
        }

        /// <summary>
        /// 获得游戏场景中的主Canvas（无干扰Canvas）
        /// </summary>
        /// <returns></returns>
        public static RectTransform GetMainGameCanvasRectTrans()
        {
            return MainGameCanvas.canvasRectTransform;
        }
        #endregion


    }

    
}