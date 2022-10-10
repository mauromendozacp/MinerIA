using System.Collections.Generic;

namespace IA.BehaviourTest
{
    public class Root : TreeNode
    {
        #region CONSTRUCTORS
        public Root()
        {
        }

        public Root(List<TreeNode> childrens) : base(childrens)
        {
        }
        #endregion

        #region PUBLIC_METHODS
        public override NodeState Evaluate()
        {
            foreach (TreeNode node in childrens)
            {
                if (node.Evaluate() != NodeState.FAILURE)
                {
                    state = node.State;
                    return state;
                }
            }

            state = NodeState.FAILURE;
            return state;
        }
        #endregion
    }
}