using System ;
using System.Collections.Generic ;
using System.Text ;
using System.Threading ;
using System.Threading.Tasks ;
using HouseofCat.Library.Gremlins;
using JetBrains.Annotations ;

using PostSharp.Patterns.Diagnostics ;
using PostSharp.Patterns.Model ;
using PostSharp.Patterns.Threading ;

using Serilog ;
using SharpCrafting.Aspects;
using static PostSharp.Patterns.Diagnostics.FormattedMessageBuilder ;
using static PostSharp.Patterns.Diagnostics.SemanticMessageBuilder ;

namespace SharpCrafting.Win32NT
{
    [PrivateThreadAware]
    class FaultGenerator : INativeClass
    {
        [Reference]
        private readonly LogSource _log = LogSource.Get().WithLevels(LogLevel.Trace, LogLevel.Warning);

        private List<Timer> _generators { get; set; } = new List<Timer>();

        [ Parent, CanBeNull ]
        private TimingService _parent { get ; set ; }

        public async Task Initialize <T> ( T parent )
        {
            Type parentType = typeof( T ) ;
            if ( parentType == typeof( TimingService ) )
                _parent = ( TimingService ) ( object ) parent ;

            _generators = new List<Timer>() {
                StartTimedGenerator(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3), "system"),
                StartTimedGenerator(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), "network"),
                StartTimedGenerator(TimeSpan.FromSeconds(8), TimeSpan.FromSeconds(8), "sql"),
                StartTimedGenerator(TimeSpan.FromSeconds(13), TimeSpan.FromSeconds(13), "random"),
            };
        }

        private Timer StartTimedGenerator(TimeSpan delay, TimeSpan interval, string type)
        {
            return new Timer(
                                    e => CauseException(type),
                                    null,
                                    TimeSpan.FromSeconds(delay.TotalSeconds),
                                    TimeSpan.FromSeconds(interval.TotalSeconds));
        }

        [SingleEntryMethod]
        public async Task Terminate ( string reason )
        {
            foreach (var generator in _generators)
            {
                generator.Dispose();
                _log.Trace.Write(Formatted("[Fault Generator]: Disposed of the fault generator after receiving reason: {reason}",
                                           reason));
            }
        }


        [ EntryPoint ]
        [ ServiceExceptionDetour ]
        private async Task CauseException ( string name )
        {
            _log.Trace.Write ( Formatted ("[Fault Generator] : The current time is {UtcNow} on the [ {name} ] exception generator which is now firing.",
                                           DateTime.UtcNow, name ) ) ;
            switch (name)
            {
                case "network":
                    await ExceptionGremlin.ThrowsNetworkExceptionAsync();
                    break;
                case "sql":
                    await ExceptionGremlin.ThrowsSqlExceptionAsync();
                    break;
                case "system":
                    await ExceptionGremlin.ThrowsSystemExceptionAsync();
                    break;
                case "random":
                    await ExceptionGremlin.ThrowsRandomExceptionAsync();
                    break;
            }
        }
    }
}
