using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Game.UI.Inner;


namespace Game.UI
{
    //TODO:让我们制作一个单例类MsgBox吧！
    //分模块，调用并Attach：Canvas；
    //Close时Destroy
    /// <summary>
    /// 使用之前需要先行挂载好UIProvider，否则无法正确显示UI图片
    /// </summary>
	public class MessageBox 
	{
        //背景图片
        static Sprite g_msgPanelImg;
        //Yes按钮图片
        static Sprite g_msgBtnYesImg;
        //No按钮图片
        static Sprite g_msgBtnNoImg;
        
        //----------------------------------------------------------------------------------------
        //                                      PUBLIC PART
        //----------------------------------------------------------------------------------------
        /// <summary>
        /// 显示消息框
        /// </summary>
        public static void ShowYesNo(string msgStr=null,CallbackFunc cbOnYes=null,CallbackFunc cbOnNo = null , 
            string btnYesStr="Yes",string btnNoStr="No")
        {
            //显示Canvas：
            DialogCanvas.ShowCanvas();

            
            #region 创建MsgBox对话框的UI
            /*
            *                              ------------------------
            *                              |                               |
            *                              |      yes/no to?   <-- |---------textObj
            *                              |                               |
            *                              |                               |    <-----msgBoxPanel
            *                              |       [YES]     [NO]<- |---------buttons
            *                              ------------------------
            */
            //Panel设置：
            //--创建panel
            RectTransform m_msgBoxPanelTrans = GameUIFuncs.CreatePanel("MsgPanel");

            //--设置为填满画布的的Anchor：
            GameUIFuncs.SetRectTransSize(m_msgBoxPanelTrans, 0.3F, 0.2F, 0.4F, 0.6F);

            //--设置可拖动：
            GameUIFuncs.SetUIDragable(m_msgBoxPanelTrans);

            //--加入Canvas中：
            DialogCanvas.AddChild(m_msgBoxPanelTrans);

            //--创建Panel子物体：
            //--消息文字框
            //----创建TextObj
            Text msgText = GameUIFuncs.CreateText(msgStr);
            RectTransform textObjRectTrans = msgText.rectTransform;

            //----消息框的位置和大小：
            GameUIFuncs.SetRectTransSize(textObjRectTrans, 0.1F, 0.2F, 0.8F, 0.7F, GameUIFuncs.BaseAnchorType.FillCorner);

            //----挂载UI到Panel上
            GameUIFuncs.AttachRectTrans(textObjRectTrans, m_msgBoxPanelTrans);

            //Yes、No都要有关闭显示完成，即可OK
            //--按钮Yes：
            //----创建ButtonYes,并且设置回调
                Button btnYes = GameUIFuncs.CreateButton(btnYesStr,
                () =>
                {
                    if (cbOnYes != null)
                        cbOnYes();

                    //Destro Panel：
                    GameUIFuncs.DestroyRectTrans(m_msgBoxPanelTrans);

                    //Close this canvas :
                    DialogCanvas.CloseCanvas();
                }
                );
            RectTransform btnYesTrans = btnYes.GetComponent<RectTransform>();

            //----设置其位置：
            GameUIFuncs.SetRectTransSize(btnYesTrans, 0.1F, 0.1F, 0.4F, 0.1F, GameUIFuncs.BaseAnchorType.FillCorner);

            //-----挂载
            GameUIFuncs.AttachRectTrans(btnYesTrans, m_msgBoxPanelTrans);

            //--按钮No：
            if (cbOnNo != null)
            {
                Button btnNo = GameUIFuncs.CreateButton(btnNoStr,
                () =>
                {
                    cbOnNo();

                    //Destroy panel:
                    GameUIFuncs.DestroyRectTrans(m_msgBoxPanelTrans);

                    //Close canvas:
                    DialogCanvas.CloseCanvas();
                });

                RectTransform btnNoTrans = btnNo.GetComponent<RectTransform>();

                //设置其位置：
                GameUIFuncs.SetRectTransSize(btnNoTrans, 0.5F, 0.1F, 0.4F, 0.1F, GameUIFuncs.BaseAnchorType.FillCorner);

                //挂载
                GameUIFuncs.AttachRectTrans(btnNoTrans, m_msgBoxPanelTrans);
            }
            #endregion
            
            
        }

        /// <summary>
        /// 显示提示信息，并且在一段时间后自动消失
        /// </summary>
        public static void ShowNotificationMsg(string notifyMsg, float stayTime = 5F,float percentX =0.5F,float percentY = 0.5F)
        {
            //Canvas显示
            DialogCanvas.ShowCanvas();

            //创建一个Notification Text（居中）
            Text notifyText= GameUIFuncs.CreateText("notifyPanel with msg :"+notifyMsg,null, TextAnchor.MiddleCenter);
            RectTransform notifyTextTrans = notifyText.rectTransform;

            //--设置位置，和文本框宽高度：
            //----计算消息是预设7个文字的多少倍
            int lenTimes = notifyMsg.Length  / 7 +1;
            float height , width = 0.1F * lenTimes;//TODO:百分比大于1时怎么办，
            height = width;
            GameUIFuncs.SetRectTransSize(notifyTextTrans, percentX, percentY, width, height);//TODO:设置居中的位置

            //--自动调整文字大小：
            notifyText.resizeTextForBestFit = true;

            //--挂载到Canvas上：
            DialogCanvas.AddChild(notifyTextTrans);

        }

        /// <summary>
        /// 在对话框中显示对话的结构体
        /// </summary>
        public struct DialogMsg
        {
            public string speakerName;
            public string msg;
            public Sprite speakerImg;
        }
        
        /// <summary>
        /// 显示对话信息（RPG用对话）
        /// </summary>
        /// <param name="speakerName">讲话者的名字，会显示在边框左上角</param>
        /// <param name="onClick">显示完成后，点击时的事件</param>
        public static void ShowDialogMsg(string dialogMsg,string speakerName,CallbackFunc onClick = null, CallbackFunc onShowedOver = null)
        {
            #region 设置Dialog
            //Give me a canvas 
            DialogCanvas.ShowCanvas();

            //Give me a click-able panel that will full fill the canvas:
            RectTransform clickBgPanel = GameUIFuncs.CreatePanel("PanelBG",0);
            GameUIFuncs.SetRectTransFullFilled(clickBgPanel);

            //--Add to canvas:
            DialogCanvas.AddChild(clickBgPanel);
            
            //Give me a panel:
            RectTransform dialogPanel = GameUIFuncs.CreatePanel("DialogPanel", 0.8f);

            //Set the panel:(带偏移值)
            GameUIFuncs.SetRectTransSize(dialogPanel, 0, 0, 1F, 0.3F,3,-3,0,3);

            //Add it to bg panel:
            GameUIFuncs.AttachRectTrans(dialogPanel, clickBgPanel);

            //Give me an image:
            Image nameImg = GameUIFuncs.CreateImage();
            GameUIFuncs.AttachRectTrans(nameImg.rectTransform, dialogPanel);
            //Set the rect:
            GameUIFuncs.SetRectTransSize(nameImg.rectTransform, 0, 0.98F, 0.2F, 0.2F);

            //Give me a text for name & attach it to img:
            Text nameText = GameUIFuncs.CreateText(speakerName,nameImg.rectTransform, TextAnchor.MiddleCenter);

            //Set the rect:
            GameUIFuncs.SetRectTransSize(nameText.rectTransform, 0,0, 1, 1);

            //CONTENT Text
            Text contentText = GameUIFuncs.CreateText(dialogMsg,dialogPanel, TextAnchor.UpperLeft);
            //Set the rect:
            GameUIFuncs.SetRectTransSize(contentText.rectTransform,0,0,1,1F,20,-20,-20,10);
            #endregion

            #region 对应的事件设置：
            //Print text one bye one
            //--用于标记文本是否显示完毕，完毕才可支持关闭Text
            bool textPrintOver = false;
            GameUIFuncs.PrintText(contentText,
                () =>
                {
                    Debug.Log("Text showed up.");
                    
                    textPrintOver = true;

                    //when it over， call back
                    if (onShowedOver != null)
                    {
                        Debug.Log("Call back>");
                        onShowedOver();
                    }

                });

            //--Set the panel click-able
            GameUIFuncs.SetUIClickable(clickBgPanel, 
                () =>       //点击时函数
                {
                    //--Text 显示完成，才允许关闭
                    if (textPrintOver)
                    {
                        //Close panel:
                        GameUIFuncs.DestroyRectTrans(clickBgPanel);
                        //CLose canvas:
                        DialogCanvas.CloseCanvas();

                        //callback
                        if (onClick != null)
                            onClick();
                    }

                }
            );



            #endregion
        }

        /// <summary>
        /// 显示一组对话信息
        /// </summary>
        public static void ShowDialogMsgs(DialogMsg[] arrDialogs , int currDialogIndex = 0, CallbackFunc onDialogsShowedDone = null)
        {
            //TODO: Show dialogs one by one
            ShowDialogMsg(arrDialogs[currDialogIndex].msg,arrDialogs[currDialogIndex].speakerName ,() =>
                {
                    Debug.Log("CLick>");

                    //Still can show next msg
                    if (currDialogIndex + 1 >= arrDialogs.Length)
                    {
                        if (onDialogsShowedDone != null)
                        {
                            onDialogsShowedDone();
                        }
                    }
                    else
                    {
                        ShowDialogMsgs(arrDialogs, currDialogIndex + 1, onDialogsShowedDone);
                    }
                },
                () =>
                {
                    Debug.Log("One dialog Showed.");

                    
                }
                );
                    
            
        }

        

        //----------------------------------------------------------------------------------------
        //                                      Private PART
        //----------------------------------------------------------------------------------------

	}
}