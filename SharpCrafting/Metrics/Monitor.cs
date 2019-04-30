using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.ApplicationInsights ;
using Microsoft.ApplicationInsights.Extensibility ;

namespace SharpCrafting.Metrics
{
    class Monitor
    {
        public TelemetryClient Telemetry { get ; set ; }

        public Monitor ()
        {
            TelemetryConfiguration config = TelemetryConfiguration.Active; // Reads ApplicationInsights.config file if present
            config.TelemetryChannel.DeveloperMode = true ;
            //config.InstrumentationKey = "" ;
            Telemetry = new TelemetryClient(config);
        }
    }
}
