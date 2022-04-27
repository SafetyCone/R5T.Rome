using System;
using System.Collections.Generic;

using R5T.T0064;


namespace R5T.Rome
{
    [ServiceDefinitionMarker]
    public interface ISecretsFileNamesProvider : IServiceDefinition
    {
        IEnumerable<string> GetSecretsFileNames();
    }
}
