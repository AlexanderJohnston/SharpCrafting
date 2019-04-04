using System ;
using System.Collections.Generic ;
using System.Text ;
using System.Threading ;
using System.Threading.Tasks ;

using JetBrains.Annotations ;

using PostSharp.Patterns.Diagnostics ;
using PostSharp.Patterns.Model ;
using PostSharp.Patterns.Threading ;

using Serilog ;

using static PostSharp.Patterns.Diagnostics.FormattedMessageBuilder ;
using static PostSharp.Patterns.Diagnostics.SemanticMessageBuilder ;

namespace SharpCrafting.Win32NT
{
    [ PrivateThreadAware ]
    class NativeTimers : INativeClass
    {
        [ Reference ]
        private readonly LogSource _log = LogSource.Get ().WithLevels ( LogLevel.Trace, LogLevel.Warning ) ;

        [ Reference ]
        private Timer _timerShort { get ; set ; }

        [ Reference ]
        private Timer _timerMedium { get ; set ; }

        [ Reference ]
        private Timer _timerLong { get ; set ; }

        [ Parent, CanBeNull ]
        private TimingService _parent { get ; set ; }

        public async Task Initialize <T> ( T parent )
        {
            Type parentType = typeof( T ) ;
            if ( parentType == typeof( TimingService ) )
                _parent = ( TimingService ) ( object ) parent ;

            _timerShort = new Timer (
                                     e => WriteTime ( "short" ),
                                     null,
                                     TimeSpan.FromSeconds ( 1 ),
                                     TimeSpan.FromSeconds ( 1 ) ) ;
            _timerMedium = new Timer (
                                      e => WriteTime ( "medium" ),
                                      null,
                                      TimeSpan.FromSeconds ( 1 ),
                                      TimeSpan.FromSeconds ( 3 ) ) ;
            _timerLong = new Timer (
                                    e => WriteTime ( "long" ),
                                    null,
                                    TimeSpan.FromSeconds ( 1 ),
                                    TimeSpan.FromSeconds ( 5 ) ) ;
        }

        public async Task Terminate ( string reason )
        {
            _timerShort.Dispose () ;
            _timerMedium.Dispose () ;
            _timerLong.Dispose () ;
            _log.Trace.Write ( Formatted ( "[Native Timers]: Disposed of the timers after receiving reason: {reason}",
                                           reason ) ) ;
        }

        [ EntryPoint ]
        private async Task WriteTime ( string name )
        {
            _log.Trace.Write ( Formatted ( "[Native Timers] : The current time is {UtcNow} on the [ {name} ] timer.",
                                           DateTime.UtcNow, name ) ) ;
        }
    }
}
