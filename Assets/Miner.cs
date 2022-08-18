using UnityEngine;

public class Miner : MonoBehaviour
{
    #region PRIVATE_FIELDS
    private GameObject mine;
    private GameObject deposit;
    private FSM fsm;

    private int mineUses = 10;
    private float miningTime = 5.0f;
    private float currentMiningTime = 0.0f;
    #endregion

    #region ENUMS
    enum States
    {
        Mining,
        GoToMine,
        GoToDeposit,
        Idle,

        _Count
    }

    enum Flags
    {
        OnFullInventory,
        OnReachMine,
        OnReachDeposit,
        OnEmpyMine,

        _Count
    }
    #endregion

    #region PUBLIC_METHODS
    public void Init(GameObject mine, GameObject deposit)
    {
        this.mine = mine;
        this.deposit = deposit;
    }

    public void StartMiner()
    {
        fsm = new FSM((int)States._Count, (int)Flags._Count);
        fsm.ForceCurretState((int)States.GoToMine);

        fsm.SetRelation((int)States.GoToMine, (int)Flags.OnReachMine, (int)States.Mining);
        fsm.SetRelation((int)States.Mining, (int)Flags.OnFullInventory, (int)States.GoToDeposit);
        fsm.SetRelation((int)States.GoToDeposit, (int)Flags.OnReachDeposit, (int)States.GoToMine);
        fsm.SetRelation((int)States.GoToDeposit, (int)Flags.OnEmpyMine, (int)States.Idle);

        fsm.AddBehaviour((int)States.Idle, () => { Debug.Log("Idle"); });

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
                    fsm.SetFlag((int)Flags.OnEmpyMine);
                else
                    fsm.SetFlag((int)Flags.OnReachDeposit);
            }
        });
    }

    public void UpdateMiner()
    {
        fsm.Update();
    }
    #endregion
}
