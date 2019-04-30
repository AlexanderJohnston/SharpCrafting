using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Reflection ;
using System.Text ;

using JetBrains.Annotations ;

using PostSharp ;
using PostSharp.Patterns.Diagnostics ;
using PostSharp.Patterns.Diagnostics.Backends.Serilog ;
using PostSharp.Patterns.Model ;
using PostSharp.Patterns.Threading ;

using static PostSharp.Patterns.Diagnostics.FormattedMessageBuilder ;
using static PostSharp.Patterns.Diagnostics.SemanticMessageBuilder ;

using Pure = PostSharp.Patterns.Model.PureAttribute;

using Serilog ;

namespace SharpCrafting
{
    [ Freezable ]
    public sealed class Runtime
    {
        static Runtime ()
        {
            Assembly       = typeof( Runtime ).Assembly ;
            ApplicationUri = Path.GetDirectoryName ( Assembly.Location ) ;
            Platform = new GenericPlatform();

        }

        [ NotNull, Reference ] private static readonly Assembly        Assembly ;
        [ NotNull, Reference ] private static readonly string          ApplicationUri ;
        [ NotNull, Child ]     private static readonly GenericPlatform Platform ;

        [ NotNull, Reference ] private static readonly LogSource
            _logSource = LogSource.Get ().WithLevels ( LogLevel.Trace, LogLevel.Warning ) ;

        [ NotNull, Reference ] private readonly LogSource _log = _logSource.ForCurrentType () ;

        [ NotNull, Reference ] private const string Template =
            "{Timestamp:yyyy-MM-dd HH:mm:ss} |{Level:u3}: [{ThreadId}:{SourceContext}]{Indent:l} {Message:lj}{NewLine}{Exception}" ;

        public void Entry ()
        {
            try
            {
                var log = VerboseLogger () ;
                Log.Logger = log ;
                LoggingServices.DefaultBackend =
                    new SerilogLoggingBackend ( log.ForContext ( "RuntimeContext", "PostSharp" ) ) ;
            }
            catch ( Exception ex )
            {
                Platform.Crash () ;
            }

            try
            {
                Post.Cast <Runtime, IFreezable> ( this ).Freeze () ;
                Post.Cast <GenericPlatform, IFreezable> ( Platform ).Freeze () ;

                var cleanlyLaunch = Platform.Host.Start ( Platform ) ;
                var returned = cleanlyLaunch.ContinueWith ( ( t ) =>
                                                            {
                                                                if ( t.IsFaulted || t.IsCanceled )
                                                                    Platform
                                                                       .Crash ( $"Fatal exception in the common platform at runtime{Environment.NewLine}{t.Exception?.ToString ()}" ) ;
                                                                else
                                                                    _log
                                                                       .Default
                                                                       .Write ( Formatted ( "Successfully launched the common platform from the bluetooth runtime." ) ) ;
                                                            } ) ;
                returned.Wait () ;

                //  Results in sink monitors receiving StopMonitoring calls.
                Log.CloseAndFlush () ;
            }
            catch ( ObjectReadOnlyException roEx )
            {
                _log.Failure
                    .Write ( Formatted ( ( "Property or state were attempted on a frozen object.{NewLine}BluetoothRuntime Freeze(): {Data}" ),
                                         Environment.NewLine, roEx.Data ) ) ;
                Platform.Crash ( "Fatal attempt to modify a frozen object." ) ;
            }
            catch ( ThreadMismatchException threadEx )
            {
                _log.Failure
                    .Write ( Formatted ( "An object was accessed from a different thread than it was created.{NewLine}Please use a threading model or freeze the object before crossing boundaries {threadEx}",
                                         Environment.NewLine, threadEx ) ) ;
                Platform.Crash ( "Fatal thread access exception was thrown." ) ;
            }
            catch ( Exception ex )
            {
                _log.Failure
                    .Write ( Formatted ( "Could not freeze and launch the common platform from the bluetooth runtime.{NewLine}{ex}",
                                         Environment.NewLine, ex ) ) ;
                Platform.Crash ( "Unknown failure occurred in an EntryPointAttribute for the bluetooth runtime." ) ;
            }
        }

        private static ILogger VerboseLogger ()
        {
            var config = new LoggerConfiguration ()
                        .MinimumLevel.Verbose ()
                        .Enrich.FromLogContext ()
                        .Enrich.WithThreadId ()
                        .WriteTo.Console ( outputTemplate: Template, theme: ConsoleExtensions.BlueConsole ) ;
            return config.CreateLogger () ;
        }

        public static Assembly GetRuntimeAssembly () => Assembly ;
        public static string GetRuntimeUri () => ApplicationUri ;
    }
}
