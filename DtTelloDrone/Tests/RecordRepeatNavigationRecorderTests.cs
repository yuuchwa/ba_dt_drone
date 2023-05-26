using System.Reflection;
using DtTelloDrone.Model.Operations.RecordAndRepeatNavigation;
using NUnit.Framework;
using System.IO;

namespace DtTelloDrone.Tests;

[TestFixture]
public class RecordRepeatNavigationRecorderTests
{
    private string _testDirectoryPath;
    private string _testKeyboardInputFilePath;

    [SetUp]
    public void Setup()
    {
        _testDirectoryPath = Path.Combine(Path.GetTempPath(), "TestDirectory/");
        _testKeyboardInputFilePath = Path.Combine(_testDirectoryPath, "TestKeyboardInput.csv");
        Directory.CreateDirectory(_testDirectoryPath);
    }

    [TearDown]
    public void Cleanup()
    {
        if (File.Exists(_testKeyboardInputFilePath))
            File.Delete(_testKeyboardInputFilePath);

        if (Directory.Exists(_testDirectoryPath))
            Directory.Delete(_testDirectoryPath);
    }

    [Test]
    public void GetDirectoryManager_ReturnsSingletonInstance()
    {
        RecordRepeatNavigationRecorder instance1 = RecordRepeatNavigationRecorder.GetRecorder();
        RecordRepeatNavigationRecorder instance2 = RecordRepeatNavigationRecorder.GetRecorder();

        Assert.AreSame(instance1, instance2);
    }
    
    [Test]
    public void Close_DisposesKeyboardInputFileAndLogsMessage()
    {
        // Arrange
        RecordRepeatNavigationRecorder recorder = RecordRepeatNavigationRecorder.GetRecorder();
        using (FileStream fileStream = File.Create(_testKeyboardInputFilePath))
        {
            recorder.GetType().GetField("_keyboardInputFile", BindingFlags.NonPublic | BindingFlags.Static)
                .SetValue(recorder, fileStream);
        }

        // Act
        RecordRepeatNavigationRecorder.Close();

        // Assert
        Assert.IsNull(recorder.GetType().GetField("_keyboardInputFile", BindingFlags.NonPublic | BindingFlags.Static)
            .GetValue(recorder));
    }
}