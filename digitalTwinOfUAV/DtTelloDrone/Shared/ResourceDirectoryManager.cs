using System;
using System.IO;
using System.Text;

namespace DtTelloDrone.Shared;

public class ResourceDirectoryManager
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private static ResourceDirectoryManager _manager;
    private readonly string _directoryPath =
        "/home/leon/Documents/Studium/Bachelorarbeit/ba_dt_drone/digitalTwinOfUAV/DtTelloDrone/OutputResources/";

    private readonly string _keyboardInputFilePath = "KeyboardInput.csv";
    private static FileStream _keyboardInputFile;
    
    public static ResourceDirectoryManager GetDirectoryManager()
    {
        return _manager ??= new();
    }

    private ResourceDirectoryManager()
    {
        var now = DateTime.Now;
        string dateFolder = "Resources." + now.ToString("yyyy-MM-dd") + "/";
        string sessionFolder = "Session." + now.ToString("HH-mm-ss") + "/";

        _directoryPath = _directoryPath + dateFolder + sessionFolder;
        new DirectoryInfo(_directoryPath).Create();

        _keyboardInputFilePath = _directoryPath + _keyboardInputFilePath;
    }
    
    public void AppendToKeyboardInputFile(string record)
    {
        if (!File.Exists(_keyboardInputFilePath))
        {
            _keyboardInputFile = File.Open(_keyboardInputFilePath, FileMode.Create, FileAccess.Write);
            byte[] bytes = "time;input;xPos;yPos;zPos\n"u8.ToArray();
            _keyboardInputFile.Write(bytes);
        }

        lock (this)
        {
            if (_keyboardInputFile != null)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(record);
                _keyboardInputFile.Write(bytes, 0, bytes.Length); 
            }
        }
    }

    public static void Close()
    {
        if (_keyboardInputFile != null)
        {
            _keyboardInputFile.Dispose();
            _keyboardInputFile = null;
            Logger.Info("Resource Directory Manager disposed");
        }
    }
}