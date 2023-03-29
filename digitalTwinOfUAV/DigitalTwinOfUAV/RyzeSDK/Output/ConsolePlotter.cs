using System.Drawing;
using RyzeTelloSDK.Core;
using RyzeTelloSDK.Models;
using Console = Colorful.Console;

namespace RyzeTelloSDKintegration.FlightManagementSystem;

public class ConsoleDisplay
{
    public ConsoleDisplay(TelloStateServer stateServer)
    {
        RenderConsole(new TelloStateParameter());
        stateServer.OnState += (s) => RenderConsole(s, false);
    }

    private void RenderConsole(TelloStateParameter stateParameter, bool firstTime = true)
    {
        if (firstTime)
        {
            // Console.SetWindowSize(90, 9); funktioniert nicht
            Console.SetCursorPosition(15, 0);
            Console.Write("      X      Y      Z                    Low. High. °C     Pitch  Roll  Yaw");
            Console.SetCursorPosition(0, 1);
            Console.Write("Acceleration:                              Temperature:");
            Console.SetCursorPosition(0, 2);
            Console.Write("Velocity:                                   TOF  Height  Battery  Barometer");
            Console.SetCursorPosition(0, 3);
            Console.Write("Time:");
            Console.SetCursorPosition(0, 4);
            Console.Write("Gamepad:");
            Console.SetCursorPosition(0, 8);
            Console.Write(">");
        }
        // ersetze nur die Werte anstatt das Interface komplett neu zu erstellen. 
        Console.SetCursorPosition(16, 1);
        Console.Write($"{stateParameter.AccelerationX,6:0} {stateParameter.AccelerationY,6:0} {stateParameter.AccelerationZ,6:0}");
        Console.SetCursorPosition(16, 2);
        Console.Write($"{stateParameter.VelocityX,6:0} {stateParameter.VelocityY,6:0} {stateParameter.VelocityZ,6:0}");
        Console.SetCursorPosition(57, 1);
        Console.Write($"   {stateParameter.TempLowest,3}", GetTempColor(stateParameter.TempLowest));
        Console.Write($"   {stateParameter.TempHighest,3}", GetTempColor(stateParameter.TempHighest));
        Console.SetCursorPosition(74, 1);
        Console.Write($"{stateParameter.Pitch,5}  {stateParameter.Roll,4}  {stateParameter.Yaw,3}");
        Console.SetCursorPosition(44, 3);
        Console.Write($"{stateParameter.TOF,3}  {stateParameter.Height,4}cm  {stateParameter.Battery,6}%  {stateParameter.Barometer,9:0.000}");
        Console.SetCursorPosition(6, 3);
        Console.Write($"{stateParameter.Time}s");
        Console.SetCursorPosition(9, 4);

        Console.SetCursorPosition(2, 8);
    }
    
    private Color GetTempColor(int temp)
    {
        if (temp > 80) return Color.Red;
        if (temp > 60) return Color.Orange;
        if (temp > 50) return Color.Yellow;
        return Color.Green;
    }
}