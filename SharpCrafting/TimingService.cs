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
                                                       .WithLevels ( PostSharp.Patterns.Diagnostics.LogLevel.Debug,
                                                                     PostSharp.Patterns.Diagnostics.LogLevel.Warning ) ;

        [ Child ]
        private INativeClass _faultGenerator { get ; set ; }

        public TimingService ( IOptions <AppConfig> options )
        {
            _platform = options.Value.Platform ;
        }

        [ Reentrant ]
        public async Task StartAsync ( CancellationToken cancellationToken )
        {
            _log.Info.Write ( Formatted ( "[Fault Generator]: Calling out to the platform for a native exception generator." ) ) ;
            _faultGenerator = ( INativeClass ) _platform.GetNativeClass ( "SharpCrafting", "FaultGenerator" ) ;
            await _faultGenerator.Initialize ( this ) ;
        }

        [ Reentrant ]
        public async Task StopAsync ( CancellationToken cancellationToken )
        {
            _log.Warning.Write ( Formatted ( "[Fault Generator]: Terminating this service." ) ) ;
            await _faultGenerator.Terminate ( "The fault generator service is being shut down by the host." ) ;
        }
    }
}
