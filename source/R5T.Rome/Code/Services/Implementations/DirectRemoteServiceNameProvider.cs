using System;

using R5T.T0064;


namespace R5T.Rome
{
    [ServiceImplementationMarker]
    public class DirectRemoteServiceNameProvider : IRemoteServiceNameProvider, IServiceImplementation
    {
        private string RemoteServiceName { get; }


        public DirectRemoteServiceNameProvider(
            [NotServiceComponent] string remoteServiceName)
        {
            this.RemoteServiceName = remoteServiceName;
        }

        public string GetRemoteServiceName()
        {
            return this.RemoteServiceName;
        }
    }
}
