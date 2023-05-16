using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DtTelloDrone.TelloSdk.DataModels;

/// <summary>
/// Represents the drone state.
/// </summary>
public class TelloStateParameter
{
    private class NameAttribute : System.Attribute
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

    public DateTime TimeStamp { get; set; }
    
    private static readonly Regex regex = new Regex(@"(\w+):([\d.-]+)");
    private static readonly PropertyInfo[] props = typeof(TelloStateParameter).GetProperties();
    private static readonly Type intType = typeof(int);
    private static readonly Type floatType = typeof(float);

    public static TelloStateParameter FromString(string data)
    {
        var state = new TelloStateParameter();

        state.TimeStamp = DateTime.Now;

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

    private bool _noHeader = true;
    
    public string ConvertToCsv()
    {
        Type type = GetType();
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

        string result = "";
        foreach (PropertyInfo property in properties)
        {
            result += $"{property.GetValue(this)};";
        }

        if (result.Length > 0)
        {
            result = result.Remove(result.Length - 2); // entfernt das letzte Komma und Leerzeichen
        }

        return result;
    }
}
