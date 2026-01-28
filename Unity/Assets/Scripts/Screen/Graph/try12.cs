using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Throw;
using Oculus.Interaction.HandGrab;

public class try12 : MonoBehaviour
{
    public HandGrabInteractor handGrabInteractor;
    public GameObject object1; // 第一个物体
    public GameObject object2; // 第二个物体
    public float closeDistance = 2.0f; // 足够近的距离阈值

    void Update()
    {
        if (object1 == null || object2 == null)
        {
            Debug.LogError("请确保两个物体都已分配！");
            return;
        }

        // 计算两个物体之间的距离
        float distance = Vector3.Distance(object1.transform.position, object2.transform.position);

        // 判断距离是否足够近
        if (distance <= closeDistance)
        {
            Debug.Log("两个物体足够近，距离为: " + distance);
        }
        else
        {
            Debug.Log("两个物体不够近，距离为: " + distance);
            handGrabInteractor.ForceRelease();
        }
    }
}