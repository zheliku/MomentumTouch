using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class DataLogger : MonoBehaviour
{
    // 配置参数
    [Header("文件设置")]
    public string baseFileName = "PhysicsData";
    public int maxRowsPerFile = 3;

    [Header("数据分类设置")]
    public static int[] CategoryTags = new[] { 1, 0, 0 }; // 分类标签数组
    public static bool LineSwitching = false;       // 行切换标志

    // 运行时状态
    public static bool newRecord = true;
    public static int showTableIndex = 0;
    public static int currentFileIndex = 0;
    public static int lastCategoryTags = 0;
    
    public static bool aaaaa = true;
    
    private int currentRowCount = 0;
    private bool newFileRequested = false;
    private string currentFilePath;

    private bool isWritten = false;
    
    // 当前行数据缓冲区（长度对应总列数10）
    private string[] currentLineBuffer = new string[10];

    public void RecordToCSV()
    {
        if (Main.dataLog)
        {
            showTableIndex = currentFileIndex;
        
            if (newRecord)
            {
                CreateNewFile();
                newRecord = false;
            }

            // 需要换行
            if (LineSwitching)
            {
                // currentRowCount++;
                isWritten = false;
                LineSwitching = false;
            }
        
            // 保存数据到csv
            DataProcessing();
            aaaaa = true;
        }
        
        
    }

    #region 文件操作模块
    private void InitializeNewFile()
    {
        currentFilePath = GenerateFilePath();
        CreateFileHeader();
        currentRowCount = 0;
        ResetLineBuffer();
        Debug.Log($"已创建新文件：{currentFilePath}");
    }

    private void ResetLineBuffer()
    {
        currentLineBuffer = new string[10];
        Debug.Log("行缓冲区已重置");
    }

    // ...（其他文件操作方法保持不变）
    #endregion

    #region 数据操作模块
    private void DataProcessing()
    {
        var blockA = DataSetting.Instance.blockA;
        var blockB = DataSetting.Instance.blockB;
        var couple = DataSetting.Instance.couple;
        var spring = DataSetting.Instance.springMove;
        var k = couple.k;

        
        
        // 计算所有物理量
        var delta = (blockA.MovePos - blockB.MovePos) / couple.moveRatio;
        float va = blockA.MoveVelocity;
        float vb = blockB.MoveVelocity;
        float ma = blockA.MoveMomentum;
        float mb = blockB.MoveMomentum;
        float TM = ma + mb;
        float ep = k * delta * delta / 2;
        float ea = blockA.MoveKineticEnergy;
        float eb = blockB.MoveKineticEnergy;
        float te = ep + ea + eb;
        
        currentLineBuffer = new string[10];
        
        // 动态更新缓冲区（保留旧值，仅覆盖激活的标签）
        if (CategoryTags[0] == 1)
        {
            currentLineBuffer[0] = delta.ToString("F2");
            currentLineBuffer[1] = va.ToString("F2");
            currentLineBuffer[2] = vb.ToString("F2");
        }
        if (CategoryTags[1] == 1)
        {
            currentLineBuffer[3] = ma.ToString("F2");
            currentLineBuffer[4] = mb.ToString("F2");
            currentLineBuffer[5] = TM.ToString("F2");
        }
        if (CategoryTags[2] == 1)
        {
            currentLineBuffer[6] = ep.ToString("F2");
            currentLineBuffer[7] = ea.ToString("F2");
            currentLineBuffer[8] = eb.ToString("F2");
            currentLineBuffer[9] = te.ToString("F2");
        }

        if (CategoryTags[0] != 0 || CategoryTags[1] != 0 || CategoryTags[2] != 0)
        {
            if (isWritten)
            {
                RemoveLastLineFromFile();
                Debug.LogWarning("删除行");
            }

            Written();
        
            isWritten = true;
        }

        // Debug.Log($"缓冲区更新：{string.Join("|", currentLineBuffer)}");
    }

    // 在类中添加以下方法
    public void RemoveLastLineFromFile()
    {
        if (!File.Exists(currentFilePath))
        {
            Debug.LogWarning($"文件不存在：{currentFilePath}");
            return;
        }

        try
        {
            // 读取所有行
            var lines = File.ReadAllLines(currentFilePath).ToList();
            
            // 确保至少有标题行和至少一行数据
            if (lines.Count > 1) // 标题行 + 至少一行数据
            {
                // 移除最后一行
                lines.RemoveAt(lines.Count - 1);
                
                // 重新写入文件
                File.WriteAllLines(currentFilePath, lines);
                
                // 更新行计数器（确保不会小于0）
                currentRowCount = Mathf.Max(0, currentRowCount - 1);
                
                Debug.Log($"成功移除最后一行（剩余行数：{currentRowCount}）");
            }
            else
            {
                Debug.LogWarning("没有可移除的数据行");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"移除最后一行失败：{e.Message}");
        }
    }

    
    private void Written()
    {
        // 检查是否有有效数据
        bool hasData = currentLineBuffer.Any(s => !string.IsNullOrEmpty(s));
        
        if (!hasData)
        {
            Debug.LogWarning("未记录数据：没有激活的分类标签");
            return;
        }

        // 检查行数限制
        if (currentRowCount >= maxRowsPerFile)
        {
            Debug.LogWarning($"已达到每文件最大行数限制（{maxRowsPerFile}行）");
            return;
        }

        // 处理空值（保留空字符串）
        string line = string.Join(",", currentLineBuffer);

        // 写入文件并更新状态
        File.AppendAllText(currentFilePath, line + "\n");
        currentRowCount++;
        // ResetLineBuffer();
        
        Debug.Log($"已写入完整行（当前文件行数：{currentRowCount}/{maxRowsPerFile}）");
    }
    #endregion

    private string GenerateFilePath()
    {
        return Path.Combine(
            "Assets/StreamingAssets/CSV",
            $"{baseFileName}_{currentFileIndex}.csv"
        );
    }

    private void CreateFileHeader()
    {
        File.WriteAllText(currentFilePath, "SpringDeformation, VelocityA, VelocityB, MomentumA, MomentumB, TotalMomentum, ElasticPotentialEnergy, KineticEnergyA, KineticEnergyB, TotalEnergy\n");
    }

    private void CreateNewFile()
    {
        currentFileIndex++;
        showTableIndex = currentFileIndex;
        InitializeNewFile();
    }


    #region 数据操作模块
    // private void SaveNewData()
    // {
    //     var blockA = DataSetting.Instance.blockA;
    //     var blockB = DataSetting.Instance.blockB;
    //     var couple = DataSetting.Instance.couple;
    //     var spring = DataSetting.Instance.springMove;
    //     var k = couple.k;
    //
    //     var l0 = spring.StartLength;
    //
    //     // 弹簧形变量
    //     var delta = (blockA.MovePos - blockB.MovePos) / couple.moveRatio;
    //
    //     // 物块A速度
    //     float va = blockA.MoveVelocity;
    //
    //     // 物块B速度
    //     float vb = blockB.MoveVelocity;
    //
    //     // 物块A动量
    //     float ma = blockA.MoveMomentum;
    //
    //     // 物块B动量
    //     float mb = blockB.MoveMomentum;
    //
    //     // 总动量
    //     float TM = ma + mb;
    //
    //     // 弹性势能
    //     float ep = k * delta * delta / 2;
    //
    //     // 物块A动能
    //     float ea = blockA.MoveKineticEnergy;
    //
    //     // 物块B动能
    //     float eb = blockB.MoveKineticEnergy;
    //
    //     // 总能量
    //     float te = ep + ea + eb;
    //
    //     // 将数据保存到数组
    //     var data = new float[] {
    //         delta,     // 弹簧形变量
    //         va,        // 物块A速度
    //         vb,        // 物块B速度
    //         ma,        // 物块A动量
    //         mb,        // 物块B动量
    //         TM,        // 总动量
    //         ep,        // 弹性势能
    //         ea,        // 物块A动能
    //         eb,        // 物块B动能
    //         te         // 总能量
    //     };
    //
    //     // 将数据保存到文件
    //     AppendDataToFile(data);
    //     currentRowCount++;
    //     Debug.Log($"已保存数据（当前文件行数：{currentRowCount}/{maxRowsPerFile}）");
    // }
    //
    // private void AppendDataToFile(float[] data)
    // {
    //     string formatted = string.Join(",", 
    //         data.Select(d => d.ToString("F2")).ToArray());
    //
    //     File.AppendAllText(currentFilePath, formatted + "\n");
    // }
    #endregion
    
}