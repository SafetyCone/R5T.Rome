using System;

using R5T.Pictia;
using R5T.Pictia.Extensions;


namespace R5T.Rome
{
    public abstract class RunRemoteScriptsActionBase : IPostFileCopyDeployAction
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
