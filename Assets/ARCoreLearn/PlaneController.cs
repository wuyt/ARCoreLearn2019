using GoogleARCore;
using UnityEngine;

//如果是在编辑器运行，点击从Instant Preview获取
#if UNITY_EDITOR
using Input = GoogleARCore.InstantPreviewInput;
#endif

public class PlaneController : MonoBehaviour
{
    /// <summary>
    /// AR Core Device下的Camera
    /// </summary>
    public Camera FirstPersonCamera;
    /// <summary>
    /// 要显示的模型
    /// </summary>
    public GameObject prefab;

    void Update()
    {
        Lifecycle();

        //触点数不等于1
        if (Input.touchCount != 1)
        {
            return;
        }

        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            AddPrefab();
        }
    }
    /// <summary>
    /// 添加模型
    /// </summary>
    void AddPrefab()
    {
        //光线投射命中的类型
        TrackableHitFlags raycastFilter = 
            TrackableHitFlags.PlaneWithinPolygon |
            TrackableHitFlags.FeaturePointWithSurfaceNormal;

        if (Frame.Raycast(
            Input.GetTouch(0).position.x, 
            Input.GetTouch(0).position.y, 
            raycastFilter, out TrackableHit hit))
        {
            //检查是否投射在了检测平面的背面
            if ((hit.Trackable is DetectedPlane) &&
                Vector3.Dot(
                    FirstPersonCamera.transform.position - hit.Pose.position,
                    hit.Pose.rotation * Vector3.up) < 0)
            {
                Debug.Log("Hit at back of the current DetectedPlane");
            }
            else
            {
                //添加追踪锚点
                Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);
                //添加模型
                Instantiate(prefab,
                    hit.Pose.position,
                    hit.Pose.rotation,
                    anchor.transform);
            }
        }
    }
    /// <summary>
    /// 生命周期
    /// </summary>
    void Lifecycle()
    {
        //没有获得摄像头控制权限
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            Application.Quit();
        }
        //ARCore出错
        if (Session.Status.IsError())
        {
            Application.Quit();
        }

        //没有追踪内容的情况下15分钟屏幕休眠
        if (Session.Status != SessionStatus.Tracking)
        {
            Screen.sleepTimeout = 15;
        }
        else
        {   //屏幕从不休眠
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}
