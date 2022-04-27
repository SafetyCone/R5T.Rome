using System;

using R5T.Pictia;
using R5T.Pictia.Extensions;using R5T.T0064;


namespace R5T.Rome
{[ServiceImplementationMarker]
    public abstract class RunRemoteScriptsActionBase:IServiceImplementation
    {
        private ISftpClientWrapperProvider SftpClientWrapperProvider { get; }


        public RunRemoteScriptsActionBase(ISftpClientWrapperProvider sftpClientWrapperProvider)
        {
            this.SftpClientWrapperProvider = sftpClientWrapperProvider;
        }

        public void Run()
        {
            using (var sftpClientWrapper = this.SftpClientWrapperProvider.GetSftpClientWrapper())
            using (var sshClientWrapper = sftpClientWrapper.GetSshClientWrapper())
            {
                this.Run(sshClientWrapper);
            }
        }

        protected abstract void Run(SshClientWrapper sshClientWrapper);
    }
}
