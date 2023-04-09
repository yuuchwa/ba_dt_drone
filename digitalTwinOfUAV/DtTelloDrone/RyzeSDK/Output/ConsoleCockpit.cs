using System.Drawing;
using DtTelloDrone.RyzeSDK.Core;
using DtTelloDrone.RyzeSDK.Models;
using Console = Colorful.Console;

namespace DtTelloDrone.RyzeSDK.Output;

public class ConsoleCockpit
{
    public ConsoleCockpit(TelloStateServer stateServer)
    {
        RenderConsole(new TelloStateParameter());
        stateServer.OnState += (s) => RenderConsole(s, false);
    }

    private void RenderConsole(TelloStateParameter state, bool firstTime = true)
    {
        if (firstTime)
        {
            // Set header
            Console.SetCursorPosition(15, 1);
            Console.Write("      X      Y      Z                    Low. High. °C     Pitch  Roll  Yaw");
            Console.SetCursorPosition(0, 2);
            Console.Write("Acceleration:                              Temperature:");
            Console.SetCursorPosition(0, 3);
            Console.Write("Velocity:");
            Console.SetCursorPosition(0, 4);
            Console.Write("Time                                       TOF  Height  Battery  Barometer");
            Console.SetCursorPosition(0, 5);
            Console.Write(">");
        }
        // Set Data
        Console.SetCursorPosition(16, 2);
        Console.Write($"{state.AccelerationX,6:0} {state.AccelerationY,6:0} {state.AccelerationZ,6:0}");
        Console.SetCursorPosition(16, 3);
        Console.Write($"{state.VelocityX,6:0} {state.VelocityY,6:0} {state.VelocityZ,6:0}");
        Console.SetCursorPosition(55, 2);
        Console.Write($"{state.TempLowest,3}", GetTempColor(state.TempLowest));
        Console.Write($"   {state.TempHighest,3}", GetTempColor(state.TempHighest));
        Console.SetCursorPosition(72, 2);
        Console.Write($"{state.Pitch,5}  {state.Roll,4}   {state.Yaw,3}");
        Console.SetCursorPosition(42, 5);
        Console.Write($"{state.TOF,3}  {state.Height,4}cm {state.Battery,6}%  {state.Barometer,9:0.000}");
        Console.SetCursorPosition(6, 4);
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