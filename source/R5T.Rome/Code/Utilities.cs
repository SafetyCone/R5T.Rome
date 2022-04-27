using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using R5T.Alamania;
using R5T.Alamania.Bulgaria;
using R5T.Angleterria;
using R5T.Angleterria.Default;
using R5T.Antium;
using R5T.Antium.Default;
using R5T.Antium.Standard;
using R5T.Bulgaria;
using R5T.Bulgaria.Default.Local;
using R5T.Caledonia;
using R5T.Caledonia.Default;
using R5T.Costobocia;
using R5T.Costobocia.Default;
using R5T.Dacia;
using R5T.Dacia.Extensions;
using R5T.Exeter;
using R5T.Frisia;
using R5T.Frisia.Suebia;
using R5T.Gepidia.Local;
using R5T.Gepidia.Remote;
using R5T.Jutland;
using R5T.Jutland.Newtonsoft;
using R5T.Lombardy;
using R5T.Lombardy.Standard;
using R5T.Macommania;
using R5T.Macommania.Default;
using R5T.Macommania.Standard;
using R5T.Magyar;
using R5T.Norsica.Standard;
using R5T.Pictia;
using R5T.Pictia.Frisia;
using R5T.Pompeii;
using R5T.Pompeii.Default;
using R5T.Pompeii.Standard;
using R5T.Suebia;
using R5T.Suebia.Alamania;
using R5T.Suebia.Default;
using R5T.Suebia.Standard;
using R5T.Teutonia;
using R5T.Teutonia.Standard;
using R5T.Virconium;
using R5T.Virconium.Default;
using R5T.Visigothia;
//using R5T.Visigothia.Default.Local;


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
            //var serviceProvider = new ServiceCollection()
            //    .AddSingleton<IVirconiumService, DefaultVirconiumService>()

            //    .AddSingleton<ISolutionFilePathProvider, StandardSolutionFilePathProvider>()
            //    .AddSingleton<ISolutionFileNameProvider, SingleSolutionFileNameProvider>()
            //    .AddSingleton<IExecutableFileDirectoryPathProvider, DefaultExecutableFileDirectoryPathProvider>()
            //    .AddSingleton<IExecutableFilePathProvider, DefaultExecutableFilePathProvider>()

            //    .AddSingleton<IStringlyTypedPathOperator, StringlyTypedPathOperator>()

            //    .BuildServiceProvider()
            //    ;

            //var solutionFilePathProvider = serviceProvider.GetRequiredService<ISolutionFilePathProvider>();

            //var solutionFilePath = solutionFilePathProvider.GetSolutionFilePath();

            //var virconiumService = serviceProvider.GetRequiredService<IVirconiumService>();

            //var output = Tuple.Create(virconiumService, solutionFilePath);
            //return output;

            throw new NotImplementedException();
        }

        public static void DeployRemoteWebsite(string remoteDeploymentSecretsFileName, string entryPointProjectName, string remoteServiceName,
            IServiceAction<ISolutionFileNameProvider> addSolutionFileNameProvider,
            IServiceAction<ISecretsFileNamesProvider> addSecretsFileNamesProvider,
            IServiceAction<IFinalizeDeployAction> addFinalizeDeployAction)
        {
            var buildConfigurationName = "Debug";

            // Build the DI-container for configuring the configuration.
            var services = new ServiceCollection()
                .AddSecretsDirectoryFilePathProvider()
                ;

            var configurationServiceProvider = services.BuildServiceProvider();

            // Build the configuration.
            var configurationBuilder = new ConfigurationBuilder()
                .AddDotnetConfiguration(configurationServiceProvider)
                ;

            var configuration = configurationBuilder.Build();

            // Build the DI-container.
            services
                .AddConfiguration(configuration)
                .AddBasicLogging()

                .AddStandardDeployAction(
                    EnumerableHelper.From(services.AddPublishPreFileCopyDeployActionAction(
                        services.AddPublishActionAction(
                            services.AddDirectEntryPointProjectNameProviderAction(entryPointProjectName),
                            services.AddDirectBuildConfigurationNameProviderAction(buildConfigurationName)))),
                    services.AddStandardFileCopyDeployActionAction(
                        services.AddPublishDeploymentSourceFileSystemSiteProviderAction(
                            services.AddDirectEntryPointProjectNameProviderAction(entryPointProjectName),
                            services.AddDirectBuildConfigurationNameProviderAction(buildConfigurationName)),
                        services.AddRemoteDeploymentDestinationFileSystemSiteProviderAction(
                            services.AddDirectDeploymentDestinationSecretsFileNameProviderAction_Old(remoteDeploymentSecretsFileName)),
                        services.AddFileSystemCloningOperatorAction()),
                    EnumerableHelper.From(services.AddCopySecretsFilesActionAction(
                        addSecretsFileNamesProvider,
                        services.AddDefaultDeploymentSource_SecretsDirectory_FileSystemSiteProviderAction_Old(
                            ServiceAction<ILocalFileSystemOperator>.AlreadyAdded,
                            ServiceAction<ISecretsDirectoryPathProvider>.AlreadyAdded),
                        services.AddDefaultRemoteDeploymentDestination_SecretsDirectory_FileSystemSiteProviderAction_Old(
                            ServiceAction<IRemoteFileSystemOperator>.AlreadyAdded,
                            ServiceAction<IRemoteDeploymentSecretsSerializationProvider>.AlreadyAdded),
                        services.AddStringlyTypedPathOperatorAction())),
                    addFinalizeDeployAction,
                    EnumerableHelper.From(services.AddVerifyWebsiteStartedActionAction(
                        ServiceAction<ISftpClientWrapperProvider>.AlreadyAdded,
                        services.AddDirectRemoteServiceNameProviderAction(remoteServiceName))))
                ;

            var serviceProvider = services.BuildServiceProvider();

            var deployAction = serviceProvider.GetRequiredService<IDeployAction>();

            deployAction.Run();
        }

        public static void DeployRemoteWebsite_Old(string remoteDeploymentSecretsFileName, string entryPointProjectName,
            ServiceAction<ISolutionFileNameProvider> addSolutionFileNameProvider)
        {
            var buildConfigurationName = "Debug";

            // Build the DI-container for configuring the configuration.
            var services = new ServiceCollection()
                .AddSecretsDirectoryFilePathProvider()
                ;

            var configurationServiceProvider = services.BuildServiceProvider();

            // Build the configuration.
            var configurationBuilder = new ConfigurationBuilder()
                .AddDotnetConfiguration(configurationServiceProvider)
                ;

            var configuration = configurationBuilder.Build();

            // Build the DI-container.
            services
                .AddConfiguration(configuration)

                // Publishing.
                .AddPublishAction(
                    services.AddDirectEntryPointProjectNameProviderAction(entryPointProjectName),
                    services.AddDirectBuildConfigurationNameProviderAction(buildConfigurationName))

                // Deployment source file-system site.
                .AddPublishDeploymentSourceFileSystemSiteProvider(
                    services.AddDirectEntryPointProjectNameProviderAction(entryPointProjectName),
                    services.AddDirectBuildConfigurationNameProviderAction(buildConfigurationName))

                // Deployment destination file-system site.
                .AddRemoteDeploymentDestinationFileSystemSiteProvider(
                    services.AddDirectDeploymentDestinationSecretsFileNameProviderAction_Old(remoteDeploymentSecretsFileName))

                .AddFileSystemCloningOperator()
                ;

            var serviceProvider = services.BuildServiceProvider();

            // Publish.
            var publishAction = serviceProvider.GetRequiredService<IPublishAction>();

            publishAction.Publish();

            // Deploy.
            Utilities.Deploy(serviceProvider);
        }

        /// <summary>
        /// Can use if only a single solution file exists in the solution directory.
        /// </summary>
        public static void DeployRemoteWebsite_Old(string remoteDeploymentSecretsFileName, string entryPointProjectName)
        {
            void addSolutionFileName(IServiceCollection services) => services.AddSingleSolutionFileNameProvider();

            Utilities.DeployRemoteWebsite_Old(remoteDeploymentSecretsFileName, entryPointProjectName,
               new ServiceAction<ISolutionFileNameProvider>(addSolutionFileName));
        }

        public static void DeployRemoteWebsite_Old(string remoteDeploymentSecretsFileName, string entryPointProjectName, string solutionFileName)
        {
            void addSolutionFileName(IServiceCollection services) => services.AddDirectSolutionFileNameProvider(solutionFileName);

            Utilities.DeployRemoteWebsite_Old(remoteDeploymentSecretsFileName, entryPointProjectName,
                new ServiceAction<ISolutionFileNameProvider>(addSolutionFileName));
        }

        public static void DeployRemoteWebsite(string remoteDeploymentSecretsFileName, string entryPointProjectName, string solutionFileName, string remoteServiceName,
            IServiceAction<ISecretsFileNamesProvider> addSecretsFileNamesProvider,
            IServiceAction<IFinalizeDeployAction> addFinalizeDeployAction)
        {
            void addSolutionFileName(IServiceCollection services) => services.AddDirectSolutionFileNameProvider(solutionFileName);

            Utilities.DeployRemoteWebsite(remoteDeploymentSecretsFileName, entryPointProjectName, remoteServiceName,
                new ServiceAction<ISolutionFileNameProvider>(addSolutionFileName),
                addSecretsFileNamesProvider,
                addFinalizeDeployAction);
        }

        /// <summary>
        /// Deploys a solution to a remote destination, assuming there is only one solution file in the solution directory so that the solution file name does not need to be specified.
        /// </summary>
#pragma warning disable IDE0060 // Remove unused parameter
        public static void DeployRemote(string remoteDeploymentSecretsFileName, string entryPointProjectName)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            //// Build the DI container.
            //var serviceProvider = new ServiceCollection()
            //    .AddSingleton<IDeploymentSourceFileSystemSiteProvider, DefaultDeploymentSourceFileSystemSiteProvider>()
            //    .AddSingleton<IExecutableFilePathProvider, DefaultExecutableFilePathProvider>()
            //    .AddSingleton<IExecutableFileDirectoryPathProvider, DefaultExecutableFileDirectoryPathProvider>()
            //    .AddSingleton<ISolutionFilePathProvider, StandardSolutionFilePathProvider>()
            //    .AddSingleton<ISolutionFileNameProvider, SingleSolutionFileNameProvider>()
            //    .AddSingleton<IProjectBuildOutputBinariesDirectoryPathProvider, StandardProjectBinariesOutputDirectoryPathProvider>()
            //    .AddSingleton<IEntryPointProjectNameProvider>(new DirectEntryPointProjectNameProvider(entryPointProjectName))
            //    .AddSingleton<IVisualStudioStringlyTypedPathPartsOperator, DefaultVisualStudioStringlyTypedPathPartsOperator>()

            //    .AddSingleton<IDeploymentDestinationFileSystemSiteProvider, SecretsFileRemoteDeploymentDestinationFileSystemSiteProvider>()
            //    .AddSingleton<RemoteDeploymentSecretsSerialization>(serviceProviderInstance =>
            //    {
            //        var serializationProvider = serviceProviderInstance.GetRequiredService<IRemoteDeploymentSecretsSerializationProvider>();

            //        var serialization = serializationProvider.GetRemoteDeploymentSecretsSerialization();
            //        return serialization;
            //    })
            //    .AddSingleton<IRemoteDeploymentSecretsSerializationProvider, DefaultRemoteDeployementSecretsSerializationProvider>()
            //    .AddSingleton<IDeploymentDestinationSecretsFileNameProvider>(new DirectDeploymentDestinationSecretsFileNameProvider(remoteDeploymentSecretsFileName))
            //    .AddSingleton<ISecretsFilePathProvider, DefaultSecretsFilePathProvider>()
            //    .AddSingleton<ISecretsDirectoryPathProvider, AlamaniaSecretsDirectoryPathProvider>()
            //    .AddSingleton<IRivetOrganizationDirectoryPathProvider, BulgariaRivetOrganizationDirectoryPathProvider>()
            //    .AddSingleton<IOrganizationDirectoryNameProvider, DefaultOrganizationDirectoryNameProvider>()
            //    .AddSingleton<IOrganizationStringlyTypedPathOperator, DefaultOrganizationStringlyTypedPathOperator>()
            //    .AddSingleton<IOrganizationsStringlyTypedPathOperator, DefaultOrganizationsStringlyTypedPathOperator>()
            //    .AddSingleton<IDropboxDirectoryPathProvider, DefaultLocalDropboxDirectoryPathProvider>()
            //    .AddSingleton<IUserProfileDirectoryPathProvider, DefaultLocalUserProfileDirectoryPathProvider>()
            //    .AddSingleton<RemoteFileSystemOperator>()
            //    .AddTransient<SftpClientWrapper>(serviceProviderInstance =>
            //    {
            //        var sftpClientWrapperProvider = serviceProviderInstance.GetRequiredService<ISftpClientWrapperProvider>();

            //        var sftpClientWrapper = sftpClientWrapperProvider.GetSftpClientWrapper();
            //        return sftpClientWrapper;
            //    })
            //    .AddSingleton<ISftpClientWrapperProvider, FrisiaSftpClientWrapperProvider>()
            //    .AddSingleton<IAwsEc2ServerSecretsProvider, SuebiaAwsEc2ServerSecretsProvider>()
            //    .AddSingleton<IAwsEc2ServerSecretsFileNameProvider, RemoteDeploymentSerializationAwsEc2ServerSecretsFileNameProvider>()
            //    .AddSingleton<IAwsEc2ServerHostFriendlyNameProvider, RemoteDeploymentSerializationAwsEc2ServerHostFriendlyNameProvider>()

            //    .UseDefaultFileSystemCloningOperator()

            //    .AddSingleton<IJsonFileSerializationOperator, NewtonsoftJsonFileSerializationOperator>()
            //    .AddSingleton<IStringlyTypedPathOperator, StringlyTypedPathOperator>()
            //    .AddSingleton<LocalFileSystemOperator>() // For source.
            //    .AddSingleton<RemoteFileSystemOperator>() // For destination.

            //    .BuildServiceProvider()
            //    ;

            //Utilities.Deploy(serviceProvider);
        }

        /// <summary>
        /// Deploys a solution to a local destination, assuming there is only one solution file in the solution directory so that the solution file name does not need to be specified.
        /// </summary>
        public static void DeployLocal(string localDeploymentSecretsFileName, string entryPointProjectName)
        {
            // NOTE! Has not been tested!

            // Build the DI container.
            var services = new ServiceCollection();

            services
                .AddDeploymentSourceFileSystemSiteProvider(
                    services.AddSingleSolutionFileNameProviderAction(),
                    services.AddDirectEntryPointProjectNameProviderAction(entryPointProjectName))
                .AddLocalDeploymentDestinationFileSystemSiteProvider(
                    services.AddDirectDeploymentDestinationSecretsFileNameProviderAction_Old(localDeploymentSecretsFileName))
                .AddFileSystemCloningOperator()
                ;

            var serviceProvider = services.BuildServiceProvider();

            Utilities.Deploy(serviceProvider);
        }

        /// <summary>
        /// Deploys a solution to a local destination, allowing specification of the solution file name (which is useful in the case when there is more than one solution file in the solution directory).
        /// </summary>
        public static void DeployLocal(string localDeploymentSecretsFileName, string solutionFileName, string entryPointProjectName)
        {
            // Build the DI container.
            var services = new ServiceCollection();

            services
                .AddDeploymentSourceFileSystemSiteProvider(
                    services.AddDirectSolutionFileNameProviderAction(solutionFileName),
                    services.AddDirectEntryPointProjectNameProviderAction(entryPointProjectName))
                .AddLocalDeploymentDestinationFileSystemSiteProvider(
                    services.AddDirectDeploymentDestinationSecretsFileNameProviderAction_Old(localDeploymentSecretsFileName))
                .AddFileSystemCloningOperator()
                ;

            var serviceProvider = services.BuildServiceProvider();

            Utilities.Deploy(serviceProvider);
        }

        private static void Deploy(IServiceProvider serviceProvider)
        {
            var sourceSiteProvider = serviceProvider.GetRequiredService<IDeploymentSource_FileSystemSiteProvider>();
            var sourceSite = sourceSiteProvider.GetDeploymentSourceFileSystemSite();

            var destinationSiteProvider = serviceProvider.GetRequiredService<IDeploymentDestination_FileSystemSiteProvider>();
            var destinationSite = destinationSiteProvider.GetDeploymentDestinationFileSystemSite();

            var cloningOperator = serviceProvider.GetRequiredService<IFileSystemCloningOperator>();

            cloningOperator.Clone(sourceSite, destinationSite, FileSystemCloningOptions.Default);
        }
    }
}
