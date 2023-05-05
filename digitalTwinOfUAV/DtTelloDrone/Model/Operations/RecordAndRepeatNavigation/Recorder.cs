namespace DtTelloDrone.Model.Operations.RecordAndRepeatNavigation;

public class Recorder
{
    private static Recorder _recorder;
    
    public static Recorder GetRecorder()
    {
        return _recorder ??= new Recorder();
    }
}