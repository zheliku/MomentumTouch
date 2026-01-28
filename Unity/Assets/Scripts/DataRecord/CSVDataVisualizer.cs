using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class CSVDataVisualizer : MonoBehaviour
{
    [Header("文本网格绑定")]
    public List<List<TextMeshPro>> textGrid = new List<List<TextMeshPro>>(); // 改为 TextMeshPro

    public void Visualizer()
    {
        if (Main.dataLog)
        {
            InitializeGridReferences();
            LoadAndDisplayCSVData();
        }
        
    }

    // 初始化网格引用（通过路径查找 Data1, Data2, Data3）
    private void InitializeGridReferences()
    {
        // 遍历 Data1, Data2, Data3
        for (int row = 0; row < 5; row++)
        {
            // 获取当前行的空物体路径（DataRecordTable/Data/Data1, Data2, Data3）
            string dataObjectPath = $"DataRecordTable/Data/Data{row + 1}";
            Transform dataObject = GameObject.Find(dataObjectPath)?.transform;
            if (dataObject == null)
            {
                Debug.LogError($"找不到空物体：{dataObjectPath}");
                continue;
            }

            // 创建当前行的 TextMeshPro 列表
            List<TextMeshPro> rowList = new List<TextMeshPro>();

            // 遍历当前空物体下的 TextMeshPro（Text-1 到 Text-10）
            for (int col = 0; col < 10; col++)
            {
                string textObjectName = $"Text-{col + 1}";
                Transform textObject = dataObject.Find(textObjectName);
                if (textObject != null && textObject.TryGetComponent(out TextMeshPro tmp))
                {
                    rowList.Add(tmp);
                    tmp.text = " "; // 初始化为空
                }
                else
                {
                    Debug.LogError($"找不到文本组件：{dataObjectPath}/{textObjectName}");
                    rowList.Add(null);
                }
            }

            // 将当前行的 TextMeshPro 列表添加到 textGrid
            textGrid.Add(rowList);
        }
    }

    // 加载并显示 CSV 数据
    public void LoadAndDisplayCSVData()
    {
        // 清空现有显示
        ClearGrid();
        
        // 根据 currentFileIndex 动态生成文件路径
        int showTableIndex = DataLogger.showTableIndex;
        string fileName = $"PhysicsData_{showTableIndex}.csv";
        string filePath = Path.Combine("Assets/StreamingAssets", "CSV", fileName);

        // 读取 CSV 数据
        List<string[]> csvData = ReadCSV(filePath);
        if (csvData == null || csvData.Count == 0)
        {
            Debug.LogWarning("CSV 文件为空或读取失败");
            return;
        }

        Debug.Log("读取开始");

        // 填充数据到网格（最多显示3行）
        for (int row = 0; row < Mathf.Min(csvData.Count, 3); row++)
        {
            string[] rowData = csvData[row];
            for (int col = 0; col < Mathf.Min(rowData.Length, 10); col++)
            {
                if (textGrid[row][col] != null)
                {
                    textGrid[row][col].text = rowData[col];
                }
            }
        }

        // 计算并显示每列的最大值和最小值
        for (int col = 0; col < 10; col++)
        {
            textGrid[3][col].text = " ";
            textGrid[4][col].text = " ";
            
            float maxT = -999;
            float minT = 999;
            for (int row = 0; row < Mathf.Min(csvData.Count, 3); row++)
            {
                if (float.TryParse(csvData[row][col], out float value))
                {
                    if (value > maxT)
                    {
                        maxT = value;
                    }
                    if (value < minT)
                    {
                        minT = value;
                    }
                }
                else
                {
                    textGrid[3][col].text = "0"; // 显示最大值
                    textGrid[4][col].text = "0"; // 显示最小值
                    // Debug.LogWarning($"第 {row + 1} 行第 {col + 1} 列数据解析失败");
                }
            }

            if (maxT != -999 && minT != 999)
            {
                textGrid[3][col].text = maxT.ToString(); // 显示最大值
                textGrid[4][col].text = minT.ToString(); // 显示最小值
            }
            
        }
    }

    // 清空网格显示
    private void ClearGrid()
    {
        foreach (var row in textGrid)
        {
            foreach (var cell in row)
            {
                if (cell != null) cell.text = "";
            }
        }
    }

    // 读取 CSV 文件内容
    private List<string[]> ReadCSV(string filePath)
    {
        List<string[]> data = new List<string[]>();
        try
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines.Skip(1)) // 跳过标题行
            {
                string[] fields = line.Split(',');
                data.Add(fields);
            }
            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"读取CSV失败：{e.Message}");
            return null;
        }
    }

    public void showTableIndexAdd()
    {
        if (DataLogger.showTableIndex < DataLogger.currentFileIndex )
        {
            DataLogger.showTableIndex++;
        }
    }
    
    public void showTableIndexSub()
    {
        if (DataLogger.showTableIndex > 1)
        {
            DataLogger.showTableIndex--;
        }
        // DataLogger.showTableIndex--;
    }
}