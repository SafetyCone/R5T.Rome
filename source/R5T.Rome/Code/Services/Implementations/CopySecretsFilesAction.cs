using System;
using System.Collections.Generic;

using R5T.Antium;
using R5T.Lombardy;
using R5T.Teutonia;


namespace R5T.Rome
{
    public class CopySecretsFilesAction : IPostFileCopyDeployAction
    {
        private ISecretsFileNamesProvider SecretsFileNamesProvider { get; }
        private IDeploymentSource_SecretsDirectory_FileSystemSiteProvider DeploymentSource_SecretsDirectory_FileSystemSiteProvider { get; }
        private IDeploymentDestination_SecretsDirectory_FileSystemSiteProvider DeploymentDestination_SecretsDirectory_FileSystemSiteProvider { get; }
        private IStringlyTypedPathOperator StringlyTypedPathOperator { get; }


        public CopySecretsFilesAction(
            ISecretsFileNamesProvider secretsFileNamesProvider,
            IDeploymentSource_SecretsDirectory_FileSystemSiteProvider deploymentSource_SecretsDirectory_FileSystemSiteProvider,
            IDeploymentDestination_SecretsDirectory_FileSystemSiteProvider deploymentDestination_SecretsDirectory_FileSystemSiteProvider,
            IStringlyTypedPathOperator stringlyTypedPathOperator)
        {
            this.SecretsFileNamesProvider = secretsFileNamesProvider;
            this.DeploymentSource_SecretsDirectory_FileSystemSiteProvider = deploymentSource_SecretsDirectory_FileSystemSiteProvider;
            this.DeploymentDestination_SecretsDirectory_FileSystemSiteProvider = deploymentDestination_SecretsDirectory_FileSystemSiteProvider;
            this.StringlyTypedPathOperator = stringlyTypedPathOperator;
        }

        public void Run()
        {
            var operations = new List<IFileSystemCloningOperation>();

            var sourceDirectoryFileSystemSite = this.DeploymentSource_SecretsDirectory_FileSystemSiteProvider.GetDeploymentSourceSecretsDirectoryFileSystemSite();
            var destinationDirectoryFileSystemSite = this.DeploymentDestination_SecretsDirectory_FileSystemSiteProvider.GetDeploymentDestinationSecretsDirectoryFileSystemSite();

            var secretsFileNames = this.SecretsFileNamesProvider.GetSecretsFileNames();
            foreach (var secretsFileName in secretsFileNames)
            {
                var sourceFilePath = this.StringlyTypedPathOperator.Combine(sourceDirectoryFileSystemSite.DirectoryPath, secretsFileName);
                var destinationFilePath = this.StringlyTypedPathOperator.Combine(destinationDirectoryFileSystemSite.DirectoryPath, secretsFileName);

                var copyFileOperation = new CopyFileOperation(sourceFilePath, destinationFilePath);

                operations.Add(copyFileOperation);
            }

            foreach (var operation in operations)
            {
                operation.Execute(sourceDirectoryFileSystemSite.FileSystemOperator, destinationDirectoryFileSystemSite.FileSystemOperator);
            }
        }
    }
}
