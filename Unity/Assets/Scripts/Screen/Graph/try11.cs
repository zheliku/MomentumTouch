using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Grab;
using Oculus.Interaction.HandGrab;

public class try11 : MonoBehaviour
{
    // 物块和手部交互相关的变量
    public Collider blockACollider;  // 物块A的Collider（移动物块）
    public Collider handCollider;//手的碰撞体
    public Rigidbody blockARigidbody;  // 物块A的Rigidbody（可移动）
    public Transform blockATransform;  // 物块A的Transform（用来调试位置）
    public Transform logo;//可删

    // 手指弯曲检测相关的变量
    public Transform[] fingerTransforms;  // 手指的Transform引用（至少5个手指）
    public Vector2[] grabAngleRanges;  // 每根手指的旋转角度范围（最小和最大角度）

    // 手部交互器
    public HandGrabInteractor handGrabInteractor;
    //public HandGrabInteractable grabInteractable;
    public HandGrabInteractable someHandGrabInteractable;  // 第一个抓取点可删
    public HandGrabInteractable someHandGrabInteractable1;  // 第二个抓取点可删


    //private bool isFrozen = false;  // 标记物块A是否被冻结

    // Start is called before the first frame update
    void Start()
    {
        if (blockACollider == null || blockARigidbody == null)
        {
            Debug.LogError("物块A的Collider或Rigidbody未分配！");
        }

        if (fingerTransforms == null || fingerTransforms.Length != 5 || grabAngleRanges == null || grabAngleRanges.Length != 5)
        {
            Debug.LogError("请确保已设置五根手指的Transform和抓取角度范围！");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 检查手指是否满足抓取条件
        bool allFingersBent = CheckAllFingersBent();
        float logoRotationX = logo.rotation.eulerAngles.x;
        // 检查物块A和手的包围盒是否重叠
        if (blockACollider != null)
        {
            bool isColliding = CheckForCollision();
            if (isColliding && allFingersBent)
            {
                if (logoRotationX >= 0f && logoRotationX <= 90f)
                {
                    Debug.Log("X轴旋转角度在0到80度之间，使用第一个抓取点");
                    handGrabInteractor.ForceSelect(someHandGrabInteractable, allowManualRelease: true);  // 使用第一个抓取点
                }// 物块A与手发生碰撞且手指弯曲到位，触发抓取
                else
                {
                    Debug.Log("X轴旋转角度超出范围，使用第二个抓取点");
                    handGrabInteractor.ForceSelect(someHandGrabInteractable1, allowManualRelease: true);  // 使用第二个抓取点
                }


            }
        }
    }

    // 检查所有手指是否都在指定的弯曲角度范围内
    private bool CheckAllFingersBent()
    {
        for (int i = 0; i < fingerTransforms.Length; i++)
        {
            // 获取每根手指的Z轴旋转角度
            float zRotation = fingerTransforms[i].localRotation.eulerAngles.z;
            //Debug.Log("Finger " + i + " Z Rotation: " + zRotation);

            // 获取每根手指的角度范围
            Vector2 angleRange = grabAngleRanges[i];

            // 检查每根手指是否在其允许的旋转角度范围内
            if (zRotation < angleRange.x || zRotation > angleRange.y)
            {
                return false;  // 只要有一个手指不符合条件，返回false
            }
        }
        return true;  // 如果所有手指都符合条件，返回true
    }

    // 检查物块A与手部的包围盒是否发生重叠
    private bool CheckForCollision()
    {
        // 获取物块A的包围盒
        Bounds blockABounds = blockACollider.bounds;

        // 获取手部的包围盒（手的Collider）

        if (handCollider == null)
        {
            Debug.LogError("手部的Collider未分配！");
            return false;
        }

        Bounds handBounds = handCollider.bounds;

        // 检查物块A和手是否发生重叠
        return blockABounds.Intersects(handBounds);
    }

    // 触发抓取
    //   private void TriggerGrab()
    // {
    // Debug.Log("手指弯曲到位，且手与物块A发生碰撞，开始抓取！");
    // handGrabInteractor.ForceSelect(grabInteractable, allowManualRelease: false);
    // Debug.Log("抓取到了物块A！");
    //  }

    private void OnDrawGizmos()//可视化包围盒便于调试
    {
        // 可视化物块A的包围盒
        if (blockACollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(blockACollider.bounds.center, blockACollider.bounds.size);
        }

        // 可视化手部的包围盒
        Collider handCollider = GetComponent<Collider>();
        if (handCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(handCollider.bounds.center, handCollider.bounds.size);
        }
    }
}
