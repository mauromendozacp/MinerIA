﻿using System.Collections.Generic;

namespace IA.BehaviourTest
{
    public class Not : TreeNode
    {
        #region CONSTRUCTORS
        public Not()
        {
        }

        public Not(List<TreeNode> childrens) : base(childrens)
        {
        }
        #endregion

        #region PUBLIC_METHODS
        public override NodeState Evaluate()
        {
            foreach (TreeNode node in childrens)
            {
                if (node.Evaluate() != NodeState.RUNNING)
                {
                    state = node.State;
                    return state;
                }
            }

            state = NodeState.RUNNING;
            return state;
        }
        #endregion
    }
}