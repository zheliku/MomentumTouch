using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Grab;
using Oculus.Interaction.GrabAPI;
using Oculus.Interaction.Input;
using Oculus.Interaction.Throw;
using Oculus.Interaction.HandGrab;
using System;

public class try4 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    //public Rigidbody blockRigidbody;

    // 物块的初始速度
    // public float moveSpeed = 2f;


    // Update is called once per frame
    void Update()
    {

    }
    public HandGrabInteractor handGrabInteractor;  // 手部交互器的引用

    // 当手势被选中时调用
    public void OnGestureSelected()
    {
        Debug.Log("！");
        handGrabInteractor.ForceRelease();
        //blockRigidbody.velocity = new Vector3(moveSpeed, blockRigidbody.velocity.y, blockRigidbody.velocity.z);

    }

    // 当手势被取消选择时调用
    public void OnGestureUnselected()
    {
        Debug.Log("");
        // 可以做其他操作，例如重置物体位置
    }

    // 控制物体移动的功能

}
