using System;

using R5T.Antium;

using R5T.T0064;


namespace R5T.Rome
{
    [ServiceImplementationMarker]
    public class PublishPreFileCopyDeployAction : IPreFileCopyDeployAction, IServiceImplementation
    {
        private IPublishAction PublishAction { get; }


        public PublishPreFileCopyDeployAction(
            IPublishAction publishAction)
        {
            this.PublishAction = publishAction;
        }

        public void Run()
        {
            this.PublishAction.Publish();
        }
    }
}
