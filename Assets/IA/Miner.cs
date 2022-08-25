using UnityEngine;

public class Miner : MonoBehaviour
{
    #region PRIVATE_FIELDS
    private GameObject mine;
    private GameObject deposit;
    private GameObject repose;
    private FSM fsm;

    private int mineUses = 10;
    private float miningTime = 5.0f;
    private float reposingTime = 5.0f;
    private float currentMiningTime = 0.0f;
    private float currentReposingTime = 0.0f;
    #endregion

    #region ENUMS
    enum States
    {
        Mining,
        Reposing,
        GoToMine,
        GoToDeposit,
        GoToRepose,
        Idle,

        _Count
    }

    enum Flags
    {
        OnFullInventory,
        OnEndRepose,
        OnReachMine,
        OnReachDeposit,
        OnReachRepose,
        OnEmptyMine,
        OnStopMine,

        _Count
    }
    #endregion

    #region PUBLIC_METHODS
    public void Init(GameObject mine, GameObject deposit, GameObject repose)
    {
        this.mine = mine;
        this.deposit = deposit;
        this.repose = repose;
    }

    public void StartMiner()
    {
        fsm = new FSM((int)States._Count, (int)Flags._Count);
        fsm.ForceCurretState((int)States.GoToMine);

        fsm.SetRelation((int)States.GoToMine, (int)Flags.OnReachMine, (int)States.Mining);
        fsm.SetRelation((int)States.Mining, (int)Flags.OnFullInventory, (int)States.GoToDeposit);
        fsm.SetRelation((int)States.GoToDeposit, (int)Flags.OnReachDeposit, (int)States.GoToMine);
        fsm.SetRelation((int)States.GoToDeposit, (int)Flags.OnEmptyMine, (int)States.Idle);

        fsm.SetRelation((int)States.GoToMine, (int)Flags.OnStopMine, (int)States.GoToRepose);
        fsm.SetRelation((int)States.Mining, (int)Flags.OnStopMine, (int)States.GoToRepose);
        fsm.SetRelation((int)States.GoToDeposit, (int)Flags.OnStopMine, (int)States.GoToRepose);
        fsm.SetRelation((int)States.GoToRepose, (int)Flags.OnReachRepose, (int)States.Reposing);
        fsm.SetRelation((int)States.Reposing, (int)Flags.OnEndRepose, (int)States.Mining);

        fsm.AddBehaviour((int)States.Idle, () => { Debug.Log("Idle"); }, () =>
        {
            fsm.SetFlag((int)Flags.OnStopMine);
        });

        fsm.AddBehaviour((int)States.Mining, () =>
        {
            if (currentMiningTime < miningTime)
            {
                currentMiningTime += Time.deltaTime;
            }
            else
            {
                currentMiningTime = 0.0f;
                fsm.SetFlag((int)Flags.OnFullInventory);
                mineUses--;
            }
        }, () =>
        {
            fsm.SetFlag((int)Flags.OnStopMine);
        });

        fsm.AddBehaviour((int)States.GoToMine, () =>
        {
            Vector2 dir = (mine.transform.position - transform.position).normalized;

            if (Vector2.Distance(mine.transform.position, transform.position) > 1.0f)
            {
                Vector2 movement = dir * 10.0f * Time.deltaTime;
                transform.position += new Vector3(movement.x, movement.y);
            }
            else
            {
                fsm.SetFlag((int)Flags.OnReachMine);
            }
        }, () =>
        {
            fsm.SetFlag((int)Flags.OnStopMine);
        });

        fsm.AddBehaviour((int)States.GoToDeposit, () =>
        {
            Vector2 dir = (deposit.transform.position - transform.position).normalized;

            if (Vector2.Distance(deposit.transform.position, transform.position) > 1.0f)
            {
                Vector2 movement = dir * 10.0f * Time.deltaTime;
                transform.position += new Vector3(movement.x, movement.y);
            }
            else
            {
                if (mineUses <= 0)
                    fsm.SetFlag((int)Flags.OnEmptyMine);
                else
                    fsm.SetFlag((int)Flags.OnReachDeposit);
            }
        }, () =>
        {
            fsm.SetFlag((int)Flags.OnStopMine);
        });

        fsm.AddBehaviour((int)States.GoToRepose, () =>
        {
            Vector2 dir = (repose.transform.position - transform.position).normalized;

            if (Vector2.Distance(repose.transform.position, transform.position) > 1.0f)
            {
                Vector2 movement = dir * 10.0f * Time.deltaTime;
                transform.position += new Vector3(movement.x, movement.y);
            }
            else
            {
                fsm.SetFlag((int)Flags.OnReachRepose);
            }
        });

        fsm.AddBehaviour((int)States.Reposing, () =>
        {
            if (currentReposingTime < reposingTime)
            {
                currentReposingTime += Time.deltaTime;
            }
            else
            {
                currentReposingTime = 0.0f;
                fsm.SetFlag((int)Flags.OnEndRepose);
            }
        });
    }

    public void UpdateMiner()
    {
        fsm.Update();
    }

    public void GoToRepose()
    {
        fsm.SetFlag((int)Flags.OnStopMine);
    }
    #endregion
}
