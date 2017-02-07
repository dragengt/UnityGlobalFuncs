using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 基于运行系统时间的时钟，可获得对应的时间，或者设置Timer
/// </summary>
class SystemTimer
{
	//----------------------------------------------------------------------------
	//							时间获取
	//----------------------------------------------------------------------------
    /// <summary>
    /// 获得系统的时间，用long表示
    /// </summary>
    /// <returns></returns>
    public static long GetSysTime()
    {
        return DateTime.UtcNow.ToFileTimeUtc();//
    }

    /// <summary>
    /// 获得前后时间差
    /// </summary>
    public static double GetTimeSpanSeconds(DateTime timeBefore,DateTime timeAfter)
    {
        TimeSpan span = timeAfter -timeBefore  ;
        
        return span.TotalSeconds;
    }

    /// <summary>
    /// 获得前后时间差
    /// </summary>
    public static double GetTimeSpanSeconds(long timeBefore, long timeAfter)
    { 
        //FileTimeUtc将生成的是1秒=10,000,000纳秒，以1601年为起始点计时；
        //(time1/3600/10000000/24/365)+1601)

        double delta = ((double)timeAfter - timeBefore)/10000000;
        return delta;
    }

	
	/// <summary>
	/// 将int秒的时间转为00：00：00格式
	/// </summary>
    /// <param name="value">秒</param>
	/// <returns></returns>
	public static string TimeToStr(int value)
	{
		int hours = value / 3600;
		value = value % 3600;
		
		int min = value / 60;
		value = value % 60;
		
		int secs = value;
		
		return string.Format(
			"{0}:{1}:{2}",
			hours.ToString("00"),
			min.ToString("00"),
			secs.ToString("00")
			);
	}
	
	/// <summary>
	/// 将00:00:00的字符串转为int时间
	/// </summary>
    /// <returns>返回秒为单位的时间</returns>
	public static int StrToTime(string str)
	{
		string[] timeStrs = str.Split(':');
		
		int hours, mins, secs;
		
		int.TryParse(timeStrs[0], out hours);
		int.TryParse(timeStrs[1], out mins);
		int.TryParse(timeStrs[2], out secs);
		
		int  result= hours * 3600 + mins * 60 + secs;
		return result;
	}
	
	//----------------------------------------------------------------------------
	//						计时器部分
	//----------------------------------------------------------------------------
	/// <summary>
	/// The delegater for coroutine
	/// </summary>
	static MonoBehaviour g_goCoroutineDelegater;
	static void InitCoroutine()
	{
		if(g_goCoroutineDelegater==null)
		{
			g_goCoroutineDelegater = new GameObject("CoroutineDelegater"). AddComponent<MonoBehaviour>();
		}
	}
	
	/// <summary>
	/// 设置一个延迟动作，（无需Mono类） Sets the action after.
	/// </summary>
	public static void SetDelayTimer(CallbackFunc cbAction, float timeDelta)
	{
		InitCoroutine();
		
		g_goCoroutineDelegater.StartCoroutine(GameObjFunc.IDelayDoSth(cbAction,timeDelta));
	}
	
	/// <summary>
	/// 设置一个每间隔时间段更新的Timer
	/// </summary>
	/// <param name="cbUpdateAction">update action.</param>
	/// <param name="timeDelta">Time delta.</param>
	public static void SetUpdateTimer(CallbackFunc cbUpdateAction, float timeDelta)
	{
		InitCoroutine();

		g_goCoroutineDelegater.StartCoroutine(GameObjFunc.IUpdateDo(cbUpdateAction,timeDelta));
	}
}

