using GoogleARCore;
using System.Collections.Generic;
using UnityEngine;

public class ImageController : MonoBehaviour
{
    /// <summary>
    /// 显示用模型
    /// </summary>
    public GameObject prefab;
    /// <summary>
    /// 模型字典
    /// </summary>
    private Dictionary<int, GameObject> dictPrefab
            = new Dictionary<int, GameObject>();
    /// <summary>
    /// 识别图片列表
    /// </summary>
    private List<AugmentedImage> listAugmentedImage 
        = new List<AugmentedImage>();


    void Update()
    {
        //检查是否处于追踪状态
        if (Session.Status != SessionStatus.Tracking)
        {
            return;
        }

        //跟新识别图片列表
        Session.GetTrackables<AugmentedImage>(
            listAugmentedImage, TrackableQueryFilter.Updated);

        Debug.Log(listAugmentedImage.Count);

        //遍历识别图片列表
        foreach(AugmentedImage image in listAugmentedImage)
        {
            //从字典中获取模型
            dictPrefab.TryGetValue(image.DatabaseIndex, out GameObject outPrefab);

            
            if (image.TrackingState == TrackingState.Tracking 
                && outPrefab == null)   //识别图片被发现
            {
                //在识别图片中心添加追踪锚点
                Anchor anchor = image.CreateAnchor(image.CenterPose);

                //添加显示的模型
                GameObject temp = Instantiate(prefab, anchor.transform);
                //字典中添加对应项
                dictPrefab.Add(image.DatabaseIndex, temp);
            }
            else if (image.TrackingState == TrackingState.Stopped 
                && outPrefab != null)   //识别图片消失
            {
                //从字典中移除对应内容
                dictPrefab.Remove(image.DatabaseIndex);
                //删除模型
                GameObject.Destroy(outPrefab);
            }
        }
    }
}
