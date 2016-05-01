using System;
using UnityEngine;

namespace uFrame2
{
    public class TemproraryLoggerHandler : ILogHandler
    {

        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            var date = DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
            Debug.logger.logHandler.LogFormat(logType, context, "["+date+"] "+format, args);
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
            Debug.logger.LogException(exception, context);
        }

    }
}