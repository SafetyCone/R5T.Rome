using System;

using Microsoft.Extensions.DependencyInjection;

using R5T.Alamania;
using R5T.Alamania.Bulgaria;
using R5T.Angleterria;
using R5T.Angleterria.Default;
using R5T.Antium;
using R5T.Antium.Default;
using R5T.Bulgaria;
using R5T.Bulgaria.Default.Local;
using R5T.Costobocia;
using R5T.Costobocia.Default;
using R5T.Gepidia.Local;
using R5T.Jutland;
using R5T.Jutland.Newtonsoft;
using R5T.Lombardy;
using R5T.Macommania;
using R5T.Macommania.Default;
using R5T.Pompeii;
using R5T.Pompeii.Standard;
using R5T.Suebia;
using R5T.Suebia.Alamania;
using R5T.Teutonia;
using R5T.Teutonia.Default;

using R5T.Visigothia;
using R5T.Visigothia.Default.Local;


namespace R5T.Rome
{
    public static class Utilities
    {
        public static void DeployLocal(string localDeploymentSecretsFileName, string entryPointProjectName)
        {
            // Build the DI container.
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IDeploymentSourceFileSystemSiteProvider, DefaultDeploymentSourceFileSystemSiteProvider>()
                .AddSingleton<IProjectOutputBinariesDirectoryPathProvider, StandardProjectBinariesOutputDirectoryPathProvider>()
                .AddSingleton<ISolutionFilePathProvider, StandardDevelopmentSolutionFilePathProvider>()
                .AddSingleton<IEntryPointProjectNameProvider>(new DirectEntryPointProjectNameProvider(entryPointProjectName))
                .AddSingleton<IVisualStudioStringlyTypedPathPartsOperator, DefaultVisualStudioStringlyTypedPathPartsOperator>()
                .AddSingleton<IExecutableFileDirectoryPathProvider, DefaultExecutableFileDirectoryPathProvider>()
                .AddSingleton<ISolutionFileNameProvider, SingleSolutionFileNameProvider>()
                .AddSingleton<IExecutableFilePathProvider, DefaultExecutableFilePathProvider>()

                .AddSingleton<IDeploymentDestinationFileSystemSiteProvider, SecretsFileLocalDeploymentDestinationFileSystemSiteProvider>()
                .AddSingleton<IDeploymentDestinationSecretsFileNameProvider>(new DirectDeploymentDestinationSecretsFileNameProvider(localDeploymentSecretsFileName))
                .AddSingleton<ISecretsFilePathProvider, AlamaniaSecretsFilePathProvider>()
                .AddSingleton<ISecretsDirectoryPathProvider, AlamaniaSecretsDirectoryPathProvider>()
                .AddSingleton<IRivetOrganizationDirectoryPathProvider, BulgariaRivetOrganizationDirectoryPathProvider>()
                .AddSingleton<IDropboxDirectoryPathProvider, DefaultLocalDropboxDirectoryPathProvider>()
                .AddSingleton<IUserProfileDirectoryPathProvider, DefaultLocalUserProfileDirectoryPathProvider>()
                .AddSingleton<IOrganizationStringlyTypedPathOperator, DefaultOrganizationStringlyTypedPathOperator>()
                .AddSingleton<IOrganizationsStringlyTypedPathOperator, DefaultOrganizationsStringlyTypedPathOperator>()
                .AddSingleton<IOrganizationDirectoryNameProvider, DefaultOrganizationDirectoryNameProvider>()

                .AddSingleton<IFileSystemCloningOperator, DefaultFileSystemCloningOperator>()
                .AddSingleton<IFileSystemCloningDifferencer, DefaultFileSystemCloningDifferencer>()

                .AddSingleton<IJsonFileSerializationOperator, NewtonsoftJsonFileSerializationOperator>()
                .AddSingleton<IStringlyTypedPathOperator, StringlyTypedPathOperator>()
                .AddSingleton<LocalFileSystemOperator>()

                .BuildServiceProvider()
                ;

            var sourceSiteProvider = serviceProvider.GetRequiredService<IDeploymentSourceFileSystemSiteProvider>();
            var sourceSite = sourceSiteProvider.GetDeploymentSourceFileSystemSite();

            var destinationSiteProvider = serviceProvider.GetRequiredService<IDeploymentDestinationFileSystemSiteProvider>();
            var destinationSite = destinationSiteProvider.GetDeploymentDestinationFileSystemSite();

            var cloningOperator = serviceProvider.GetRequiredService<IFileSystemCloningOperator>();

            cloningOperator.Clone(sourceSite, destinationSite, FileSystemCloningOptions.Default);
        }
    }
}
