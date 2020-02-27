using System;


namespace R5T.Rome
{
    public class DirectRemoteServiceNameProvider : IRemoteServiceNameProvider
    {
        private string RemoteServiceName { get; }


        public DirectRemoteServiceNameProvider(string remoteServiceName)
        {
            this.RemoteServiceName = remoteServiceName;
        }

        public string GetRemoteServiceName()
        {
            return this.RemoteServiceName;
        }
    }
}
