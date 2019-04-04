using System;
using System.Collections.Generic;
using System.Text;

using PostSharp.Patterns.Diagnostics ;
using PostSharp.Patterns.Diagnostics.Contexts ;

namespace SharpCrafting
{
    class FaultLogHandler : ILoggingExceptionHandler
    {
        public void OnInternalException ( LoggingExceptionInfo exceptionInfo )
        {
            throw new Exception("Internal logging exception.", exceptionInfo.Exception);
        }

        public void OnInvalidUserCode ( ref CallerInfo callerInfo, LoggingTypeSource source, string message, params object[] args )
        {
            throw new InvalidOperationException(string.Format(message, args));
        }

        public static void Initialize()
        {
            LoggingServices.ExceptionHandler = new FaultLogHandler();
        }
    }
}
