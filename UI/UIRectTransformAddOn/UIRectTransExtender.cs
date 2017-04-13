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
        /// DFS查找，获得Transform的子物体（及其后代）中第一个含有该名字的物体。适用于需要深度搜索的情况
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

        /// <summary>
        /// 采用广度优先的方式查找Transform的子物体（及其后代）中第一个含有该名字的物体。适用于无需太多深度搜索的情况；
        /// </summary>
        public static RectTransform GetChildByName(this RectTransform parent, string childObjName,bool useBFS)
        {
            //深度优先（BFS）找到该名字的RectTransform 
            //使用一个栈进行遍历 =_=
            System.Collections.Generic.Queue<Transform> stack =
                new System.Collections.Generic.Queue<Transform>();

            //--将parent入栈
            stack.Enqueue(parent);

            Transform tmp;
            //--BFS开始
            while (stack.Count > 0)
            {
                tmp = stack.Dequeue();

                if (tmp.name.Equals(childObjName))
                {
                    //Great, got it
                    return (RectTransform)tmp;
                }

                //--push back the children
                for (int i = 0; i <tmp.childCount ; i++)
                {
                    stack.Enqueue(tmp.GetChild(i));
                }
            }

            //Not found
            return null;
        }

        
        [System.Obsolete("Use FindChild() instead")]
        /// <summary>
        /// Returns a transform child by name.
        /// 获得在子物体中（只包含第一代子物体）的物体
        /// </summary>
        /// <seealso cref="Transform.FindChild()"/>
        public static RectTransform GetChild(this RectTransform parent, string childObjName)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                if (parent.GetChild(i).name.Equals(childObjName))
                {
                    return (RectTransform) parent.GetChild(i);
                }
            }

            return null;
        }
	}
}