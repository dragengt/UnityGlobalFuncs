using System.Collections.Generic;
using UnityEngine;

namespace Game.Global.Buffer
{
    /// <summary>
    /// 提供缓存机制的Object buffer，在需要时获得缓存中的对应的obj，如果不足则诞生新物体；用完要记得归还哦！
    /// </summary>
    class GameObjBuffer
    {
        Stack<GameObject> m_bufferList;

        public GameObjBuffer(GameObject objPrefab, int defaultCount = 3)
        {
            InitBuffer(objPrefab, defaultCount);
        }

        /// <summary>
        /// 设置buffer
        /// </summary>
        /// <param name="objPrefab">buffer中填充的物体</param>
        /// <param name="defaultCount">默认的长度</param>
        public void InitBuffer(GameObject objPrefab, int defaultCount = 3)
        {
            m_bufferList = new Stack<GameObject>(defaultCount * 2);

            GameObject obj;
            for (int i = 0; i < defaultCount; i++)
            {
                obj = GameObject.Instantiate(objPrefab);
                m_bufferList.Push(obj);

                obj.SetActive(false);
            }
        }

        /// <summary>
        /// 目前已经在buffer中的可用数量
        /// </summary>
        public int bufferSize
        {
            get
            {
                return m_bufferList.Count;
            }
        }

        /// <summary>
        /// 清除目前的buffer：
        /// </summary>
        public void ClearBuffer()
        {
            m_bufferList.Clear();
        }

        /// <summary>
        /// 获得buffer中的obj
        /// </summary>
        /// <remarks>buffer中物体不足1个时将会自我复制</remarks>
        public GameObject PopObjectBuffered()
        {
            GameObject result = m_bufferList.Pop();

            //--如果空了，先复制，再返回：
            if (m_bufferList.Count <= 0)
            {
                //再复制一个：
                m_bufferList.Push( GameObject.Instantiate(result) );
            }

            //显示出来：
            result.SetActive(true);

            return result;
        }

        /// <summary>
        /// 将obj放回buffer
        /// </summary>
        /// <param name="obj"></param>
        public void PushObject(GameObject obj)
        {
            obj.SetActive(false);

            m_bufferList.Push(obj);
        }

        /// <seealso cref="PushObject()"/>
        public void DestroyObject(GameObject obj)
        {
            PushObject(obj);
        }

        /// <seealso cref="PopObjectBuffered()"/>
        public GameObject InstantiateObj()
        {
            return PopObjectBuffered();
        }
    }
}