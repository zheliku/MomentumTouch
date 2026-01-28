using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringLengthMaxMin : MonoBehaviour
{
    
    BlockMove blockA = DataSetting.Instance.blockA;
    BlockMove blockB = DataSetting.Instance.blockB;
    BlockSpringCouple couple = DataSetting.Instance.couple;
    SpringMove spring = DataSetting.Instance.springMove;
    
    // Start is called before the first frame update
    void Start()
    {
        // 物块A质量
        float ma = blockA.Mass;
        // 物块B质量
        float mb = blockB.Mass;
        // 弹簧劲度系数
        float k = couple.k;
        // 弹簧原长
        float l = spring.StartLength;
        // 物块A初速度
        float initialSpeed = ConstantPanel.initialSpeed;
        
        // // 计算所有物理量
        // var delta = (blockA.MovePos - blockB.MovePos) / couple.moveRatio;
        // float va = blockA.MoveVelocity;
        // float vb = blockB.MoveVelocity;
        // float ma = blockA.MoveMomentum;
        // float mb = blockB.MoveMomentum;
        // float TM = ma + mb;
        // float ep = k * delta * delta / 2;
        // float ea = blockA.MoveKineticEnergy;
        // float eb = blockB.MoveKineticEnergy;
        // float te = ep + ea + eb;
    }

    // Update is called once per frame
    void Update()
    {
        BlockMove blockA = DataSetting.Instance.blockA;
        BlockMove blockB = DataSetting.Instance.blockB;
        BlockSpringCouple couple = DataSetting.Instance.couple;
        SpringMove spring = DataSetting.Instance.springMove;
        
        // 物块A质量
        float ma = blockA.Mass;
        // 物块B质量
        float mb = blockB.Mass;
        // 弹簧劲度系数
        float k = couple.k;
        // 弹簧原长
        float l = spring.StartLength;
        // 物块A初速度
        float initialSpeed = ConstantPanel.initialSpeed;
        
        var (original, compressed, stretched) = CalculateStates(ma, mb, k, l, initialSpeed);
        
    }
    
    public static (float[] original, float[] compressed, float[] stretched) CalculateStates(
        float ma, float mb, float k, float l, float initialSpeed)
    {
        // 计算核心物理参数
        float totalMass = ma + mb;
        float vCM = (ma * initialSpeed) / totalMass;       // 质心速度
        float reducedMass = (ma * mb) / totalMass;         // 约化质量
        float x = (float)Math.Sqrt(reducedMass * initialSpeed * initialSpeed / k); // 最大形变量

        // 原长状态
        float[] original = new float[10];
        original[0] = 0;                                    // delta
        original[1] = initialSpeed;                         // va
        original[2] = 0;                                    // vb
        original[3] = ma * original[1];                     // da
        original[4] = mb * original[2];                     // db
        original[5] = original[3] + original[4];            // TM
        original[6] = 0;                                    // ep
        original[7] = 0.5f * ma * original[1] * original[1];// ea
        original[8] = 0;                                    // eb
        original[9] = original[7] + original[8];            // te

        // 压缩状态
        float[] compressed = new float[10];
        compressed[0] = -x;                                 // delta
        compressed[1] = vCM;                                // va
        compressed[2] = vCM;                                // vb
        compressed[3] = ma * compressed[1];                 // da
        compressed[4] = mb * compressed[2];                 // db
        compressed[5] = compressed[3] + compressed[4];      // TM
        compressed[6] = 0.5f * k * x * x;                   // ep
        compressed[7] = 0.5f * ma * vCM * vCM;              // ea
        compressed[8] = 0.5f * mb * vCM * vCM;              // eb
        compressed[9] = compressed[7] + compressed[8] + compressed[6]; // te

        // 拉伸状态
        float[] stretched = new float[10];
        stretched[0] = x;                                   // delta
        stretched[1] = vCM;                                 // va
        stretched[2] = vCM;                                 // vb
        stretched[3] = ma * vCM;                            // da
        stretched[4] = mb * vCM;                            // db
        stretched[5] = stretched[3] + stretched[4];         // TM
        stretched[6] = compressed[6];                       // ep
        stretched[7] = compressed[7];                       // ea
        stretched[8] = compressed[8];                       // eb
        stretched[9] = compressed[9];                       // te

        return (original, compressed, stretched);
    }
    
}
