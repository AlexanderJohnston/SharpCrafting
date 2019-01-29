using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks ;

using JetBrains.Annotations ;

namespace SharpCrafting
{
    public interface INativeClass
    {
        Task Initialize <T> ([CanBeNull]T parent);

        Task Terminate(string reason);
    }
}
