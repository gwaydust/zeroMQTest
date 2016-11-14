using System;
using System.IO;

namespace PullPushWorker
{
	/// <summary>
	/// Description of Logger.
	/// </summary>

public static class Logger	
{	
	private static readonly object locker = new object();
	private static string logFileName = "./Log/application.log";
	
	public static void logFile(string filename) {
		logFileName = filename;
	}
	
    public static void Error(string message, string module)
    {
        WriteEntry(message, "error", module);
    }

    public static void Error(Exception ex, string module)
    {
        WriteEntry(ex.Message, "error", module);
    }

    public static void Warning(string message, string module)
    {
        WriteEntry(message, "warning", module);
    }

    public static void Info(string message, string module)
    {
        WriteEntry(message, "info", module);
    }

    private static void WriteEntry(string message, string type, string module)
    {    	    	    
    	lock(locker) {
    		File.AppendAllText(logFileName,
				DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ", " +
				type + ", " + module + ", " + message + "\r\n");
    	}
    }
}
}
