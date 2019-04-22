using GoogleARCore;
using UnityEngine;
using UnityEngine.UI;

public class LightManager : MonoBehaviour
{
    /// <summary>
    /// 滚动条
    /// </summary>
    private Slider slider;

    void Start()
    {
        slider = FindObjectOfType<Slider>();
    }

    void Update()
    {
        //如果光照评估状态无效
        if (Frame.LightEstimate.State != LightEstimateState.Valid)
        {
            return;
        }

        slider.value = Frame.LightEstimate.PixelIntensity;
    }
}
