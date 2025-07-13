public enum SensorUpdateMode
{
    EveryFrame,    // 60 FPS - critical situations
    Reduced,       // 30 FPS - normal gameplay  
    Minimal,       // 10 FPS - idle/safe situations
    Disabled       // Completely off
}
