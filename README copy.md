# MomentumTouch - VR一致性触觉交互系统

## 项目简介

MomentumTouch 是一个结合物理硬件与虚拟现实的一致性触觉交互系统，通过 Arduino 传感器采集真实世界的力反馈数据，并在 Unity VR 环境中实现沉浸式交互体验。本项目提供完整的硬件设计文件、Arduino 数据处理代码以及 Unity VR 交互项目。

## 系统架构

```
MomentumTouch
├── Models/           # 3D模型文件
│   ├── for 3d print/ # 用于3D打印的STL文件
│   └── for unity/    # 用于Unity的FBX文件
├── Arduino/          # Arduino传感器代码
│   ├── Press.ino     # 压力传感器程序
│   └── Pull.ino      # 拉力传感器程序
└── Unity/            # Unity VR项目
    ├── Assets/       # Unity资源文件
    ├── Packages/     # Unity包管理
    └── ProjectSettings/ # 项目配置
```

## 目录说明

### 1. Models/ - 3D模型资源

#### for 3d print/
包含所有可3D打印的硬件部件STL文件：
- `button01.stl` - 按钮部件1
- `button02.stl` - 按钮部件2
- `button03.stl` - 按钮部件3
- `knob01.stl` - 旋钮部件1
- `knob02.stl` - 旋钮部件2
- `slide.stl` - 滑动部件

**使用说明**：
- 推荐材料：PLA 或 ABS
- 打印参数：层高 0.2mm，填充率 20-30%
- 支撑结构：根据模型复杂度选择是否添加

#### for unity/
包含可导入Unity的FBX模型文件：
- `button.fbx` - 按钮模型
- `knob01.fbx` - 旋钮模型1
- `knob02.fbx` - 旋钮模型2
- `slide.fbx` - 滑动模型

### 2. Arduino/ - 传感器数据采集

由于我们使用的角度传感器自带嵌入程序，因此这里没有提供 .ino 文件。

#### Press.ino - 压力传感器程序
**功能**：通过模拟引脚采集压力传感器数据并通过串口输出。

**硬件连接**：
- 传感器信号引脚 → Arduino A0
- VCC → 5V
- GND → GND

**参数配置**：
```cpp
const int sensorPin = A0;  // 传感器引脚
Serial.begin(9600);        // 串口波特率
delay(100);                // 采样间隔(毫秒)
```

**数据输出格式**：
```
1023
512
256
...
```

#### Pull.ino - 拉力传感器程序
**功能**：使用 HX711 模块采集拉力传感器数据并转换为重量值。

**硬件连接**：
- HX711 SCK → Arduino D3
- HX711 DT → Arduino D2
- VCC → 5V
- GND → GND

**重要参数**：
```cpp
int HX711_SCK = 3;        // 时钟引脚
int HX711_DT = 2;         // 数据引脚
#define GapValue 405      // 校准值（需根据实际传感器调整）
```

**校准步骤**：
1. 上传程序到 Arduino
2. 空载状态下记录输出值
3. 放置已知重量物体
4. 根据公式计算 GapValue：`GapValue = (读数差值) / (重量克数)`
5. 修改代码中的 GapValue 并重新上传

**数据输出格式**：
```
Pull:125
Pull:130
Pull:128
...
```

**通用配置**：
- 串口波特率：9600
- 采样频率：10Hz (100ms间隔)
- 数据通过 USB 串口发送至计算机

### 3. Unity/ - VR交互项目

#### 系统要求
- **Unity 版本**：2021.3.45f2 (推荐使用相同版本以避免兼容性问题)
- **VR SDK**：Meta XR SDK v63.0.0
- **支持设备**：Meta Quest 2/3/Pro 或其他支持 OpenXR 的 VR 设备
- **操作系统**：Windows 10/11 (64位)

#### 项目结构
```
Unity/
├── Assets/
│   ├── Scenes/          # VR场景文件
│   ├── Scripts/         # C#交互脚本
│   ├── Models/          # 导入的3D模型
│   ├── Oculus/          # Meta XR SDK
|   ├── Resources/       # 资源文件
│   └── ...              # 其他
├── Packages/
│   └── manifest.json    # 包依赖配置
└── ProjectSettings/     # Unity项目配置
```

#### 快速开始

**1. 安装依赖**

首先下载并安装 Meta XR SDK：
- 访问 [Meta Quest 开发者中心](https://developer.oculus.com/downloads/package/meta-xr-sdk-all-in-one-upm/)
- 下载 Meta XR All-in-One SDK v63.0.0

**2. 配置包路径**

编辑 `Unity/Packages/manifest.json`，修改 Meta XR SDK 的本地路径：

```json
{
  "dependencies": {
    "com.meta.xr.sdk.core": "file:YOUR_PATH/com.meta.xr.sdk.core-63.0.0.tgz",
    "com.meta.xr.sdk.interaction": "file:YOUR_PATH/com.meta.xr.sdk.interaction-63.0.0.tgz",
    "com.meta.xr.sdk.interaction.ovr": "file:YOUR_PATH/com.meta.xr.sdk.interaction.ovr-63.0.0.tgz",
    ...
  }
}
```

将 `YOUR_PATH` 替换为您的实际 SDK 下载路径。

你也可以直接使用 Unity 2021.3.45f2 打开项目，项目中已配置好环境，直接运行即可。

**3. 打开项目**

1. 启动 Unity Hub
2. 点击"打开" → 选择 `Unity/` 文件夹
3. 等待 Unity 导入项目资源（首次打开需要较长时间）

**4. 配置 VR 设备**

1. 连接 Meta Quest 设备到计算机（USB 或 Air Link）
2. Unity 菜单：`Edit` → `Project Settings` → `XR Plug-in Management`
3. 勾选 `Oculus` 或 `OpenXR`
4. 确认设备在开发者模式下运行

**5. 串口通信配置**

在相关脚本 PullInput.cs/PressInput.cs/AngleInput.cs 中配置 Arduino 串口参数：
```csharp
string portName = "COM3";  // 根据实际端口修改
int baudRate = 9600;
```

可通过设备管理器查看 Arduino 实际使用的 COM 口。

**6. 运行项目**

1. 打开主场景：`Assets/Scenes/MainScene.unity`
2. 点击 Play 按钮或按 `Ctrl+P`
3. 戴上 VR 头显进行交互测试

#### 构建与部署

**构建为 APK（Quest 独立运行）**：
1. `File` → `Build Settings`
2. 切换平台到 `Android`
3. 连接 Quest 设备
4. 点击 `Build And Run`

**PC VR 模式**：
1. 保持平台为 `Windows`
2. 通过 Link 线缆或 Air Link 连接设备
3. 直接在 Unity 中运行

## 完整工作流程

### 硬件准备
1. 使用 `Models/for 3d print/` 中的 STL 文件进行 3D 打印
2. 组装硬件部件
3. 连接压力/拉力传感器到 Arduino
4. 上传对应的 `.ino` 程序到 Arduino

### 软件配置
1. 确认 Arduino 通过 USB 连接到计算机并正常工作
2. 使用串口监视器测试传感器数据输出
3. 打开 Unity 项目并配置串口参数
4. 导入 `Models/for unity/` 中的 FBX 模型（如需替换）

### 测试运行
1. 启动 Unity 项目
2. 确认 VR 设备已连接
3. 运行场景
4. 操作物理硬件，观察 VR 中的交互反馈

## 故障排查

### Arduino 问题
- **串口无法打开**：检查驱动安装、端口号、波特率设置
- **数据异常**：检查传感器接线、校准 GapValue 参数
- **无数据输出**：确认程序上传成功，检查串口监视器波特率

### Unity 问题
- **项目无法打开**：确认 Unity 版本匹配（2021.3.45f2）
- **Meta XR SDK 报错**：检查 manifest.json 中的路径配置
- **VR 设备不识别**：启用开发者模式、重启 Oculus 软件
- **串口读取失败**：确认 COM 口号正确、Arduino 正在运行

### VR 交互问题
- **手部追踪失效**：检查 VR 设备权限设置
- **触觉反馈不同步**：降低 Arduino 数据发送频率或优化 Unity 脚本
- **模型显示异常**：重新导入 FBX 文件并检查材质

## 技术支持

如遇到问题，请检查以下资源：
- [Arduino 官方文档](https://www.arduino.cc/reference/)
- [Unity 官方手册](https://docs.unity3d.com/)
- [Meta XR SDK 文档](https://developer.oculus.com/documentation/unity/)

## 开源协议

本项目将在论文正式录用后发布，具体开源协议待定。

## 引用

如果本项目对您的研究有帮助，请引用我们的论文（论文信息待补充）。

---

**更新日期**：2025年12月2日  
**项目状态**：开发完成，等待论文录用后正式发布