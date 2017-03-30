using UnityEngine;
using System.Collections;

namespace Game.UI
{
    /// <summary>
    /// 对RectTransform增加了扩展方法，增加了对子物体的查找功能
    /// </summary>
	public static class UIRectTransExtender 
	{
        //为RectTransform增加一个扩展函数
        /// <summary>
        /// 根据id获得对应的rectTransform。（要求这个RectTransform必须挂有UIRectTransformExtend）
        /// </summary>
        /// <seealso cref="UIRectTransExtend.GetChildById(id)"/>
        public static RectTransform GetChildById(this RectTransform parent, string childId)
        {
            UIRectTransExtend rectTransManager = parent.GetComponent<UIRectTransExtend>();
            RectTransform childGot = null;

            //没有管理器就null：
            if (rectTransManager == null)
            {
                return null;
            }
            else
            {
                //--尝试获取child
                rectTransManager.GetChildById(childId);
                return childGot;
            }
        }

        /// <summary>
        /// 获得Transform的子物体中第一个含有该名字的物体
        /// </summary>
        /// <param name="childObjName">子物体的名字</param>
        public static RectTransform GetChildByName(this RectTransform parent, string childObjName)
        { 
            //深度优先（DFS）找到该名字的RectTransform 
            //使用一个栈进行遍历 =_=
            System.Collections.Generic.Stack<Transform> stack = 
                new System.Collections.Generic.Stack<Transform>();

            //--将parent入栈
            stack.Push(parent);

            Transform tmp;
            //--DFS开始
            while (stack.Count > 0)
            {
                tmp = stack.Pop();

                if (tmp.name.Equals(childObjName))
                {
                    //Great, got it
                    return (RectTransform)tmp;
                }

                //--push back the children
                for (int i = tmp.childCount - 1; i >= 0 ; i--)
                {
                    stack.Push(tmp.GetChild(i));
                }
            }

            //Not found
            return null;
        }
        
	}
}