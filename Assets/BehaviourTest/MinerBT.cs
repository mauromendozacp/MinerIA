using System;
using System.Collections.Generic;
using UnityEngine;

namespace IA.BehaviourTest
{
    public class MinerBT : BehaviourTree
    {
        #region EXPOSED_FIELDS
        [SerializeField] private float miningDelay = 0f;
        [SerializeField] private int maxGoldAmount = 0;
        #endregion

        #region PRIVATE_FIELDS
        private float miningTimer = 0f;
        private int currentGold = 0;
        #endregion

        #region PROTECTED_METHODS
        protected override TreeNode Setup()
        {
            TreeNode root = new Root(new List<TreeNode>
            {
                new Sequence(new List<TreeNode>
                {
                    new Task(null, Mining)
                })
            });

            return root;
        }
        #endregion

        #region PRIVATE_METHODS
        private void Mining(Action onSuccess, Action onFailure)
        {
            miningTimer += Time.deltaTime;
            Debug.Log("Mining..");

            if (miningTimer > miningDelay)
            {
                miningTimer = 0;
                currentGold++;

                Debug.Log("Gold recollected: " + currentGold);

                if (currentGold == maxGoldAmount)
                {
                    Debug.Log("Gold recollecting completed.");
                    onSuccess?.Invoke();
                }
            }
        }
        #endregion
    }
}