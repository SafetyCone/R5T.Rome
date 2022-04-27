using System;

using R5T.T0064;


namespace R5T.Rome
{
    [ServiceDefinitionMarker]
    public interface IRemoteServiceNameProvider : IServiceDefinition
    {
        string GetRemoteServiceName();
    }
}
