using UnityEngine;
using System.Collections;

public delegate void CallbackFunc();
public delegate void CallbackFuncWithBool(bool param);
public delegate void CallbackFuncWithGameObj(GameObject param);
public delegate void CallbackFuncWithTransform(Transform param);
public delegate void CallbackFuncWithVector3(Vector3 param);
public delegate void CallbackFuncWithCollider(Collider param);

public delegate bool BoolCallbackFunc();
public delegate bool BoolCallbackFuncWithBool(bool param);
public delegate bool BoolCallbackFuncWithInt(int param);
public delegate bool BoolCallbackFuncWithStr(string param);
public delegate bool BoolCallbackFuncWithGameObj(GameObject param);

public delegate int IntCallbackFunc();
public delegate int IntCallbackFuncWithInt(int param);
public delegate int IntCallbackFuncWithFloat(float param);

/// <summary>
/// 提供各种游戏物体中常用的函数；
/// </summary>
public class GameObjFunc
{

    /// <summary>
    /// 压扁物体
    /// </summary>
    /// <param name="scaleTargetTimes">缩放倍数</param>
    public static IEnumerator IScaleDownTrans(Transform m_transform, float scaleTargetTimes = 0.4F)
    {
        bool isShrink = scaleTargetTimes < 1F;                    //是否是缩小
        Vector3 currScale = m_transform.localScale;
        float currScaleY = currScale.y;
        float tarScaleY = currScaleY * scaleTargetTimes;
        float rateToScale = (currScaleY - tarScaleY);// *( isShrink ? 1F : -1F ); //是缩小的话，则缩小率为（当前-目标）
        //是放大的话，则为（目标-当前）

        while (((currScaleY > tarScaleY) && isShrink)              //缩小的情况下，则是在当前还比目标状态大的情况下进行缩小
            ||
            ((!isShrink) && currScaleY < tarScaleY)                //放大时，则是在当前还比target小的情况下进行缩大
            )
        {
            //缩小时减少，放大时增大
            currScaleY -= rateToScale * Time.deltaTime;// *( isShrink ? -1 : 1 );
            currScale.y = currScaleY;
            m_transform.localScale = currScale;
            yield return 1;
        }

        currScale.y = tarScaleY;
        m_transform.localScale = currScale;
    }


    /// <summary>
    /// 直接让物体定位到一个物体的位置上
    /// </summary>
    /// <param name="deltaYToTarget">与目标物体Transform在Y轴上的偏移值</param>
    public static void FollowTrans(Transform src, Transform target, float deltaXToTarget = 0, float deltaYToTarget = 0)
    {
        //Follow position:
        Vector3 tmpV3 = Camera.main.WorldToScreenPoint(target.position);
        tmpV3.x -= deltaXToTarget;
        tmpV3.y -= deltaYToTarget;
        src.position = tmpV3;
    }

    /// <summary>
    /// 跟随到鼠标的位置
    /// </summary>
    /// <param name="src"></param>
    public static void FollowMouse(Transform src)
    {
        Vector3 mousePos = MyInput.mousePosition;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        src.transform.position = worldPos;
    }

    /// <summary>
    /// 协程：移动一个物体到指定位置
    /// </summary>
    public static IEnumerator IMoveUIToRectTransPos(Transform srcRT, Vector3 targetPos, CallbackFunc onDone = null)
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
    /// 发起一个持续一段时间的Update函数，每一帧更新
    /// </summary>
    /// <param name="continueTime">更新持续的时间</param>
    /// <param name="updateFunc">更新的动作</param>
    public static IEnumerator IUpdateDo(float continueTime, CallbackFunc updateFunc, CallbackFunc doneFunc = null)
    {
        float timePassed = 0F;

        while (timePassed <= continueTime)
        {
            updateFunc();

            timePassed += Time.deltaTime;
            yield return 1;
        }

        if (doneFunc != null)
            doneFunc();
    }

    /// <summary>
    /// 持续Update一段动作，同时指定了Update的时间间隔
    /// </summary>
    /// <param name="deltaTime">每次update间的间隔时间</param>
    public static IEnumerator IUpdateDo(float continueTime,float deltaTime, CallbackFunc updateFunc, CallbackFunc doneFunc = null)
    {
        float timePassed = 0F;
        WaitForSeconds waitTime = new WaitForSeconds(deltaTime);

        while (timePassed <= continueTime)
        {
            updateFunc();

            timePassed += deltaTime;
            yield return waitTime;
        }

        if (doneFunc != null)
            doneFunc();
    }

    /// <summary>
    /// 启动一个update协程，并且可以指定间隔时间，停止条件
    /// </summary>
    /// <param name="deltaTime">update 间隔时间</param>
	/// <param name="cbContinueCondition">继续更新的条件</para>
	public static IEnumerator IUpdateDo(CallbackFunc updateFunc,float deltaTime = 0,BoolCallbackFunc cbContinueCondition = null)
    {
		//如果没有设置退出条件的回调函数，则设置回调函数为true造成死循环
		if(cbContinueCondition == null)
		{
			cbContinueCondition =
				()=>
			{
				return true;
			};
			
		}

    
        if (deltaTime > 0)
        {
            //有间隔更新
            WaitForSeconds waitSeconds = new WaitForSeconds(deltaTime);
            
			//Debug.Log ("deltatime:"+deltaTime);

            while (cbContinueCondition() ==true )
            {
                updateFunc();

                yield return waitSeconds;
            }
        }
        else
        {
            //每一帧更新
            while (cbContinueCondition()==true)
            {
                updateFunc();

                yield return 1;
            }
        }

    }



    /// <summary>
    /// 延迟一段时间才执行动作
    /// </summary>
    public static IEnumerator IDelayDoSth(CallbackFunc delayAction, float delayTime)
    {
        while (delayTime > 0)
        {
            yield return 1;
            delayTime -= Time.deltaTime;
        }

        delayAction();
    }

    /// <summary>
    /// 延迟一帧才执行动作
    /// </summary>
    public static IEnumerator IDelayDoSth(CallbackFunc delayAction)
    {
        yield return 1;

        delayAction();
    }

    /// <summary>
    /// 转向目标物体，并且保持为只在y轴上的转向
    /// </summary>
    public static void RotateYToTransform(Transform src, Transform target)
    {
        RotateYToTransform(src, target.position);
    }

    /// <summary>
    /// 转向目标物体，并且保持为只在y轴上的转向
    /// </summary>
    public static void RotateYToTransform(Transform src, Vector3 target)
    {
        //转向老爹
        src.LookAt(target);
        Vector3 eulerRotate = src.rotation.eulerAngles;

        //修正欧拉角转向
        eulerRotate.x = eulerRotate.z = 0;
        src.rotation = Quaternion.Euler(eulerRotate);
    }


}
