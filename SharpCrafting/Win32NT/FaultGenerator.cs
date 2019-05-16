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
using SharpCrafting.Win32NT.Utilities;
using static PostSharp.Patterns.Diagnostics.FormattedMessageBuilder ;
using static PostSharp.Patterns.Diagnostics.SemanticMessageBuilder ;

namespace SharpCrafting.Win32NT
{
    [PrivateThreadAware]
    class FaultGenerator : INativeClass
    {
        [ Reference ]
        private readonly LogSource _log = LogSource.Get().WithLevels(LogLevel.Trace, LogLevel.Warning);

        private List<Timer> _generators { get; set; } = new List<Timer>();

        [ Parent, CanBeNull ]
        private TimingService _parent { get ; set ; }

        public async Task Initialize <T> ( T parent )
        {
            Type parentType = typeof( T ) ;
            if ( parentType == typeof( TimingService ) )
                _parent = ( TimingService ) ( object ) parent ;

            _generators = new List <Timer> () ;
            _generators.AddRange ( CreateGenerators ( 1 ) ) ;
        }

        private IEnumerable <Timer> CreateGenerators ( int number )
        {
            for ( var i = 0 ; i < number ; i ++ )
                yield return StartTimedGenerator ( TimeSpan.FromMilliseconds (3000), TimeSpan.FromMilliseconds(10000), "random" ) ;
        }

        private Timer StartTimedGenerator(TimeSpan delay, TimeSpan interval, string type)
        {
            return new Timer(
                                    e => CauseException(type),
                                    null,
                                    TimeSpan.FromSeconds(delay.TotalSeconds),
                                    TimeSpan.FromSeconds(interval.TotalSeconds));
        }

        [ SingleEntryMethod ]
        public async Task Terminate ( string reason )
        {
            foreach (var generator in _generators)
            {
                generator.Dispose();
                _log.Debug.Write(Formatted("[Fault Generator]: Disposed of the fault generator after receiving reason: {reason}",
                                           reason));
            }
        }


        [ EntryPoint ]
        [ ServiceExceptionDetour ]
        private async Task CauseException ( string name )
        {
            _log.Debug.Write ( Formatted ("[Fault Generator] : The current time is {UtcNow} on the [ {name} ] exception generator which is now firing.",
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
                    await ExceptionThrower.ThrowUnsafeExceptionAsync();
                    break ;
                case "legacy":
                    await ExceptionThrower.ThrowsRandomSystemExceptionAsync();
                    break;
            }
        }
    }
}
