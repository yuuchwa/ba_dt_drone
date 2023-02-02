using RyzeTelloSDK.Core;
using RyzeTelloSDK.Enum;
using System.Threading.Tasks;

namespace RyzeTelloSDK.Extensions
{
    public static class TelloContolExtensions
    {
        public static Task<bool> Init(this ITelloClient tello)
        {
            return tello.SendAction("command");
        }

        public static Task<bool> TakeOff(this ITelloClient tello)
        {
            return tello.SendAction("takeoff");
        }

        public static Task<bool> Land(this ITelloClient tello)
        {
            return tello.SendAction("land");
        }

        public static Task<bool> StreamOn(this ITelloClient tello)
        {
            return tello.SendAction("streamon");
        }

        public static Task<bool> StreamOff(this ITelloClient tello)
        {
            return tello.SendAction("streamoff");
        }

        public static Task<bool> Emergency(this ITelloClient tello)
        {
            return tello.SendAction("emergency");
        }

        public static Task<bool> Stop(this ITelloClient tello)
        {
            return tello.SendAction("stop");
        }

        public static Task<bool> FlyDirection(this ITelloClient tello, MoveDirection direction, int cm)
        {
            CommandConstraints.CheckDistance(cm);
            return tello.SendAction($"{direction.ToString().ToLower()} {cm}");
        }

        public static Task<bool> RotateDirection(this ITelloClient tello, bool clockwise, int degree)
        {
            CommandConstraints.CheckDegree(degree);
            return tello.SendAction($"{(clockwise ? "cw" : "ccw")} {degree}");
        }

        public static Task<bool> Flip(this ITelloClient tello, FlipDirection direction)
        {
            return tello.SendAction($"flip {direction.ToString().ToLower()[0]}");
        }

        // ToDo: FlyTo overload "mid"
        public static Task<bool> FlyTo(this ITelloClient tello, int x, int y, int z, int speed)
        {
            CommandConstraints.CheckDistance(x);
            CommandConstraints.CheckDistance(y);
            CommandConstraints.CheckDistance(z);
            CommandConstraints.CheckSpeed(speed);
            return tello.SendAction($"go {x} {y} {z} {speed}");
        }

        // ToDo: Curve overload "mid"
        public static Task<bool> Curve(this ITelloClient tello, int x1, int y1, int z1, int x2, int y2, int z2, int speed)
        {
            CommandConstraints.CheckDistance(x1);
            CommandConstraints.CheckDistance(y1);
            CommandConstraints.CheckDistance(z1);
            CommandConstraints.CheckDistance(x2);
            CommandConstraints.CheckDistance(y2);
            CommandConstraints.CheckDistance(z2);
            CommandConstraints.CheckSpeed(speed);

            // ToDo: x/y/z can’t be between -20 20 at the same time.

            return tello.SendAction($"curve {x1} {y1} {z1} {x2} {y2} {z2} {speed}");
        }

        // ToDo: Jump
    }
}
