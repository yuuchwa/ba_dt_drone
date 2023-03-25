using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace RyzeTelloSDK.Models
{
    public class TelloState
    {
        private class NameAttribute : Attribute
        {
            public string Name { get; private set; }

            public NameAttribute(string name)
            {
                Name = name;
            }
        }

        [Name("pitch")]
        public int Pitch { get; private set; }
        [Name("roll")]
        public int Roll { get; private set; }
        [Name("yaw")]
        public int Yaw { get; private set; }

        [Name("vgx")]
        public int VelocityX { get; private set; }
        [Name("vgy")]
        public int VelocityY { get; private set; }
        [Name("vgz")]
        public int VelocityZ { get; private set; }

        [Name("templ")]
        public int TempLowest { get; private set; }
        [Name("temph")]
        public int TempHighest { get; private set; }

        [Name("tof")]
        public int TOF { get; private set; }
        [Name("h")]
        public int Height { get; private set; }
        [Name("bat")]
        public int Battery { get; private set; }
        [Name("baro")]
        public float Barometer { get; private set; }
        [Name("time")]
        public int Time { get; private set; }

        [Name("agx")]
        public float AccelerationX { get; private set; }
        [Name("agy")]
        public float AccelerationY { get; private set; }
        [Name("agz")]
        public float AccelerationZ { get; private set; }

        private static readonly Regex regex = new Regex(@"(\w+):([\d.-]+)");
        private static readonly PropertyInfo[] props = typeof(TelloState).GetProperties();
        private static readonly Type intType = typeof(int);
        private static readonly Type floatType = typeof(float);

        public static TelloState FromString(string data)
        {
            var state = new TelloState();

            var matches = regex.Matches(data);

            var results = new Dictionary<string, string>();
            foreach (Match match in matches)
            {
                results.Add(match.Groups[1].Value, match.Groups[2].Value);
            }

            foreach (var prop in props)
            {
                var attr = prop.GetCustomAttribute<NameAttribute>();
                if (attr == null) continue;
                if (!results.ContainsKey(attr.Name)) continue;

                if (prop.PropertyType == intType)
                {
                    prop.SetValue(state, int.Parse(results[attr.Name]));
                }
                else if (prop.PropertyType == floatType)
                {
                    prop.SetValue(state, float.Parse(results[attr.Name], CultureInfo.InvariantCulture.NumberFormat));
                }
            }

            return state;
        }
    }
}
