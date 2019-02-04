using System ;
using System.Collections.Generic ;
using System.Text ;
using System.Threading.Tasks ;

using JetBrains.Annotations ;

using Microsoft.Extensions.Configuration ;
using Microsoft.Extensions.DependencyInjection ;
using Microsoft.Extensions.Hosting ;

using PostSharp.Patterns.Threading ;

using SharpCrafting.Aspects;

using Serilog ;
using Serilog.Events ;

namespace SharpCrafting
{
    [PrivateThreadAware]
    public class GenericHost
    {
        [SingleEntryMethod]
        public async Task Start ( GenericPlatform platform, [ CanBeNull ] string[] args = null )
        {
            var builder = new HostBuilder ()
                         .ConfigureAppConfiguration ( configureDelegate: ( context, config ) =>
                                                      {
                                                          config.AddJsonFile ( "appsettings.json", optional: true ) ;
                                                          config.AddEnvironmentVariables () ;

                                                          if ( args != null )
                                                              config.AddCommandLine ( args ) ;
                                                      } )
                         .ConfigureServices ( configureDelegate: ( context, services ) =>
                                                                 {
                                                                     services.AddOptions () ;
                                                                     services.Configure <AppConfig> ( context
                                                                                                     .Configuration
                                                                                                     .GetSection ( "AppConfig" ) ) ;
                                                                     services.AddHostedService <TimingService> () ;
                                                                     services.Configure <AppConfig> ( options =>
                                                                                                      {
                                                                                                          options.Platform = platform ;
                                                                                                      } ) ;
                                                                 } ) ;
            await builder.RunConsoleAsync () ;
        }
    }
}
