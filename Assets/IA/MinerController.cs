using System.Collections.Concurrent;
using System.Threading.Tasks;

using UnityEngine;

public class MinerController : MonoBehaviour
{
    #region EXPOSED_FIELDS
    [SerializeField] private GameObject minerPrefab = null;
    [SerializeField] private Transform minersHolder = null;
    [SerializeField] private Transform mineStartPos = null;
    [SerializeField] private int minersLength = 0;

    [SerializeField] private GameObject mine = null;
    [SerializeField] private GameObject deposit = null;
    [SerializeField] private GameObject repose = null;
    #endregion

    #region PRIVATE_FIELDS
    private ConcurrentBag<Miner> miners = null;
    private ParallelOptions parrallel = null;
    #endregion

    #region UNITY_CALLS
    private void Start()
    {
        miners = new();
        parrallel = new ParallelOptions() { MaxDegreeOfParallelism = 8 };

        for (int i = 0; i < minersLength; i++)
        {
            GameObject minerGO = Instantiate(minerPrefab, minersHolder);
            minerGO.transform.position = mineStartPos.position;

            Miner miner = minerGO.GetComponent<Miner>();
            miner.Init(mine, deposit, repose);
            miner.StartMiner();

            miners.Add(miner);
        }
    }

    private void Update()
    {
        Parallel.ForEach(miners, parrallel, minedou =>
        {
            minedou.UpdateMiner();
        });

        GoMinerToRepose();
    }
    #endregion

    #region PRIVATE_METHODS
    private void GoMinerToRepose()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (miners.TryPeek(out Miner miner))
            {
                miner.GoToRepose();
            };
        }
    }
    #endregion
}
