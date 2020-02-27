using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;

using R5T.Antium;
using R5T.Antium.Default;
using R5T.Antium.Standard;
using R5T.Dacia;
using R5T.Frisia.Suebia;
using R5T.Lombardy;
using R5T.Lombardy.Standard;
using R5T.Pictia;
using R5T.Teutonia;


namespace R5T.Rome
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="DirectRemoteServiceNameProvider"/> implementation of <see cref="IRemoteServiceNameProvider"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddDirectRemoteServiceNameProvider(this IServiceCollection services, string remoteServiceName)
        {
            services.AddSingleton<IRemoteServiceNameProvider, DirectRemoteServiceNameProvider>(serviceProvider =>
            {
                var directRemoteServiceNameProvider = new DirectRemoteServiceNameProvider(remoteServiceName);
                return directRemoteServiceNameProvider;
            });

            return services;
        }

        /// <summary>
        /// Adds the <see cref="DirectRemoteServiceNameProvider"/> implementation of <see cref="IRemoteServiceNameProvider"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static ServiceAction<IRemoteServiceNameProvider> AddDirectRemoteServiceNameProviderAction(this IServiceCollection services, string remoteServiceName)
        {
            var serviceAction = new ServiceAction<IRemoteServiceNameProvider>(() => services.AddDirectRemoteServiceNameProvider(remoteServiceName));
            return serviceAction;
        }

        /// <summary>
        /// Adds the <see cref="VerifyWebsiteStartedAction"/> implementation of <see cref="IPostFinalizeDeployAction"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddVerifyWebsiteStartedAction(this IServiceCollection services,
            ServiceAction<ISftpClientWrapperProvider> addSftpClientWrapperProvider,
            ServiceAction<IRemoteServiceNameProvider> addRemoteServiceNameProvider)
        {
            services
                .AddSingleton<IPostFinalizeDeployAction, VerifyWebsiteStartedAction>()
                .RunServiceAction(addSftpClientWrapperProvider)
                .RunServiceAction(addRemoteServiceNameProvider)
                ;

            return services;
        }

        /// <summary>
        /// Adds the <see cref="VerifyWebsiteStartedAction"/> implementation of <see cref="IPostFinalizeDeployAction"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static ServiceAction<IPostFinalizeDeployAction> AddVerifyWebsiteStartedActionAction(this IServiceCollection services,
            ServiceAction<ISftpClientWrapperProvider> addSftpClientWrapperProvider,
            ServiceAction<IRemoteServiceNameProvider> addRemoteServiceNameProvider)
        {
            var serviceAction = new ServiceAction<IPostFinalizeDeployAction>(() => services.AddVerifyWebsiteStartedAction(
                addSftpClientWrapperProvider,
                addRemoteServiceNameProvider));
            return serviceAction;
        }

        /// <summary>
        /// Adds the <see cref="CopySecretsFilesAction"/> implementation of <see cref="IPostFileCopyDeployAction"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddCopySecretsFilesAction(this IServiceCollection services,
            ServiceAction<ISecretsFileNamesProvider> addSecretsFileNamesProvider,
            ServiceAction<IDeploymentSource_SecretsDirectory_FileSystemSiteProvider> addDeploymentSource_SecretsDirectory_FileSystemSiteProvider,
            ServiceAction<IDeploymentDestination_SecretsDirectory_FileSystemSiteProvider> addDeploymentDestination_SecretsDirectory_FileSystemSiteProvider,
            ServiceAction<IStringlyTypedPathOperator> addStringlyTypedPathOperator)
        {
            services
                .AddSingleton<IPostFileCopyDeployAction, CopySecretsFilesAction>()
                .RunServiceAction(addSecretsFileNamesProvider)
                .RunServiceAction(addDeploymentSource_SecretsDirectory_FileSystemSiteProvider)
                .RunServiceAction(addDeploymentDestination_SecretsDirectory_FileSystemSiteProvider)
                .RunServiceAction(addStringlyTypedPathOperator)
                ;

            return services;
        }

        /// <summary>
        /// Adds the <see cref="CopySecretsFilesAction"/> implementation of <see cref="IPostFileCopyDeployAction"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static ServiceAction<IPostFileCopyDeployAction> AddCopySecretsFilesActionAction(this IServiceCollection services,
            ServiceAction<ISecretsFileNamesProvider> addSecretsFileNamesProvider,
            ServiceAction<IDeploymentSource_SecretsDirectory_FileSystemSiteProvider> addDeploymentSource_SecretsDirectory_FileSystemSiteProvider,
            ServiceAction<IDeploymentDestination_SecretsDirectory_FileSystemSiteProvider> addDeploymentDestination_SecretsDirectory_FileSystemSiteProvider,
            ServiceAction<IStringlyTypedPathOperator> addStringlyTypedPathOperator)
        {
            var serviceAction = new ServiceAction<IPostFileCopyDeployAction>(() => services.AddCopySecretsFilesAction(
                addSecretsFileNamesProvider,
                addDeploymentSource_SecretsDirectory_FileSystemSiteProvider,
                addDeploymentDestination_SecretsDirectory_FileSystemSiteProvider,
                addStringlyTypedPathOperator));
            return serviceAction;
        }

        /// <summary>
        /// Adds the <see cref="CopySecretsFilesAction"/> implementation of the <see cref="IPostFileCopyDeployAction"/> service.
        /// </summary>
        public static IServiceCollection AddRemoteCopySecretsFilesAction(this IServiceCollection services,
            ServiceAction<ISecretsFileNamesProvider> addSecretsFileNamesProvider,
            ServiceAction<IAwsEc2ServerHostFriendlyNameProvider> addAwsEc2ServerHostFriendlyNameProvider,
            ServiceAction<IDeploymentDestinationSecretsFileNameProvider> addDeploymentDestinationSecretsFileNameProvider)
        {
            services.AddCopySecretsFilesAction(
                addSecretsFileNamesProvider,
                services.AddDeploymentSource_SecretsDirectory_FileSystemSiteProviderAction(),
                services.AddRemoteDeploymentDestination_SecretsDirectory_FileSystemSiteProviderAction(
                    addAwsEc2ServerHostFriendlyNameProvider,
                    addDeploymentDestinationSecretsFileNameProvider),
                services.AddStringlyTypedPathOperatorAction());

            return services;
        }

        /// <summary>
        /// Adds the <see cref="CopySecretsFilesAction"/> implementation of the <see cref="IPostFileCopyDeployAction"/> service.
        /// </summary>
        public static ServiceAction<IPostFileCopyDeployAction> AddRemoteCopySecretsFilesActionAction(this IServiceCollection services,
            ServiceAction<ISecretsFileNamesProvider> addSecretsFileNamesProvider,
            ServiceAction<IAwsEc2ServerHostFriendlyNameProvider> addAwsEc2ServerHostFriendlyNameProvider,
            ServiceAction<IDeploymentDestinationSecretsFileNameProvider> addDeploymentDestinationSecretsFileNameProvider)
        {
            var serviceAction = new ServiceAction<IPostFileCopyDeployAction>(() => services.AddRemoteCopySecretsFilesAction(
                addSecretsFileNamesProvider,
                addAwsEc2ServerHostFriendlyNameProvider,
                addDeploymentDestinationSecretsFileNameProvider));
            return serviceAction;
        }

        /// <summary>
        /// Adds the <see cref="StandardDeployAction"/> implementation of <see cref="IDeployAction"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddStandardDeployAction(this IServiceCollection services,
            IEnumerable<ServiceAction<IPreFileCopyDeployAction>> addPreFileCopyDeployActions,
            ServiceAction<IFileCopyDeployAction> addFileCopyDeployAction,
            IEnumerable<ServiceAction<IPostFileCopyDeployAction>> addPostFileCopyDeployActions,
            ServiceAction<IFinalizeDeployAction> addFinalizeDeployAction,
            IEnumerable<ServiceAction<IPostFinalizeDeployAction>> addPostFinalizeDeployActions)
        {
            services
                .AddSingleton<IDeployAction, StandardDeployAction>()
                .RunServiceActions(addPreFileCopyDeployActions)
                .RunServiceAction(addFileCopyDeployAction)
                .RunServiceActions(addPostFileCopyDeployActions)
                .RunServiceAction(addFinalizeDeployAction)
                .RunServiceActions(addPostFinalizeDeployActions)
                ;

            return services;
        }

        /// <summary>
        /// Adds the <see cref="StandardDeployAction"/> implementation of <see cref="IDeployAction"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static ServiceAction<IDeployAction> AddStandardDeployActionAction(this IServiceCollection services,
            IEnumerable<ServiceAction<IPreFileCopyDeployAction>> addPreFileCopyDeployActions,
            ServiceAction<IFileCopyDeployAction> addFileCopyDeployAction,
            IEnumerable<ServiceAction<IPostFileCopyDeployAction>> addPostFileCopyDeployActions,
            ServiceAction<IFinalizeDeployAction> addFinalizeDeployAction,
            IEnumerable<ServiceAction<IPostFinalizeDeployAction>> addPostFinalizeDeployActions)
        {
            var serviceAction = new ServiceAction<IDeployAction>(() => services.AddStandardDeployAction(
                addPreFileCopyDeployActions,
                addFileCopyDeployAction,
                addPostFileCopyDeployActions,
                addFinalizeDeployAction,
                addPostFinalizeDeployActions));
            return serviceAction;
        }

        /// <summary>
        /// Adds the <see cref="StandardFileCopyDeployAction"/>  implementation of <see cref="IFileCopyDeployAction"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddStandardFileCopyDeployAction(this IServiceCollection services,
            ServiceAction<IDeploymentSource_FileSystemSiteProvider> addDeploymentSource_FileSystemSiteProvider,
            ServiceAction<IDeploymentDestination_FileSystemSiteProvider> addDeploymentDestination_FileSystemSiteProvider,
            ServiceAction<IFileSystemCloningOperator> addFileSystemCloningOperator)
        {
            services
                .AddSingleton<IFileCopyDeployAction, StandardFileCopyDeployAction>()
                .RunServiceAction(addDeploymentSource_FileSystemSiteProvider)
                .RunServiceAction(addDeploymentDestination_FileSystemSiteProvider)
                .RunServiceAction(addFileSystemCloningOperator)
                ;

            return services;
        }

        /// <summary>
        /// Adds the <see cref="StandardFileCopyDeployAction"/>  implementation of <see cref="IFileCopyDeployAction"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static ServiceAction<IFileCopyDeployAction> AddStandardFileCopyDeployActionAction(this IServiceCollection services,
            ServiceAction<IDeploymentSource_FileSystemSiteProvider> addDeploymentSource_FileSystemSiteProvider,
            ServiceAction<IDeploymentDestination_FileSystemSiteProvider> addDeploymentDestination_FileSystemSiteProvider,
            ServiceAction<IFileSystemCloningOperator> addFileSystemCloningOperator)
        {
            var serviceAction = new ServiceAction<IFileCopyDeployAction>(() => services.AddStandardFileCopyDeployAction(
                addDeploymentSource_FileSystemSiteProvider,
                addDeploymentDestination_FileSystemSiteProvider,
                addFileSystemCloningOperator));
            return serviceAction;
        }

        /// <summary>
        /// Adds the <see cref="PublishPreFileCopyDeployAction"/> implementation of <see cref="IPreFileCopyDeployAction"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddPublishPreFileCopyDeployAction(this IServiceCollection services,
            ServiceAction<IPublishAction> addPublishAction)
        {
            services
                .AddSingleton<IPreFileCopyDeployAction, PublishPreFileCopyDeployAction>()
                .RunServiceAction(addPublishAction)
                ;

            return services;
        }

        /// <summary>
        /// Adds the <see cref="PublishPreFileCopyDeployAction"/> implementation of <see cref="IPreFileCopyDeployAction"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static ServiceAction<IPreFileCopyDeployAction> AddPublishPreFileCopyDeployActionAction(this IServiceCollection services,
            ServiceAction<IPublishAction> addPublishAction)
        {
            var serviceAction = new ServiceAction<IPreFileCopyDeployAction>(() => services.AddPublishPreFileCopyDeployAction(
                addPublishAction));
            return serviceAction;
        }
    }
}
