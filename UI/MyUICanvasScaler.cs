using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 根据不同设备予以UI不同的显示模式
/// </summary>
public class MyUICanvasScaler : MonoBehaviour {
    public static float g_preferSizeX = 800;
    public static float g_preferSizeY = 600;

	// Use this for initialization
	void Awake () 
    {
	    #if MOBILE_INPUT || UNITY_EDITOR
            //先判断是否iPad：
            bool isIpad = false;
            UnityEngine.iOS.DeviceGeneration ipGen = UnityEngine.iOS.Device.generation;
            switch(ipGen)
            {
                case UnityEngine.iOS.DeviceGeneration.iPad1Gen:
                case UnityEngine.iOS.DeviceGeneration.iPad2Gen:
                case UnityEngine.iOS.DeviceGeneration.iPad3Gen:
                case UnityEngine.iOS.DeviceGeneration.iPad4Gen:
                case UnityEngine.iOS.DeviceGeneration.iPadAir1:
                case UnityEngine.iOS.DeviceGeneration.iPadAir2:
                case UnityEngine.iOS.DeviceGeneration.iPadMini1Gen:
                case UnityEngine.iOS.DeviceGeneration.iPadMini2Gen:
                case UnityEngine.iOS.DeviceGeneration.iPadMini3Gen:
                case UnityEngine.iOS.DeviceGeneration.iPadMini4Gen:
                case UnityEngine.iOS.DeviceGeneration.iPadPro1Gen:
                case UnityEngine.iOS.DeviceGeneration.iPadUnknown:
                    isIpad = true;
                break;
            }
            
            //--再根据大小给定画布值
            //移动端时：
            CanvasScaler scaler = GetComponent<CanvasScaler>();
            Vector2 wantResolution;
            if (isIpad == false)
            {
                //手持设备：
                if (Screen.width > 2048)
                {
                    wantResolution.x = 1366;
                    wantResolution.y = 768;
                }
                else if (Screen.width > 1024)
                {
                    wantResolution.x = 1136;
                    wantResolution.y = 640;
                }
                else
                {
                    wantResolution.x = 800;
                    wantResolution.y = 600;
                }
            }
            else
            { 
                //iPad:
                //scaler.referenceResolution = new Vector2(Screen.width, Screen.width * 0.5625F);
                wantResolution.x = 2048;
                wantResolution.y = 1152;
            }

            scaler.referenceResolution = wantResolution;

            g_preferSizeX = wantResolution.x;
            g_preferSizeY = wantResolution.y;
        #else
            //非移动端时
            GetComponent<CanvasScaler>().referenceResolution = new Vector2(Screen.width, Screen.height);
        #endif
    }
	
#if UNITY_EDITOR
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("更改为PC UI:");
            GetComponent<CanvasScaler>().referenceResolution = new Vector2(1366, 768);
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("更改为Mobile UI:");
            GetComponent<CanvasScaler>().referenceResolution = new Vector2(800, 600);
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Ipad");
            GetComponent<CanvasScaler>().referenceResolution = new Vector2(2048,1152);
        }
	}
#endif
}
