using System;

namespace RyzeTelloSDK.Core
{
    /**
     * Checks if the paramters used by the tello sdk is valid.
     */
    public static class CommandConstraints
    {
        private const int DegreeMin = 1;
        private const int DegreeMax = 3600;

        private const int SpeedMin = 10;
        private const int SpeedMax = 100;
        private const int CurveSpeedMax = 60;

        private const int DistanceMin = 20;
        private const int DistanceMax = 500;

        private const int RCMin = -100;
        private const int RCMax = 100;

        /// <summary>
        /// Check if the degree is within an allowed range.
        /// </summary>
        /// <param name="value">The value which is to be checked.</param>
        /// <exception cref="ArgumentException">Thrown if the value is not within the allowed range.</exception>
        public static void CheckDegree(int value)
        {
            if (value < DegreeMin || value > DegreeMax) throw new ArgumentException($"Degree should be between {DegreeMin} and {DegreeMax}", nameof(value));
        }

        /// <summary>
        /// Check if the degree is within an allowed range.
        /// </summary>
        /// <param name="value">The value which is to be checked.</param>
        /// <exception cref="ArgumentException">Thrown if the value is not within the allowed range.</exception>
        public static void CheckSpeed(int value)
        {
            if (value < SpeedMin || value > SpeedMax) throw new ArgumentException($"Speed should be between {SpeedMin} and {SpeedMax}", nameof(value));
        }
        
        /// <summary>
        /// Check if the degree is within an allowed range.
        /// </summary>
        /// <param name="value">The value which is to be checked.</param>
        /// <exception cref="ArgumentException">Thrown if the value is not within the allowed range.</exception>
        public static void CheckCurveSpeed(int value)
        {
            if (value < SpeedMin || value > CurveSpeedMax) throw new ArgumentException($"Speed should be between {SpeedMin} and {CurveSpeedMax}", nameof(value));
        }

        /// <summary>
        /// Check if the degree is within an allowed range.
        /// </summary>
        /// <param name="value">The value which is to be checked.</param>
        /// <exception cref="ArgumentException">Thrown if the value is not within the allowed range.</exception>
        public static void CheckDistance(int value)
        {
            if (value < DistanceMin || value > DistanceMax) throw new ArgumentException($"Distance should be between {DistanceMin} and {DistanceMax}", nameof(value));
        }

        /// <summary>
        /// Check if the degree is within an allowed range.
        /// </summary>
        /// <param name="value">The value which is to be checked.</param>
        /// <exception cref="ArgumentException">Thrown if the value is not within the allowed range.</exception>
        public static void CheckRC(int value)
        {
            if (value < RCMin || value > RCMax) throw new ArgumentException($"RC should be between {RCMin} and {RCMax}", nameof(value));
        }
    }
}
