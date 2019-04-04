using System;
using System.IO ;
using System.Reflection ;

using JetBrains.Annotations ;

using PostSharp.Patterns.Diagnostics ;
using PostSharp.Patterns.Diagnostics.Backends.Serilog ;
using PostSharp.Patterns.Model ;

using Serilog ;

namespace SharpCrafting
{
    class Program
    {
        [NotNull, Reference] private static readonly string RuntimeUri;
        [NotNull, Reference] private static readonly Assembly Assembly;

        [NotNull, Reference]
        private const string Template =
            "{Timestamp:yyyy-MM-dd HH:mm:ss} |{Level:u3}: [{ThreadId}:{SourceContext}]{Indent:l} {Message:lj}{NewLine}{Exception}";

        static Program ()
        {
            RuntimeUri = Runtime.GetRuntimeUri () ;
            Assembly = Runtime.GetRuntimeAssembly () ;
        }

        static void Main(string[] args)
        {
            FaultLogHandler.Initialize();
            var log = MetaLogger () ;
            LoggingServices.Roles[LoggingRoles.Meta].Backend = new SerilogLoggingBackend(log.ForContext("Meta", "PostSharp"));
            Console.WriteLine("Hello World!");
            var runtime = new Runtime();
            runtime.Entry();
            Console.WriteLine("Press any key to stop.");
            Console.ReadKey () ;
        }

        private static ILogger MetaLogger()
        {
            var config = new LoggerConfiguration()
                        .MinimumLevel.Verbose()
                        .Enrich.FromLogContext()
                        .Enrich.WithThreadId()
                        .WriteTo.File(path: $@"{RuntimeUri}\meta.log", outputTemplate: Template);
            return config.CreateLogger();
        }
    }
}
