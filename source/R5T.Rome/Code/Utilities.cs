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
using R5T.Frisia;
using R5T.Frisia.Suebia;
using R5T.Gepidia.Local;
using R5T.Gepidia.Remote;
using R5T.Jutland;
using R5T.Jutland.Newtonsoft;
using R5T.Lombardy;
using R5T.Macommania;
using R5T.Macommania.Default;
using R5T.Pictia;
using R5T.Pictia.Frisia;
using R5T.Pompeii;
using R5T.Pompeii.Standard;
using R5T.Suebia;
using R5T.Suebia.Alamania;
using R5T.Teutonia;
using R5T.Teutonia.Default.Extensions;
using R5T.Visigothia;
using R5T.Visigothia.Default.Local;


namespace R5T.Rome
{
    public static class Utilities
    {
        public static void DeployRemote(string remoteDeploymentSecretsFileName, string entryPointProjectName)
        {
            // Build the DI container.
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IDeploymentSourceFileSystemSiteProvider, DefaultDeploymentSourceFileSystemSiteProvider>()
                .AddSingleton<IExecutableFilePathProvider, DefaultExecutableFilePathProvider>()
                .AddSingleton<IExecutableFileDirectoryPathProvider, DefaultExecutableFileDirectoryPathProvider>()
                .AddSingleton<ISolutionFilePathProvider, StandardDevelopmentSolutionFilePathProvider>()
                .AddSingleton<ISolutionFileNameProvider, SingleSolutionFileNameProvider>()
                .AddSingleton<IProjectOutputBinariesDirectoryPathProvider, StandardProjectBinariesOutputDirectoryPathProvider>()
                .AddSingleton<IEntryPointProjectNameProvider>(new DirectEntryPointProjectNameProvider(entryPointProjectName))
                .AddSingleton<IVisualStudioStringlyTypedPathPartsOperator, DefaultVisualStudioStringlyTypedPathPartsOperator>()

                .AddSingleton<IDeploymentDestinationFileSystemSiteProvider, SecretsFileRemoteDeploymentDestinationFileSystemSiteProvider>()
                .AddSingleton<RemoteDeploymentSecretsSerialization>(serviceProviderInstance =>
                {
                    var serializationProvider = serviceProviderInstance.GetRequiredService<IRemoteDeploymentSecretsSerializationProvider>();

                    var serialization = serializationProvider.GetRemoteDeploymentSecretsSerialization();
                    return serialization;
                })
                .AddSingleton<IRemoteDeploymentSecretsSerializationProvider, DefaultRemoteDeployementSecretsSerializationProvider>()
                .AddSingleton<IDeploymentDestinationSecretsFileNameProvider>(new DirectDeploymentDestinationSecretsFileNameProvider(remoteDeploymentSecretsFileName))
                .AddSingleton<ISecretsFilePathProvider, AlamaniaSecretsFilePathProvider>()
                .AddSingleton<ISecretsDirectoryPathProvider, AlamaniaSecretsDirectoryPathProvider>()
                .AddSingleton<IRivetOrganizationDirectoryPathProvider, BulgariaRivetOrganizationDirectoryPathProvider>()
                .AddSingleton<IOrganizationDirectoryNameProvider, DefaultOrganizationDirectoryNameProvider>()
                .AddSingleton<IOrganizationStringlyTypedPathOperator, DefaultOrganizationStringlyTypedPathOperator>()
                .AddSingleton<IOrganizationsStringlyTypedPathOperator, DefaultOrganizationsStringlyTypedPathOperator>()
                .AddSingleton<IDropboxDirectoryPathProvider, DefaultLocalDropboxDirectoryPathProvider>()
                .AddSingleton<IUserProfileDirectoryPathProvider, DefaultLocalUserProfileDirectoryPathProvider>()
                .AddSingleton<RemoteFileSystemOperator>()
                .AddTransient<SftpClientWrapper>(serviceProviderInstance =>
                {
                    var sftpClientWrapperProvider = serviceProviderInstance.GetRequiredService<ISftpClientWrapperProvider>();

                    var sftpClientWrapper = sftpClientWrapperProvider.GetSftpClientWrapper();
                    return sftpClientWrapper;
                })
                .AddSingleton<ISftpClientWrapperProvider, FrisiaSftpClientWrapperProvider>()
                .AddSingleton<IAwsEc2ServerSecretsProvider, SuebiaAwsEc2ServerSecretsProvider>()
                .AddSingleton<IAwsEc2ServerSecretsFileNameProvider, RemoteDeploymentSerializationAwsEc2ServerSecretsFileNameProvider>()
                .AddSingleton<IAwsEc2ServerHostFriendlyNameProvider, RemoteDeploymentSerializationAwsEc2ServerHostFriendlyNameProvider>()

                .UseDefaultFileSystemCloningOperator()

                .AddSingleton<IJsonFileSerializationOperator, NewtonsoftJsonFileSerializationOperator>()
                .AddSingleton<IStringlyTypedPathOperator, StringlyTypedPathOperator>()
                .AddSingleton<LocalFileSystemOperator>() // For source.
                .AddSingleton<RemoteFileSystemOperator>() // For destination.

                .BuildServiceProvider()
                ;

            Utilities.Deploy(serviceProvider);
        }

        public static void DeployLocal(string localDeploymentSecretsFileName, string entryPointProjectName)
        {
            // Build the DI container.
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IDeploymentSourceFileSystemSiteProvider, DefaultDeploymentSourceFileSystemSiteProvider>()
                .AddSingleton<IExecutableFilePathProvider, DefaultExecutableFilePathProvider>()
                .AddSingleton<IExecutableFileDirectoryPathProvider, DefaultExecutableFileDirectoryPathProvider>()
                .AddSingleton<ISolutionFilePathProvider, StandardDevelopmentSolutionFilePathProvider>()
                .AddSingleton<ISolutionFileNameProvider, SingleSolutionFileNameProvider>()
                .AddSingleton<IProjectOutputBinariesDirectoryPathProvider, StandardProjectBinariesOutputDirectoryPathProvider>()
                .AddSingleton<IEntryPointProjectNameProvider>(new DirectEntryPointProjectNameProvider(entryPointProjectName))
                .AddSingleton<IVisualStudioStringlyTypedPathPartsOperator, DefaultVisualStudioStringlyTypedPathPartsOperator>()

                .AddSingleton<IDeploymentDestinationFileSystemSiteProvider, SecretsFileLocalDeploymentDestinationFileSystemSiteProvider>()
                .AddSingleton<IDeploymentDestinationSecretsFileNameProvider>(new DirectDeploymentDestinationSecretsFileNameProvider(localDeploymentSecretsFileName))
                .AddSingleton<IUserProfileDirectoryPathProvider, DefaultLocalUserProfileDirectoryPathProvider>()
                .AddSingleton<IDropboxDirectoryPathProvider, DefaultLocalDropboxDirectoryPathProvider>()
                .AddSingleton<IRivetOrganizationDirectoryPathProvider, BulgariaRivetOrganizationDirectoryPathProvider>()
                .AddSingleton<ISecretsDirectoryPathProvider, AlamaniaSecretsDirectoryPathProvider>()
                .AddSingleton<ISecretsFilePathProvider, AlamaniaSecretsFilePathProvider>()
                .AddSingleton<IOrganizationDirectoryNameProvider, DefaultOrganizationDirectoryNameProvider>()
                .AddSingleton<IOrganizationStringlyTypedPathOperator, DefaultOrganizationStringlyTypedPathOperator>()
                .AddSingleton<IOrganizationsStringlyTypedPathOperator, DefaultOrganizationsStringlyTypedPathOperator>()

                .UseDefaultFileSystemCloningOperator()

                .AddSingleton<IJsonFileSerializationOperator, NewtonsoftJsonFileSerializationOperator>()
                .AddSingleton<IStringlyTypedPathOperator, StringlyTypedPathOperator>()
                .AddSingleton<LocalFileSystemOperator>() // For source AND destination.

                .BuildServiceProvider()
                ;

            Utilities.Deploy(serviceProvider);
        }

        private static void Deploy(IServiceProvider serviceProvider)
        {
            var sourceSiteProvider = serviceProvider.GetRequiredService<IDeploymentSourceFileSystemSiteProvider>();
            var sourceSite = sourceSiteProvider.GetDeploymentSourceFileSystemSite();

            var destinationSiteProvider = serviceProvider.GetRequiredService<IDeploymentDestinationFileSystemSiteProvider>();
            var destinationSite = destinationSiteProvider.GetDeploymentDestinationFileSystemSite();

            var cloningOperator = serviceProvider.GetRequiredService<IFileSystemCloningOperator>();

            cloningOperator.Clone(sourceSite, destinationSite, FileSystemCloningOptions.Default);
        }
    }
}
