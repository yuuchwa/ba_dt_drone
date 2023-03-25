using RyzeTelloSDK.Core;
using RyzeTelloSDK.Enum;
using System.Threading.Tasks;

namespace RyzeTelloSDK.Extensions
{
    /// <summary>
    /// This class manages all direct commands to the tello drone.
    /// </summary>
    public static class TelloContolExtensions
    {
        /// <summary>
        /// Set tello to SDK mode.
        /// </summary>
        /// <param name="telloClient">Upd server connected to tello</param>
        /// <returns></returns>
        public static Task<bool> InitTello(this ITelloClient tello)
        {
            return tello.SendCommandWithResponse("command");
        }
        
        /// <summary>
        /// Command drone to Take off.
        /// </summary>
        /// <param name="telloClient">Upd server connected to tello</param>
        /// <returns>The response</returns>
        public static Task<bool> TakeOff(this ITelloClient tello)
        {
            return tello.SendCommandWithResponse("takeoff");
        }
        
        /// <summary>
        /// Command drone to land.
        /// </summary>
        /// <param name="telloClient">Upd server connected to tello</param>
        /// <returns>The response</returns>
        public static Task<bool> Land(this ITelloClient tello)
        {
            return tello.SendCommandWithResponse("land");
        }

        /// <summary>
        /// Command drone to activate video stream.
        /// </summary>
        /// <param name="telloClient">Upd server connected to tello</param>
        /// <returns></returns>
        public static Task<bool> StreamOn(this ITelloClient tello)
        {
            return tello.SendCommandWithResponse("streamon");
        }

        /// <summary>
        /// Command drone to deactivate video stream.
        /// </summary>
        /// <param name="telloClient">Upd server connected to tello</param>
        /// <returns></returns>
        public static Task<bool> StreamOff(this ITelloClient tello)
        {
            return tello.SendCommandWithResponse("streamoff");
        }

        /// <summary>
        /// Command drone to land immediately.
        /// </summary>
        /// <param name="telloClient">Upd server connected to tello</param>
        /// <returns></returns>
        public static Task<bool> Emergency(this ITelloClient tello)
        {
            return tello.SendCommandWithResponse("emergency");
        }

        /// <summary>
        /// Command drone to hover.
        /// </summary>
        /// <param name="telloClient">Upd server connected to tello</param>
        /// <returns></returns>
        public static Task<bool> StopAction(this ITelloClient tello)
        {
            return tello.SendCommandWithResponse("stop");
        }

        /// <summary>
        /// Command drone to hover.
        /// </summary>
        /// <param name="telloClient">Upd server connected to tello.</param>
        /// <param name="direction">the direction in which the drone should fly.</param>
        /// <param name="distance">The distance in which the drone should fly in centimeter.</param>
        /// <returns></returns>
        public static Task<bool> FlyDirection(this ITelloClient tello, MoveDirection direction, int cm)
        {
            CommandConstraints.CheckDistance(cm);
            return tello.SendCommandWithResponse($"{direction.ToString().ToLower()} {cm}");
        }

        /// <summary>
        /// Command drone to rotate.
        /// </summary>
        /// <param name="telloClient">Upd server connected to tello</param>
        /// <param name="clockwise">Where the drone should rotate clockwise.</param>
        /// <param name="degree">The degree in which the drone should rotate.</param>
        /// <returns></returns>
        public static Task<bool> RotateDirection(this ITelloClient tello, bool clockwise, int degree)
        {
            CommandConstraints.CheckDegree(degree);
            return tello.SendCommandWithResponse($"{(clockwise ? "cw" : "ccw")} {degree}");
        }

        /// <summary>
        /// Command drone to hover.
        /// </summary>
        /// <param name="telloClient">Upd server connected to tello</param>
        /// <param name="direction">The direction in which the drone should do the flip.</param>
        /// <returns></returns>
        public static Task<bool> Flip(this ITelloClient tello, FlipDirection direction)
        {
            return tello.SendCommandWithResponse($"flip {direction.ToString().ToLower()[0]}");
        }

        // ToDo: FlyTo overload "mid"
        /// <summary>
        /// Command to fly to a specific position with a specific speed.
        /// </summary>
        /// <param name="telloClient">Upd server connected to tello</param>
        /// <param name="x">X Position</param>
        /// <param name="y">Y Position</param>
        /// <param name="z">Z Position</param>
        /// <param name="speed">Speed</param>
        /// <returns>Drone response</returns>
        public static Task<bool> FlyTo(this ITelloClient tello, int x, int y, int z, int speed)
        {
            CommandConstraints.CheckDistance(x);
            CommandConstraints.CheckDistance(y);
            CommandConstraints.CheckDistance(z);
            CommandConstraints.CheckSpeed(speed);
            return tello.SendCommandWithResponse($"go {x} {y} {z} {speed}");
        }

        // ToDo: Curve overload "mid"
        /// <summary>
        /// Command the drone to fly at a curve according to the two given coordinate at speed (cm/s).
        /// </summary>
        /// <param name="telloClient"></param>
        /// <param name="x1">x1 Coordinate</param>
        /// <param name="y1">y1 Coordinate</param>
        /// <param name="z1">z1 Coordinate</param>
        /// <param name="x2">x2 Coordniate</param>
        /// <param name="y2">y2 Coordinate</param>
        /// <param name="z2">z2 Coordinate</param>
        /// <param name="speed">The speed in cm/s</param>
        /// <returns></returns>
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

            return tello.SendCommandWithResponse($"curve {x1} {y1} {z1} {x2} {y2} {z2} {speed}");
        }

        // ToDo: Jump
    }
}
