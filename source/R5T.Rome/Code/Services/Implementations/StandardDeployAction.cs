using System;
using System.Collections.Generic;


namespace R5T.Rome
{
    public class StandardDeployAction : IDeployAction
    {
        private IEnumerable<IPreFileCopyDeployAction> PreFileCopyDeployActions { get; }
        private IFileCopyDeployAction FileCopyDeployAction { get; }
        private IEnumerable<IPostFileCopyDeployAction> PostFileCopyDeployActions { get; }
        private IFinalizeDeployAction FinalizeDeployAction { get; }
        private IEnumerable<IPostFinalizeDeployAction> PostFinalizeDeployActions { get; }

        public StandardDeployAction(
            IEnumerable<IPreFileCopyDeployAction> preFileCopyDeployActions,
            IFileCopyDeployAction fileCopyDeployAction,
            IEnumerable<IPostFileCopyDeployAction> postFileCopyDeployActions,
            IFinalizeDeployAction finalizeDeployAction,
            IEnumerable<IPostFinalizeDeployAction> postFinalizeDeployActions)
        {
            this.PreFileCopyDeployActions = preFileCopyDeployActions;
            this.FileCopyDeployAction = fileCopyDeployAction;
            this.PostFileCopyDeployActions = postFileCopyDeployActions;
            this.FinalizeDeployAction = finalizeDeployAction;
            this.PostFinalizeDeployActions = postFinalizeDeployActions;
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

            this.FinalizeDeployAction.Run();

            foreach (var postFinalizeDeployAction in this.PostFinalizeDeployActions)
            {
                postFinalizeDeployAction.Run();
            }
        }
    }
}
