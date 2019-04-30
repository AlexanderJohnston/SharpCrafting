using System ;
using System.Collections.Generic ;
using System.Text ;

using PostSharp.Aspects ;
using PostSharp.Extensibility ;
using PostSharp.Patterns.Diagnostics ;
using PostSharp.Patterns.Diagnostics.Custom.Messages ;
using PostSharp.Patterns.Model ;
using PostSharp.Serialization ;

using Totem.IO ;
using Totem.Runtime.Metrics ;

using static PostSharp.Patterns.Diagnostics.FormattedMessageBuilder ;
using static PostSharp.Patterns.Diagnostics.SemanticMessageBuilder ;

namespace SharpCrafting.Aspects
{
    [ PSerializable ]
    [ MulticastAttributeUsage ( Inheritance = MulticastInheritance.Strict ) ]
    class ServiceExceptionDetourAttribute : OnExceptionAspect
    {
        private static LogSource _log = LogSource.Get ().WithLevels ( LogLevel.Trace, LogLevel.Warning ) ;

        public override void OnException ( MethodExecutionArgs margs )
        {
            var argCount  = margs.Arguments.Count ;
            var arguments = new StringBuilder () ;
            for ( var i = 0 ; i < argCount ; i ++ )
            {
                arguments.Append ( margs.Arguments.GetArgument ( i ) ) ;
                if ( argCount > 1 )
                    arguments.Append ( ',' ) ;
            }

            var targetName = LimitNamespace ( margs.Exception.TargetSite.ToString () ) ;

            _log.Error.Write ( Formatted ( "##[Exception Monitor]##: Swallowed an exception from {targetName} with: [{arguments}]{NewLine}{Message}",
                                           targetName,
                                           arguments,
                                           Environment.NewLine,
                                           margs.Exception.Message ) ) ;
        }

        public string LimitNamespace ( string typeDeclaration )
        {
            var words    = typeDeclaration.Split ( ' ' ) ;
            var site     = words[0] ;
            var target   = words[1] ;
            var nameLink = LinkPath.From ( site, new[] { "." } ) ;
            var length   = nameLink.Segments.Count ;
            var sb       = new StringBuilder () ;
            sb.Append ( nameLink.Segments[length - 2] + "." ) ;
            sb.Append ( nameLink.Segments[length - 1] + "." ) ;
            sb.Append ( target ) ;
            return sb.ToString () ;
        }
    }
}
