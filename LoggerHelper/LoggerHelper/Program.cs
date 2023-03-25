using System.Reflection;
using log4net;
using log4net.Config;

public class Program
{
    private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    
    static void Main(string[] args)
    {
        var displayMessage = "hallo Wokfasdf";
        
        Console.WriteLine(displayMessage);
        
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        _log.Info(displayMessage);
    }
}