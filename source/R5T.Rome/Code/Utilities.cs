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
using R5T.Caledonia;
using R5T.Caledonia.Default;
using R5T.Costobocia;
using R5T.Costobocia.Default;
using R5T.Dacia.Extensions;
using R5T.Frisia;
using R5T.Frisia.Suebia;
using R5T.Gepidia.Local;
using R5T.Gepidia.Remote;
using R5T.Jutland;
using R5T.Jutland.Newtonsoft;
using R5T.Lombardy;
using R5T.Macommania;
using R5T.Macommania.Default;
using R5T.Norsica;
using R5T.Norsica.Default;
using R5T.Pictia;
using R5T.Pictia.Frisia;
using R5T.Pompeii;
using R5T.Pompeii.Standard;
using R5T.Suebia;
using R5T.Suebia.Alamania;
using R5T.Suebia.Default;
using R5T.Teutonia;
using R5T.Teutonia.Default.Extensions;
using R5T.Virconium;
using R5T.Virconium.Default;
using R5T.Visigothia;
using R5T.Visigothia.Default.Local;


namespace R5T.Rome
{
    public static class Utilities
    {
        public static void RemoveExtraneousDependencies()
        {
            var serviceAndSolutionFilePath = Utilities.GetVirconiumFunctionality();

            serviceAndSolutionFilePath.Item1.RemoveExtraneousDependencies(serviceAndSolutionFilePath.Item2, Console.Out, false);
        }

        public static void AddMissingDependencies()
        {
            var serviceAndSolutionFilePath = Utilities.GetVirconiumFunctionality();

            serviceAndSolutionFilePath.Item1.AddMissingProjectDependencies(serviceAndSolutionFilePath.Item2, Console.Out, false);
        }

        private static Tuple<IVirconiumService, string> GetVirconiumFunctionality()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IVirconiumService, DefaultVirconiumService>()

                .AddSingleton<ISolutionFilePathProvider, StandardSolutionFilePathProvider>()
                .AddSingleton<ISolutionFileNameProvider, SingleSolutionFileNameProvider>()
                .AddSingleton<IExecutableFileDirectoryPathProvider, DefaultExecutableFileDirectoryPathProvider>()
                .AddSingleton<IExecutableFilePathProvider, DefaultExecutableFilePathProvider>()

                .AddSingleton<IStringlyTypedPathOperator, StringlyTypedPathOperator>()

                .BuildServiceProvider()
                ;

            var solutionFilePathProvider = serviceProvider.GetRequiredService<ISolutionFilePathProvider>();

            var solutionFilePath = solutionFilePathProvider.GetSolutionFilePath();

            var virconiumService = serviceProvider.GetRequiredService<IVirconiumService>();

            var output = Tuple.Create(virconiumService, solutionFilePath);
            return output;
        }

        public static void DeployRemoteWebsite<TSolutionFileNameProvider>(string remoteDeploymentSecretsFileName, string entryPointProjectName, TSolutionFileNameProvider solutionFileNameProvider = null)
            where TSolutionFileNameProvider: class, ISolutionFileNameProvider
        {
            // Build the DI container.
            var serviceProvider = new ServiceCollection()
                // Publishing.
                .AddSingleton<IPublishOperation, DotnetPublishOperation>()
                .AddSingleton<IDotnetCommandLineOperator, DotnetCommandLineOperator>()
                .AddSingleton<IDotnetCommandLineOperatorCore, DefaultDotnetCommandLineOperatorCore>()
                .AddSingleton<ICommandLineInvocationOperator, DefaultCommandLineInvocationOperator>()

                // Deployment source file-system site.
                .AddSingleton<IDeploymentSourceFileSystemSiteProvider, DefaultDeploymentSourceFileSystemSiteProvider>()
                .AddSingleton<IExecutableFilePathProvider, DefaultExecutableFilePathProvider>()
                .AddSingleton<IExecutableFileDirectoryPathProvider, DefaultExecutableFileDirectoryPathProvider>()
                .AddSingleton<ISolutionFilePathProvider, StandardSolutionFilePathProvider>()
                .AddSingletonAsTypeIfInstanceNull<ISolutionFileNameProvider, TSolutionFileNameProvider>(solutionFileNameProvider)
                .AddSingleton<IProjectBuildOutputBinariesDirectoryPathProvider, PublishDirectoryProjectBuildOutputBinariesDirectoryPathProvider>() // See possible combination with ***.
                .UseStandardEntryPointProjectConventions(entryPointProjectName, ReleaseBuildConfiguration.BuildConfigurationName)
                .AddSingleton<IEntryPointProjectBinariesDirectoryPathProvider, PublishDirectoryEntryPointProjectBinariesDirectoryPathProvider>() // Use the publish directory for websites. ***

                // Deployment destination file-system site.
                .AddSingleton<IDeploymentDestinationFileSystemSiteProvider, SecretsFileRemoteDeploymentDestinationFileSystemSiteProvider>()
                .AddSingleton<RemoteDeploymentSecretsSerialization>(serviceProviderInstance =>
                {
                    var serializationProvider = serviceProviderInstance.GetRequiredService<IRemoteDeploymentSecretsSerializationProvider>();

                    var serialization = serializationProvider.GetRemoteDeploymentSecretsSerialization();
                    return serialization;
                })
                .AddSingleton<IRemoteDeploymentSecretsSerializationProvider, DefaultRemoteDeployementSecretsSerializationProvider>()
                .AddSingleton<IDeploymentDestinationSecretsFileNameProvider>(new DirectDeploymentDestinationSecretsFileNameProvider(remoteDeploymentSecretsFileName))
                .AddSingleton<ISecretsFilePathProvider, DefaultSecretsFilePathProvider>()
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

                // Utilities.
                .AddSingleton<IJsonFileSerializationOperator, NewtonsoftJsonFileSerializationOperator>()
                .AddSingleton<IStringlyTypedPathOperator, StringlyTypedPathOperator>()
                .AddSingleton<LocalFileSystemOperator>() // For source.
                .AddSingleton<RemoteFileSystemOperator>() // For destination.

                .BuildServiceProvider()
                ;

            // Publish.
            var publishOperation = serviceProvider.GetRequiredService<IPublishOperation>();

            publishOperation.Execute();

            Utilities.Deploy(serviceProvider);
        }

        /// <summary>
        /// Can use if only a single solution file exists in the solution directory.
        /// </summary>
        public static void DeployRemoteWebsite(string remoteDeploymentSecretsFileName, string entryPointProjectName)
        {
            Utilities.DeployRemoteWebsite<SingleSolutionFileNameProvider>(remoteDeploymentSecretsFileName, entryPointProjectName);
        }

        public static void DeployRemoteWebsite(string remoteDeploymentSecretsFileName, string entryPointProjectName, string solutionFileName)
        {
            Utilities.DeployRemoteWebsite(remoteDeploymentSecretsFileName, entryPointProjectName, new DirectSolutionFileNameProvider(solutionFileName));
        }

        public static void DeployRemote(string remoteDeploymentSecretsFileName, string entryPointProjectName)
        {
            // Build the DI container.
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IDeploymentSourceFileSystemSiteProvider, DefaultDeploymentSourceFileSystemSiteProvider>()
                .AddSingleton<IExecutableFilePathProvider, DefaultExecutableFilePathProvider>()
                .AddSingleton<IExecutableFileDirectoryPathProvider, DefaultExecutableFileDirectoryPathProvider>()
                .AddSingleton<ISolutionFilePathProvider, StandardSolutionFilePathProvider>()
                .AddSingleton<ISolutionFileNameProvider, SingleSolutionFileNameProvider>()
                .AddSingleton<IProjectBuildOutputBinariesDirectoryPathProvider, StandardProjectBinariesOutputDirectoryPathProvider>()
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
                .AddSingleton<ISecretsFilePathProvider, DefaultSecretsFilePathProvider>()
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
                .AddSingleton<ISolutionFilePathProvider, StandardSolutionFilePathProvider>()
                .AddSingleton<ISolutionFileNameProvider, SingleSolutionFileNameProvider>()
                .AddSingleton<IProjectBuildOutputBinariesDirectoryPathProvider, StandardProjectBinariesOutputDirectoryPathProvider>()
                .AddSingleton<IEntryPointProjectNameProvider>(new DirectEntryPointProjectNameProvider(entryPointProjectName))
                .AddSingleton<IVisualStudioStringlyTypedPathPartsOperator, DefaultVisualStudioStringlyTypedPathPartsOperator>()

                .AddSingleton<IDeploymentDestinationFileSystemSiteProvider, SecretsFileLocalDeploymentDestinationFileSystemSiteProvider>()
                .AddSingleton<IDeploymentDestinationSecretsFileNameProvider>(new DirectDeploymentDestinationSecretsFileNameProvider(localDeploymentSecretsFileName))
                .AddSingleton<IUserProfileDirectoryPathProvider, DefaultLocalUserProfileDirectoryPathProvider>()
                .AddSingleton<IDropboxDirectoryPathProvider, DefaultLocalDropboxDirectoryPathProvider>()
                .AddSingleton<IRivetOrganizationDirectoryPathProvider, BulgariaRivetOrganizationDirectoryPathProvider>()
                .AddSingleton<ISecretsDirectoryPathProvider, AlamaniaSecretsDirectoryPathProvider>()
                .AddSingleton<ISecretsFilePathProvider, DefaultSecretsFilePathProvider>()
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
