using System;

using R5T.Antium;
using R5T.Teutonia;

using R5T.T0064;


namespace R5T.Rome
{
    [ServiceImplementationMarker]
    public class StandardFileCopyDeployAction : IFileCopyDeployAction, IServiceImplementation
    {
        private IDeploymentSource_FileSystemSiteProvider DeploymentSource_FileSystemSiteProvider { get; }
        private IDeploymentDestination_FileSystemSiteProvider DeploymentDestination_FileSystemSiteProvider { get; }
        private IFileSystemCloningOperator FileSystemCloningOperator { get; }


        public StandardFileCopyDeployAction(
            IDeploymentSource_FileSystemSiteProvider deploymentSource_FileSystemSiteProvider,
            IDeploymentDestination_FileSystemSiteProvider deploymentDestination_FileSystemSiteProvider,
            IFileSystemCloningOperator fileSystemCloningOperator)
        {
            this.DeploymentSource_FileSystemSiteProvider = deploymentSource_FileSystemSiteProvider;
            this.DeploymentDestination_FileSystemSiteProvider = deploymentDestination_FileSystemSiteProvider;
            this.FileSystemCloningOperator = fileSystemCloningOperator;
        }

        public void Run()
        {
            var sourceSite = this.DeploymentSource_FileSystemSiteProvider.GetDeploymentSourceFileSystemSite();
            var destinationSite = this.DeploymentDestination_FileSystemSiteProvider.GetDeploymentDestinationFileSystemSite();

            this.FileSystemCloningOperator.Clone(sourceSite, destinationSite, FileSystemCloningOptions.Default);
        }
    }
}
