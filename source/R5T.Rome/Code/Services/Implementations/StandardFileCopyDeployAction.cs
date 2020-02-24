using System;

using R5T.Antium;
using R5T.Teutonia;


namespace R5T.Rome
{
    public class StandardFileCopyDeployAction : IFileCopyDeployAction
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
