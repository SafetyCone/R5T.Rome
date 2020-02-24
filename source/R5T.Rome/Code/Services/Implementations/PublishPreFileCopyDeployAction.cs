using System;

using R5T.Antium;


namespace R5T.Rome
{
    public class PublishPreFileCopyDeployAction : IPreFileCopyDeployAction
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
