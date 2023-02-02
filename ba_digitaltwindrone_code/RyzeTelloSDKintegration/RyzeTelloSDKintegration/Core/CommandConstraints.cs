﻿using System;

namespace RyzeTelloSDK.Core
{
    public static class CommandConstraints
    {
        public const int DegreeMin = 1;
        public const int DegreeMax = 3600;

        public const int SpeedMin = 10;
        public const int SpeedMax = 100;
        public const int CurveSpeedMax = 60;

        public const int DistanceMin = 20;
        public const int DistanceMax = 500;

        public const int RCMin = -100;
        public const int RCMax = 100;

        public static void CheckDegree(int value)
        {
            if (value < DegreeMin || value > DegreeMax) throw new ArgumentException($"Degree should be between {DegreeMin} and {DegreeMax}", nameof(value));
        }

        public static void CheckSpeed(int value)
        {
            if (value < SpeedMin || value > SpeedMax) throw new ArgumentException($"Speed should be between {SpeedMin} and {SpeedMax}", nameof(value));
        }

        public static void CheckCurveSpeed(int value)
        {
            if (value < SpeedMin || value > CurveSpeedMax) throw new ArgumentException($"Speed should be between {SpeedMin} and {CurveSpeedMax}", nameof(value));
        }

        public static void CheckDistance(int value)
        {
            if (value < DistanceMin || value > DistanceMax) throw new ArgumentException($"Distance should be between {DistanceMin} and {DistanceMax}", nameof(value));
        }

        public static void CheckRC(int value)
        {
            if (value < -100 || value > 100) throw new ArgumentException($"RC should be between {RCMin} and {RCMax}", nameof(value));
        }
    }
}
