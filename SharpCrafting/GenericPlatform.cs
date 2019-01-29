using System;
using System.Collections.Generic;
using System.Diagnostics ;
using System.Linq ;
using System.Text;

using System.Reflection;
using System.Runtime.CompilerServices ;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyModel;

using PostSharp.Patterns.Contracts;
using PostSharp.Patterns.Model;
using PostSharp.Patterns.Threading;

using Serilog;

namespace SharpCrafting
{
    [Freezable]
    public class GenericPlatform
    {
        [Reference] public GenericHost Host = new GenericHost();

        [Pure]
        private Assembly _caller() => Assembly.GetCallingAssembly();

        [Pure]
        private bool InvalidNamespace([Required] Assembly assembly, [Required] string @namespace) => 
            assembly.GetTypes().All(type => type.Namespace != @namespace) ;

        [Pure]
        public INativeClass GetNativeClass([Required] string @namespace, [Required] string className) => 
            this.Create(_caller(), @namespace, className) ;

        [Pure]
        private INativeClass Create([Required] Assembly assembly, [Required] string @namespace, [Required] string className)
        {
            if (this.InvalidNamespace(assembly, @namespace))
                this.Crash($"[Generic Platform]: Namespace not be located: {@namespace} in assembly: {assembly.FullName}");

            // Platform looks like Win32NT for Windows.
            string platformNamespace = $"{@namespace}.{Environment.OSVersion.Platform.ToString()}";

            // Check for any implementations of this platform.
            if (this.InvalidNamespace(assembly, platformNamespace))
                this.Crash($"[Generic Platform]: {Environment.OSVersion.Platform.ToString()} was not found at {@namespace} in assembly: {assembly.FullName}");

            // Native implementations currently require usage of the same class name but can exist under multiple platform namespaces.
            try
            {
                var native = assembly.GetType($"{platformNamespace}.{className}");
                var instance = (INativeClass)Activator.CreateInstance(native);
                return instance;
            }
            catch (Exception ex)
            {
                this.Crash($"Could not create an instance of a native platform class called {className} --- {ex.Message}");
                return null;
            }
        }

        [Pure, ContractAnnotation("=> halt")]
        public void Crash(string reason = "Unexpected behavior.")
        {
            Log.Fatal(reason);

            this.DebugHook();

            // ReSharper disable once InconsistentNaming - 128 (0x80) indicates no need to wait for child processes
            const int ERROR_WAIT_NO_CHILDREN = 128;
            Environment.Exit(ERROR_WAIT_NO_CHILDREN);
        }

        [Pure, Conditional("DEBUG")]
        private void DebugHook()
        {
            if (Debugger.IsAttached) Debugger.Break();
        }
    }
}
