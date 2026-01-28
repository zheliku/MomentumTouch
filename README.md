# MomentumTouch - VR Consistent Haptic Interaction System

## Project Introduction

MomentumTouch is a consistent haptic interaction system that combines physical hardware with virtual reality. It collects real-world force feedback data through Arduino sensors and achieves immersive interaction experiences in Unity VR environments. This project provides complete hardware design files, Arduino data processing code, and Unity VR interaction projects.

## System Architecture

```
MomentumTouch
├── Models/           # 3D model files
│   ├── for 3d print/ # STL files for 3D printing
│   └── for unity/    # FBX files for Unity
├── Arduino/          # Arduino sensor code
│   ├── Press.ino     # Pressure sensor program
│   └── Pull.ino      # Tension sensor program
└── Unity/            # Unity VR project
    ├── Assets/       # Unity asset files
    ├── Packages/     # Unity package management
    └── ProjectSettings/ # Project configuration
```

## Directory Description

### 1. Models/ - 3D Model Resources

#### for 3d print/

Contains all printable hardware component STL files:

- `button01.stl` - Button component 1
- `button02.stl` - Button component 2
- `button03.stl` - Button component 3
- `knob01.stl` - Knob component 1
- `knob02.stl` - Knob component 2
- `slide.stl` - Slide component

**Usage Instructions**:

- Recommended material: PLA or ABS
- Support structure: Choose whether to add based on model complexity

#### for unity/

Contains FBX model files importable into Unity:

- `button.fbx` - Button model
- `knob01.fbx` - Knob model 1
- `knob02.fbx` - Knob model 2
- `slide.fbx` - Slide model

### 2. Arduino/ - Sensor Data Acquisition

Since the angle sensors we use come with embedded programs, no .ino files are provided here.

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

#### Pull.ino - Tension Sensor Program

**Function**: Uses HX711 module to collect tension sensor data and convert to weight values.

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
2. Record output value in unloaded state
3. Place object of known weight
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

- **Unity Version**: 2021.3.45f2 (Recommended to use the same version to avoid compatibility issues)
- **VR SDK**: Meta XR SDK v63.0.0
- **Supported Devices**: Meta Quest 2/3/Pro VR devices
- **Operating System**: Windows 10/11 (64-bit)

#### Project Structure

```
Unity/
├── Assets/
│   ├── Scenes/          # VR scene files
│   ├── Scripts/         # C# interaction scripts
│   ├── Models/          # Imported 3D models
│   ├── Oculus/          # Meta XR SDK
│   ├── Resources/       # Resource files
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

**2. Configure Package Paths**

Edit `Unity/Packages/manifest.json`, modify the local path for Meta XR SDK:

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

You can also directly open the project with Unity 2021.3.45f2, as the environment is already configured in the project, and run it directly.

**3. Open Project**

1. Launch Unity Hub
2. Click "Open" → Select `Unity/` folder
3. Wait for Unity to import project resources (first time opening takes longer)

**4. Configure VR Device**

1. Connect Meta Quest device to computer (USB or Air Link)
2. Unity menu: `Edit` → `Project Settings` → `XR Plug-in Management`
3. Check `Oculus` or `OpenXR`
4. Confirm device is running in developer mode

**5. Serial Communication Configuration**

Configure Arduino serial parameters in relevant scripts PullInput.cs/PressInput.cs/AngleInput.cs:

```csharp
string portName = "COM3";  // Modify according to actual port
int baudRate = 9600;
```

![image-20260128160000383](https://raw.githubusercontent.com/zheliku/TyporaImgBed/main/ImgBed202601281600512.png)

Check the actual COM port used by Arduino through Device Manager.

**6. Select Mode**

Click on the Main object to set interaction method:

- Virtual Hands: Virtual hand interaction
- Real Objects: Physical object interaction

![image-20260128160114340](https://raw.githubusercontent.com/zheliku/TyporaImgBed/main/ImgBed202601281601381.png)

**7. Run Project**

1. Open main scene: `Assets/Scenes/Main.unity`
2. Click Play button or press `Ctrl+P`
3. Put on VR headset for interaction testing

> If no headset is available, you can also run on desktop:
>
> - Press space to release blocks
> - Press R key to reset position
> - Drag slider to control timeline
>
>   <img src="https://raw.githubusercontent.com/zheliku/TyporaImgBed/main/ImgBed202601281640939.png" alt="image-20260128164048868" style="zoom:50%;" />

## Complete Workflow

### Hardware Preparation

1. Use STL files in `Models/for 3d print/` for 3D printing
2. Assemble hardware components
3. Connect pressure/tension sensors to Arduino
4. Upload corresponding `.ino` program to Arduino

### Software Configuration

1. Confirm Arduino is connected to computer via USB and working normally
2. Use serial monitor to test sensor data output
3. Open Unity project and configure serial parameters

### Test Run

1. Launch Unity project
2. Confirm VR device is connected
3. Run scene
4. Operate physical hardware, observe interaction feedback in VR

## Citation

If this project helps your research, please cite our paper (paper information to be added).

---

**Update Date**: January 28, 2026
