using System.Threading;
using DtTelloDrone.RyzeSDK.Models;
using NUnit.Framework;

namespace DtTelloDrone.Tests;

public class TelloStateParameterTest
{
    [Test]
    public void TestConverToCsv()
    {
        string rawData =
            "pitch:-1;roll:0;yaw:-81;vgx:0;vgy:-5;vgz:0;templ:71;temph:75;tof:78;h:30;bat:66;baro:-59.19;time:11;agx:-19.00;agy:-36.00;agz:-1025.00;";
        var stateData = TelloStateParameter.FromString(rawData);
        var data1 = stateData.ConvertToCsv();
        
        rawData =  "pitch:-1;roll:0;yaw:-81;vgx:0;vgy:-5;vgz:0;templ:71;temph:75;tof:78;h:30;bat:66;baro:-59.19;time:11;agx:-19.00;agy:-36.00;agz:-1025.00;";
        var stateData2 = TelloStateParameter.FromString(rawData);
        var data2 = stateData.ConvertToCsv();
    }
}