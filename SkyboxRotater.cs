using UnityEngine;
using System.Collections;

namespace Game.Global.Skybox
{
    public class SkyboxRotater : MonoBehaviour
    {

        [SerializeField]
        Material[] skyBoxsMats;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            UpdateSkybox();

        }


        //旋转SkyBox
        const float m_skyBoxRotateSpeed = 0.74F;
        private void UpdateSkybox()
        {
            float rotOfSb = RenderSettings.skybox.GetFloat("_Rotation") + m_skyBoxRotateSpeed * Time.deltaTime;
            rotOfSb = rotOfSb % 360;
            RenderSettings.skybox.SetFloat("_Rotation", rotOfSb);

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                RenderSettings.skybox = skyBoxsMats[0];
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                RenderSettings.skybox = skyBoxsMats[1];
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                RenderSettings.skybox = skyBoxsMats[2];
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                RenderSettings.skybox = skyBoxsMats[3];
            }
        }
    }
}