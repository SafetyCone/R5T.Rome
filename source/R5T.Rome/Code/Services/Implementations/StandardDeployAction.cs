using System;
using System.Collections.Generic;


namespace R5T.Rome
{
    public class StandardDeployAction : IDeployAction
    {
        private IEnumerable<IPreFileCopyDeployAction> PreFileCopyDeployActions { get; }
        private IFileCopyDeployAction FileCopyDeployAction { get; }
        private IEnumerable<IPostFileCopyDeployAction> PostFileCopyDeployActions { get; }


        public StandardDeployAction(
            IEnumerable<IPreFileCopyDeployAction> preFileCopyDeployActions,
            IFileCopyDeployAction fileCopyDeployAction,
            IEnumerable<IPostFileCopyDeployAction> postFileCopyDeployActions)
        {
            this.PreFileCopyDeployActions = preFileCopyDeployActions;
            this.FileCopyDeployAction = fileCopyDeployAction;
            this.PostFileCopyDeployActions = postFileCopyDeployActions;
        }

        public void Run()
        {
            foreach (var preFileCopyDeployAction in this.PreFileCopyDeployActions)
            {
                preFileCopyDeployAction.Run();
            }

            this.FileCopyDeployAction.Run();

            foreach (var postFileCopyDeployAction in this.PostFileCopyDeployActions)
            {
                postFileCopyDeployAction.Run();
            }
        }
    }
}
