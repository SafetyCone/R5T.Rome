using System;

using R5T.Pictia;


namespace R5T.Rome
{
    public class VerifyWebsiteStartedAction : RunRemoteScriptsActionBase
    {
        public VerifyWebsiteStartedAction(ISftpClientWrapperProvider sftpClientWrapperProvider)
            : base(sftpClientWrapperProvider)
        {
        }

        protected override void Run(SshClientWrapper sshClientWrapper)
        {
            // Search sudo systemctl status kestrel-anyoneone.service for success value, or unsuccess value.
            //sshClientWrapper.SshClient;
        }
    }
}
