using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public abstract class SensorInputBase<T> : MonoBehaviour where T : SensorInputBase<T>
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                Type type = typeof(T); // 获取参数 T 的类型信息
                GameObject obj = GameObject.Find(type.Name);
                if (obj == null)
                {
                    obj = new GameObject(type.Name); // 创建名称相同的 obj
                    _instance = obj.AddComponent<T>(); // 添加 _instance
                }
                else
                    _instance = obj.GetComponent<T>(); // 获取 _instance

                DontDestroyOnLoad(obj.transform.root); // 过场景不移除
            }

            return _instance;
        }
    }

    public string portName = "COM3";
    public int baudRate = 9600;

    private SerialPort _serialPort; // 传感器端口
    private Thread _sensorListener; // 监听线程

    [SerializeField] private bool _isRunning = false; // 线程是否运行

    public bool IsRunning => _isRunning;

    [SerializeField] protected float _nowValue, _lastValue; // 传感器读取的当前值和上一次的值

    [SerializeField] protected float _startValue; // 设置传感器初始值，供其他逻辑使用

    private void OnEnable()
    {
        Debug.Log(this.GetType().Name);

        // 先寻找文件中存储的端口
        string filePath = typeof(T).Name + ".txt";
        string port = TxtMgr.Instance.Load(filePath);
        if (port != default && TryOpenPort(port))
        {
            // 文件中存储的端口有效，则直接打开
            Debug.Log($"{typeof(T).Name}: Find port from file \"{filePath}\": {port}");
            return;
        }

        // 文件中存储的端口无效，则从端口 1 到端口 15 寻找 
        Invoke(nameof(LoadDelay), 1f);
    }

    private void LoadDelay()
    {
        StartCoroutine(InitPort(15));
    }

    /// <summary>
    /// 从端口 1 到端口 num 寻找端口
    /// </summary>
    /// <returns></returns>
    private IEnumerator InitPort(int num)
    {
        for (int i = 1; i <= num; i++)
        {
            var port = $"COM{i}";
            if (TryOpenPort(port))
            {
                Debug.Log($"{typeof(T).Name}: Find port from port1 to port10: {port}");
                break;
            }

            yield return null;
        }

        if (_serialPort is not { IsOpen: true })
            Debug.LogWarning($"{typeof(T).Name}: No port found!");
    }

    /// <summary>
    /// 尝试打开名为 port 的端口，返回是否打开成功
    /// </summary>
    /// <param name="port"></param>
    /// <returns></returns>
protected bool TryOpenPort(string port)
    {
        const int maxRetries = 5;
        const int readTimeout = 150;
    
        var typeName = GetType().Name;

        try
        {
            // 释放之前的端口实例
            if (_serialPort != null)
            {
                _serialPort.Close();
                _serialPort.Dispose();
                _serialPort = null;
            }

            _serialPort = new SerialPort(port, baudRate);
            _serialPort.ReadTimeout = readTimeout; // 设置读取超时

            if (_serialPort.IsOpen)
                return false;

            _serialPort.Open();
        
            string validData = null;
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    var input = _serialPort.ReadLine();
                    if (IsValidString(input))
                    {
                        validData = input;
                        break;
                    }
                }
                catch (TimeoutException)
                {
                    Debug.Log($"尝试 {i+1}/{maxRetries}: 端口 {port} 读取超时");
                }
            
                Thread.Sleep(300); // 重试间隔
            }
        
            if (validData == null)
            {
                _serialPort?.Close();
                return false;
            }

            portName = _serialPort.PortName;
            TxtMgr.Instance.Save(typeof(T).Name + ".txt", portName);

            _isRunning = _serialPort.IsOpen;

            if (_isRunning)
            {
                _sensorListener = new Thread(SensorListener)
                {
                    IsBackground = true
                };
                _sensorListener.Start();
                Debug.Log(portName + " opened!");

                Invoke(nameof(SetStartValueDelay), 1f);
            }
        }
        catch (Exception ex) // 建议记录异常ex以便调试
        {
            _serialPort?.Close();
            return false;
        }

        return true;
    }

    private void OnDisable()
    {
        _isRunning = false;
        if (_serialPort.IsOpen)
        {
            _serialPort.Close(); // 关闭串口连接
        }

        _sensorListener?.Abort(); // 退出线程
    }

    protected virtual void Update()
    {
    }

    /// <summary>
    /// 端口监听事件
    /// </summary>
    void SensorListener()
    {
        while (_isRunning)
        {
            // 如果线程可以运行
            try
            {
                // 尝试从端口获取值
                string input = ProcessString(_serialPort.ReadLine()); // 数据预处理
                _lastValue = _nowValue; // 更新上次的值
                _nowValue = Convert.ToSingle(input); // 读数
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }

    /// <summary>
    /// 用于延时记录传感器初始读数
    /// </summary>
    protected void SetStartValueDelay()
    {
        _startValue = _nowValue;
    }

    /// <summary>
    /// 处理传感器读取的字符串，变成全为数字的字符串（由子类实现）
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    protected abstract string ProcessString(string input);

    protected abstract bool IsValidString(string input);

    public abstract float Delta { get; }
    public abstract float Value { get; }
}