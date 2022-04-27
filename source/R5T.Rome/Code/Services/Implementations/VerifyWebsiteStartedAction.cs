using System;
using System.IO;
using System.Threading;

using Microsoft.Extensions.Logging;

using R5T.Pictia;

using R5T.T0064;


namespace R5T.Rome
{
    [ServiceImplementationMarker]
    public class VerifyWebsiteStartedAction : RunRemoteScriptsActionBase, IPostFinalizeDeployAction, IServiceImplementation
    {
        private IRemoteServiceNameProvider RemoteServiceNameProvider { get; }
        private ILogger Logger { get; }


        public VerifyWebsiteStartedAction(
            ISftpClientWrapperProvider sftpClientWrapperProvider,
            IRemoteServiceNameProvider remoteServiceNameProvider,
            ILogger<VerifyWebsiteStartedAction> logger)
            : base(sftpClientWrapperProvider)
        {
            this.RemoteServiceNameProvider = remoteServiceNameProvider;
            this.Logger = logger;
        }

        protected override void Run(SshClientWrapper sshClientWrapper)
        {
            var remoteServiceName = this.RemoteServiceNameProvider.GetRemoteServiceName();

            // Wait initially.
            var numberOfSeconds = 5;

            this.Logger.LogInformation($"Pausing for {numberOfSeconds} seconds to allow website service to start...");

            Thread.Sleep(5 * 1000);

            // Now test whether service is running.
            this.Logger.LogInformation($"Now checking if {remoteServiceName} website service is active...");            

            var commandText = $"sudo systemctl status {remoteServiceName}";
            var command = sshClientWrapper.SshClient.RunCommand(commandText);
            var result = command.Result;

            using (var stringReader = new StringReader(result))
            {
                stringReader.ReadLine();
                stringReader.ReadLine();

                var activeLine = stringReader.ReadLine();

                var serviceIsActive = activeLine.Contains("active (running)");
                if(serviceIsActive)
                {
                    this.Logger.LogInformation($"Website service {remoteServiceName} is active.");
                }
                else
                {
                    var message = $"Website service is not active!";

                    this.Logger.LogError(message);

                    throw new Exception(message);
                }
            }
        }
    }
}
