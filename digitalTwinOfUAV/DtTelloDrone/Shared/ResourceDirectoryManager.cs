using System;
using System.IO;
using System.Text;

namespace DtTelloDrone.Shared;

public class ResourceDirectoryManager
{
    private static ResourceDirectoryManager _manager;
    private readonly string DirectoryPath =
        "/home/leon/Documents/Studium/Bachelorarbeit/ba_dt_drone/digitalTwinOfUAV/DtTelloDrone/OutputResources/";

    private readonly string KeyboardInputFilePath = "KeyboardInput.csv";
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

        DirectoryPath = DirectoryPath + dateFolder + sessionFolder;
        new DirectoryInfo(DirectoryPath).Create();

        KeyboardInputFilePath = DirectoryPath + KeyboardInputFilePath;
    }
    
    public void AppendToKeyboardInputFile(string record)
    {
        if (!File.Exists(KeyboardInputFilePath))
        {
            _keyboardInputFile = File.Open(KeyboardInputFilePath, FileMode.Create, FileAccess.Write);
            byte[] bytes = "time;input\n"u8.ToArray();
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
        }
    }
}