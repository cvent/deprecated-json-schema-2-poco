using System;
using System.Collections.Generic;

namespace Cvent.JsonSchema2Poco
{
    public interface IGenerationConfig
    {
        HashSet<Uri> Sources { get; }
        string TargetDirectory { get; }
        string Namespace { get; }
        bool RemoveOldOutput { get; }
    }
}
