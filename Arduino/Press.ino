const int sensorPin = A0;  // 将传感器连接到Analog 0引脚
int sensorValue;          // 存储传感器读数

void setup() {
  Serial.begin(9600);  // 初始化串口通信，设置波特率为9600
}

void loop() {
  sensorValue = analogRead(sensorPin);  // 读取传感器数值
  // Serial.print("Sensor Value: ");
  Serial.println(sensorValue);          // 将传感器数值输出到串口
  delay(100);                           // 等待100毫秒再次读取
}
