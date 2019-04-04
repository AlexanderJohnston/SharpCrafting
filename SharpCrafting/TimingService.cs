using System.Threading ;
using System.Threading.Tasks ;

using Microsoft.Extensions.Hosting ;
using Microsoft.Extensions.Options ;

using PostSharp.Patterns.Diagnostics ;
using PostSharp.Patterns.Model ;
using PostSharp.Patterns.Threading ;

using Serilog ;

using static PostSharp.Patterns.Diagnostics.FormattedMessageBuilder ;
using static PostSharp.Patterns.Diagnostics.SemanticMessageBuilder ;

namespace SharpCrafting
{
    [ Actor ]
    public class TimingService : IHostedService
    {
        [ Reference ] private GenericPlatform _platform ;

        [ Reference ] private readonly LogSource _log = LogSource
                                                       .Get ()
                                                       .WithLevels ( PostSharp.Patterns.Diagnostics.LogLevel.Trace,
                                                                     PostSharp.Patterns.Diagnostics.LogLevel.Warning ) ;

        [ Child ]
        private INativeClass _nativeTimers { get ; set ; }

        public TimingService ( IOptions <AppConfig> options )
        {
            _platform = options.Value.Platform ;
        }

        [ Reentrant ]
        public async Task StartAsync ( CancellationToken cancellationToken )
        {
            _log.Info.Write ( Formatted ( "[Timing Service]: Calling out to the platform for native timers." ) ) ;
            _nativeTimers = ( INativeClass ) _platform.GetNativeClass ( "SharpCrafting", "NativeTimers" ) ;
            await _nativeTimers.Initialize ( this ) ;
        }

        [ Reentrant ]
        public async Task StopAsync ( CancellationToken cancellationToken )
        {
            _log.Warning.Write ( Formatted ( "[Timing Service]: Terminating this service." ) ) ;
            await _nativeTimers.Terminate ( "The timing service is being shut down by the host." ) ;
        }
    }
}
