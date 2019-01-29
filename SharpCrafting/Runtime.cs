using System;
using System.Collections.Generic;
using System.IO ;
using System.Reflection ;
using System.Text;

using JetBrains.Annotations ;

using PostSharp ;
using PostSharp.Patterns.Diagnostics ;
using PostSharp.Patterns.Diagnostics.Backends.Serilog ;
using PostSharp.Patterns.Model ;
using PostSharp.Patterns.Threading ;

using Serilog ;

namespace SharpCrafting
{
    [Freezable]
    public sealed class Runtime
    {
        static Runtime()
        {
            Platform       = new GenericPlatform();
            Assembly       = GetAssembly(typeof(Runtime));
            ApplicationUri = Path.GetDirectoryName(Assembly.Location);
        }

        [Pure]
        private static Assembly GetAssembly<T>(T type) => type.GetType().GetTypeInfo().Assembly;

        [NotNull, Reference] private static readonly Assembly           Assembly;
        [NotNull, Reference] private static readonly string             ApplicationUri;
        [NotNull, Child]     private static readonly GenericPlatform     Platform;
        [NotNull, Reference] private readonly        ILogger            _log = Log.ForContext<Runtime>();

        [NotNull,Reference]
        private const string Template =
            "{Timestamp:yyyy-MM-dd HH:mm:ss} |{Level:u3}: [{ThreadId}:{SourceContext}]{Indent:l} {Message:lj}{NewLine}{Exception}";

        public void Entry()
        {
            try
            {
                var log = VerboseLogger();
                Log.Logger = log;
                LoggingServices.DefaultBackend =
                    new SerilogLoggingBackend(log.ForContext("RuntimeContext", "PostSharp"));
            }
            catch (Exception ex)
            {
                Platform.Crash();
            }

            try
            {
                Post.Cast<Runtime, IFreezable>(this).Freeze();
                Post.Cast<GenericPlatform, IFreezable>(Platform).Freeze();

                var cleanlyLaunch = Platform.Host.Start(Platform);
                var returned = cleanlyLaunch.ContinueWith((t) =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                        Platform
                           .Crash($"Fatal exception in the common platform at runtime{Environment.NewLine}{t.Exception?.ToString()}");
                    else
                        _log
                           .Information("Successfully launched the common platform from the bluetooth runtime.");
                });
                returned.Wait();

                //  Results in sink monitors receiving StopMonitoring calls.
                Log.CloseAndFlush();
            }
            catch (ObjectReadOnlyException roEx)
            {
                _log.Fatal($"Property or state were attempted on a frozen object." +
                             $"${Environment.NewLine}BluetoothRuntime Freeze(): {roEx.Data}");
                Platform.Crash("Fatal attempt to modify a frozen object.");
            }
            catch (ThreadMismatchException threadEx)
            {
                _log.Fatal($"An object was accessed from a different thread than it was created." +
                             $"{Environment.NewLine}Please use a threading model or freeze the object before crossing boundaries." +
                             $"{threadEx}");
                Platform.Crash("Fatal thread access exception was thrown.");
            }
            catch (Exception ex)
            {
                _log.Fatal($"Could not freeze and launch the common platform from the bluetooth runtime." +
                             $"{Environment.NewLine}{ex}");
                Platform.Crash("Unknown failure occurred in an EntryPointAttribute for the bluetooth runtime.");
            }
        }

        private static ILogger VerboseLogger()
        {
            var config = new LoggerConfiguration()
                        .MinimumLevel.Verbose()
                        .Enrich.FromLogContext()
                        .Enrich.WithThreadId()
                        .WriteTo.Console(outputTemplate: Template, theme: ConsoleExtensions.BlueConsole);
            return config.CreateLogger();
        }
    }
}
