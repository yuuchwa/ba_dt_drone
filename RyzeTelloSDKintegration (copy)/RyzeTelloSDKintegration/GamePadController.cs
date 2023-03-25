using Microsoft.Extensions.Logging;
using RyzeTelloSDK.Core;
using RyzeTelloSDK.Extensions;
using SharpDX.DirectInput;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TelloTestApp
{
    public class GamePadController
    {
        private static readonly int MaxInputValue = (int)Math.Pow(2, 16);
        private static readonly int DeadZone = 10;

        // private readonly ILogger logger;
        private readonly Core core;
        private readonly TelloClient client;

        private Joystick gamepad;
        private Task mainLoop;
        private CancellationTokenSource cts;

        public GamePadController(Core core, TelloClient client, ILogger<GamePadController> logger)
        {
            this.core = core;
            //  this.logger = logger;
            this.client = client;

            Init();
        }

        public void Close()
        {
            if (cts == null) return;
            cts.Cancel();
        }

        public bool Listen()
        {
            if (gamepad == null) return false;
            cts = new CancellationTokenSource();
            mainLoop = Task.Run(ListenTask, cts.Token);
            return true;
        }

        private async void ListenTask()
        {
            var lastRcInput = new RcInput();
            while (true)
            {
                await Task.Delay(100); // might wanna remove it; causes slight input delay
                try
                {
                    var data = gamepad.GetCurrentState();
                    var newRcInput = RcInput.ParseFromGamePad(data);
                    if (newRcInput.Equals(lastRcInput)) continue;

                    lastRcInput = newRcInput;
                    // logger.LogInformation($"RC({newRcInput.RightLeft} {newRcInput.ForwardBack} {newRcInput.UpDown} {newRcInput.Yaw})");
                    // do not wait for response
                    _ = client.RC(newRcInput.RightLeft, newRcInput.ForwardBack, newRcInput.UpDown, newRcInput.Yaw);
                }
                catch (Exception ex)
                {
                    // logger.LogError(ex, "Exception while proccessing gamepad controls");
                }
            }
        }

        private void Init()
        {
            var directInput = new DirectInput();
            // PS4 controller with Steam driver is considered FirstPerson device type for some reason
            // so you might wanna to change this in order to support your controller
            var device = directInput.GetDevices(DeviceType.FirstPerson, DeviceEnumerationFlags.AllDevices).FirstOrDefault();
            if (device == null) return;

            gamepad = new Joystick(directInput, device.InstanceGuid);
            gamepad.Acquire();
        }

        private struct RcInput
        {
            public int RightLeft;
            public int ForwardBack;
            public int UpDown;
            public int Yaw;

            public static RcInput ParseFromGamePad(JoystickState state)
            {
                return new RcInput
                {
                    RightLeft = ConvertInput(state.X),
                    ForwardBack = ConvertInput(state.Y, true),
                    UpDown = ConvertInput(state.RotationZ, true),
                    Yaw = ConvertInput(state.Z)
                };
            }

            // converts 0 - 65536 values to -100 - 100 range
            private static int ConvertInput(int value, bool invert = false)
            {
                var converted = (value * 201 / MaxInputValue) - 100;
                if (invert) converted *= -1;
                return converted < DeadZone && converted > -DeadZone ? 0 : converted;
            }
        }
    }
}
