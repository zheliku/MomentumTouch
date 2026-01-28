# MomentumTouch - VR Consistent Haptic Interaction System

## Project Overview

MomentumTouch is a consistent haptic interaction system that combines physical hardware with virtual reality. It collects real-world force feedback data through Arduino sensors and delivers immersive interaction experiences in a Unity VR environment. This project provides complete hardware design files, Arduino data processing code, and Unity VR interaction projects.

## System Architecture

```
MomentumTouch
├── Models/           # 3D model files 
│   ├── for 3d print/ # STL files for 3D printing
│   └── for unity/    # FBX files for Unity
├── Arduino/          # Arduino sensor code
│   ├── Press.ino     # Pressure sensor program
│   └── Pull.ino      # Pull sensor program
└── Unity/            # Unity VR project
    ├── Assets/       # Unity asset files
    ├── Packages/     # Unity package management
    └── ProjectSettings/ # Project configuration
```

## Directory Description

### 1. Models/ - 3D Model Resources

#### for 3d print/
Contains STL files for all 3D-printable hardware components:
- `button01.stl` - Button component 1
- `button02.stl` - Button component 2
- `button03.stl` - Button component 3
- `knob01.stl` - Knob component 1
- `knob02.stl` - Knob component 2
- `slide.stl` - Slide component

**Usage Instructions**:
- Recommended materials: PLA or ABS
- Print settings: Layer height 0.2mm, infill 20-30%
- Support structures: Add based on model complexity

#### for unity/
Contains FBX model files for importing into Unity:
- `button.fbx` - Button model
- `knob01.fbx` - Knob model 1
- `knob02.fbx` - Knob model 2
- `slide.fbx` - Slide model

### 2. Arduino/ - Sensor Data Acquisition

Since the angle sensor we use has embedded firmware, no .ino files are provided here.

#### Press.ino - Pressure Sensor Program
**Function**: Collects pressure sensor data through analog pins and outputs via serial port.

**Hardware Connection**:
- Sensor signal pin → Arduino A0
- VCC → 5V
- GND → GND

**Parameter Configuration**:
```cpp
const int sensorPin = A0;  // Sensor pin
Serial.begin(9600);        // Serial baud rate
delay(100);                // Sampling interval (milliseconds)
```

**Data Output Format**:
```
1023
512
256
...
```

#### Pull.ino - Pull Sensor Program
**Function**: Uses HX711 module to collect pull sensor data and convert to weight values.

**Hardware Connection**:
- HX711 SCK → Arduino D3
- HX711 DT → Arduino D2
- VCC → 5V
- GND → GND

**Important Parameters**:
```cpp
int HX711_SCK = 3;        // Clock pin
int HX711_DT = 2;         // Data pin
#define GapValue 405      // Calibration value (adjust based on actual sensor)
```

**Calibration Steps**:
1. Upload program to Arduino
2. Record output value under no-load condition
3. Place an object of known weight
4. Calculate GapValue using formula: `GapValue = (reading difference) / (weight in grams)`
5. Modify GapValue in code and re-upload

**Data Output Format**:
```
Pull:125
Pull:130
Pull:128
...
```

**General Configuration**:
- Serial baud rate: 9600
- Sampling frequency: 10Hz (100ms interval)
- Data sent to computer via USB serial port

### 3. Unity/ - VR Interaction Project

#### System Requirements
- **Unity Version**: 2021.3.45f2 (recommended to use the same version to avoid compatibility issues)
- **VR SDK**: Meta XR SDK v63.0.0
- **Supported Devices**: Meta Quest 2/3/Pro or other VR devices supporting OpenXR
- **Operating System**: Windows 10/11 (64-bit)

#### Project Structure
```
Unity/
├── Assets/
│   ├── Scenes/          # VR scene files
│   ├── Scripts/         # C# interaction scripts
│   ├── Models/          # Imported 3D models
│   ├── Oculus/          # Meta XR SDK
|   ├── Resources/       # Resource files
│   └── ...              # Others
├── Packages/
│   └── manifest.json    # Package dependency configuration
└── ProjectSettings/     # Unity project configuration
```

#### Quick Start

**1. Install Dependencies**

First, download and install Meta XR SDK:
- Visit [Meta Quest Developer Center](https://developer.oculus.com/downloads/package/meta-xr-sdk-all-in-one-upm/)
- Download Meta XR All-in-One SDK v63.0.0

**2. Configure Package Path**

Edit `Unity/Packages/manifest.json` and modify the local path of Meta XR SDK:

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

Replace `YOUR_PATH` with your actual SDK download path.

You can also directly open the project using Unity 2021.3.45f2. The environment is already configured in the project and can be run directly.

**3. Open Project**

1. Launch Unity Hub
2. Click "Open" → Select the `Unity/` folder
3. Wait for Unity to import project assets (first-time opening takes longer)

**4. Configure VR Device**

1. Connect Meta Quest device to computer (USB or Air Link)
2. Unity menu: `Edit` → `Project Settings` → `XR Plug-in Management`
3. Check `Oculus` or `OpenXR`
4. Confirm device is running in developer mode

**5. Serial Communication Configuration**

In the relevant scripts PullInput.cs/PressInput.cs/AngleInput.cs, configure Arduino serial port parameters:
```csharp
string portName = "COM3";  // Modify based on actual port
int baudRate = 9600;
```

Check the actual COM port used by Arduino through Device Manager.

**6. Run Project**

1. Open main scene: `Assets/Scenes/MainScene.unity`
2. Click Play button or press `Ctrl+P`
3. Put on VR headset for interaction testing

#### Build and Deployment

**Build as APK (Quest standalone)**:
1. `File` → `Build Settings`
2. Switch platform to `Android`
3. Connect Quest device
4. Click `Build And Run`

**PC VR Mode**:
1. Keep platform as `Windows`
2. Connect device via Link cable or Air Link
3. Run directly in Unity

## Complete Workflow

### Hardware Preparation
1. Use STL files in `Models/for 3d print/` for 3D printing
2. Assemble hardware components
3. Connect pressure/pull sensors to Arduino
4. Upload corresponding `.ino` programs to Arduino

### Software Configuration
1. Confirm Arduino is connected to computer via USB and working properly
2. Test sensor data output using serial monitor
3. Open Unity project and configure serial port parameters
4. Import FBX models from `Models/for unity/` (if replacement needed)

### Testing and Running
1. Launch Unity project
2. Confirm VR device is connected
3. Run scene
4. Operate physical hardware and observe interaction feedback in VR

## Troubleshooting

### Arduino Issues
- **Cannot open serial port**: Check driver installation, port number, baud rate settings
- **Abnormal data**: Check sensor wiring, calibrate GapValue parameter
- **No data output**: Confirm program uploaded successfully, check serial monitor baud rate

### Unity Issues
- **Project won't open**: Confirm Unity version matches (2021.3.45f2)
- **Meta XR SDK error**: Check path configuration in manifest.json
- **VR device not recognized**: Enable developer mode, restart Oculus software
- **Serial read failure**: Confirm COM port number is correct, Arduino is running

### VR Interaction Issues
- **Hand tracking failure**: Check VR device permission settings
- **Haptic feedback out of sync**: Reduce Arduino data transmission frequency or optimize Unity scripts
- **Model display abnormal**: Re-import FBX files and check materials

## Technical Support

If you encounter problems, please check the following resources:
- [Arduino Official Documentation](https://www.arduino.cc/reference/)
- [Unity Official Manual](https://docs.unity3d.com/)
- [Meta XR SDK Documentation](https://developer.oculus.com/documentation/unity/)

## Open Source License

This project will be released after the paper is officially accepted. The specific open source license is to be determined.

## Citation

If this project is helpful to your research, please cite our paper (paper information to be added).

---

**Last Updated**: December 2, 2025  
**Project Status**: Development completed, awaiting official release after paper acceptance
