using Microsoft.Extensions.Logging;
using RyzeTelloSDK.Core;
using RyzeTelloSDK.Enum;
using RyzeTelloSDK.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using RyzeTelloSDKintegration.Core;
using Console = Colorful.Console;

namespace TelloTestApp
{
    public class ConsoleWorker
    {
        private static readonly Dictionary<ConsoleKey, MoveDirection> MoveMappings = new Dictionary<ConsoleKey, MoveDirection>()
        {
            { ConsoleKey.W, MoveDirection.Forward },
            { ConsoleKey.S, MoveDirection.Back },
            { ConsoleKey.A, MoveDirection.Left },
            { ConsoleKey.D, MoveDirection.Right },
            { ConsoleKey.R, MoveDirection.Up },
            { ConsoleKey.F, MoveDirection.Down },
        };

        // private readonly ILogger logger;
        private readonly TelloCore _telloCore;
        private readonly TelloClient client;
        // private readonly GamePadController gamePad;

        private bool gamePadEnabled;

        // public ConsoleWorker(Core core, TelloClient client, GamePadController gamePad, ILogger<ConsoleWorker> logger)
        public ConsoleWorker(TelloCore telloCore, TelloClient client)
        {
            this._telloCore = telloCore;
            // this.logger = logger;
            this.client = client;
            // this.gamePad = gamePad;

            gamePadEnabled = false;
        }

        public async Task MainLoop()
        {
            //await _telloCore.Init();
            // start readkey backgroundworker
            
            // RenderConsoleLoop();
            var shouldLoop = true;
            while (shouldLoop)
            {
                try
                {
                    var key = Console.ReadKey(true);
                    Console.WriteLine($"{key.Key} pressed");
                    // logger.LogInformation($"{key.Key} pressed");
                    switch (key.Key)
                    {
                        case ConsoleKey.W:
                        case ConsoleKey.S:
                        case ConsoleKey.A:
                        case ConsoleKey.D:
                        case ConsoleKey.R:
                        case ConsoleKey.F:
                            // logger.LogInformation($"FlyDirection({MoveMappings[key.Key]}, 30)");
                            await client.FlyDirection(MoveMappings[key.Key], 30);
                            break;

                        case ConsoleKey.Q:
                        case ConsoleKey.E:
                            // logger.LogInformation($"RotateDirection({(key.Key == ConsoleKey.E ? "cw" : "ccw")}, 20)");
                            await client.RotateDirection(key.Key == ConsoleKey.E, 20);
                            break;

                        case ConsoleKey.T:
                            // logger.LogInformation("TakeOff()");
                            await client.TakeOff();
                            break;

                        case ConsoleKey.L:
                            // logger.LogInformation("Land()");
                            await client.Land();
                            break;

                        case ConsoleKey.Spacebar:
                            // logger.LogInformation("Emergency()");
                            await client.Emergency();
                            break;
                        
                        case ConsoleKey.Escape:
                            // logger.LogInformation("Closing all of the connections");
                            _telloCore.Close();
                            shouldLoop = false;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    // logger.LogError(ex, "Exception while proccessing key press");
                }
            }
            // logger.LogInformation("Exiting loop");
        }

        private async void RenderConsoleLoop()
        {
            RenderConsole(true);
            while (true)
            {
                await Task.Delay(250); // 0.25s
                RenderConsole();
            }
        }

        private void RenderConsole(bool firstTime = true)
        {
            var state = _telloCore.GetState();
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
            Console.SetCursorPosition(16, 1);
            Console.Write($"{state.AccelerationX,6:0} {state.AccelerationY,6:0} {state.AccelerationZ,6:0}");
            Console.SetCursorPosition(16, 2);
            Console.Write($"{state.VelocityX,6:0} {state.VelocityY,6:0} {state.VelocityZ,6:0}");
            Console.SetCursorPosition(57, 1);
            Console.Write($"{state.TempLowest,3}", GetTempColor(state.TempLowest));
            Console.Write($"   {state.TempHighest,3}", GetTempColor(state.TempHighest));
            Console.SetCursorPosition(74, 1);
            Console.Write($"{state.Pitch,5}  {state.Roll,4}  {state.Yaw,3}");
            Console.SetCursorPosition(44, 3);
            Console.Write($"{state.TOF,3}  {state.Height,4}cm  {state.Battery,6}%  {state.Barometer,9:0.000}");
            Console.SetCursorPosition(6, 3);
            Console.Write($"{state.Time}s");
            Console.SetCursorPosition(9, 4);
            if (gamePadEnabled)
            {
                Console.Write("ON ", Color.Green);
            }
            else
            {
                Console.Write("OFF", Color.Red);
            }

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
}
