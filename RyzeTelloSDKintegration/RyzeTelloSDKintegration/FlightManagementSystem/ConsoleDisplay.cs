using System.Drawing;
using RyzeTelloSDK.Core;
using RyzeTelloSDK.Models;
using Console = Colorful.Console;

namespace RyzeTelloSDKintegration.FlightManagementSystem;

public class ConsoleDisplay
{
    public ConsoleDisplay(TelloStateServer stateServer)
    {
        RenderConsole(new TelloState());
        //stateServer.OnState += (s) => RenderConsole(s, false);
    }

    private void RenderConsole(TelloState state, bool firstTime = true)
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
        Console.Write($"{state.AccelerationX,6:0} {state.AccelerationY,6:0} {state.AccelerationZ,6:0}");
        Console.SetCursorPosition(16, 2);
        Console.Write($"{state.VelocityX,6:0} {state.VelocityY,6:0} {state.VelocityZ,6:0}");
        Console.SetCursorPosition(57, 1);
        Console.Write($"   {state.TempLowest,3}", GetTempColor(state.TempLowest));
        Console.Write($"   {state.TempHighest,3}", GetTempColor(state.TempHighest));
        Console.SetCursorPosition(74, 1);
        Console.Write($"{state.Pitch,5}  {state.Roll,4}  {state.Yaw,3}");
        Console.SetCursorPosition(44, 3);
        Console.Write($"{state.TOF,3}  {state.Height,4}cm  {state.Battery,6}%  {state.Barometer,9:0.000}");
        Console.SetCursorPosition(6, 3);
        Console.Write($"{state.Time}s");
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