using System;
using UnityEngine;

public enum TEAM
{
    NONE,
    A,
    B
}

public class Tank : TankBase
{
    #region PRIVATE_FIELDS
    private float fitness = 1f;
    public TEAM team = TEAM.NONE;

    public Action<TEAM> onUpdateScore;
    #endregion

    #region PROPERTIES
    public float Fitness => fitness;
    #endregion

    #region PROTECTED_METHODS
    protected override void OnReset()
    {
        fitness = 1;
    }

    protected override void OnThink(float dt)
    {
        Vector3 nearTankPos = nearTankInNearMine.transform.position;
        Vector3 dirToMine = GetDirToMine(nearMine);
        Vector3 dir = transform.forward;

        inputs[0] = dirToMine.x;
        inputs[1] = dirToMine.z;
        inputs[2] = dir.x;
        inputs[3] = dir.z;
        inputs[4] = nearTankPos.x;
        inputs[5] = nearTankPos.z;

        float[] output = brain.Synapsis(inputs);

        SetForces(output[0], output[1], dt);
    }

    protected override void OnTakeMine(GameObject mine)
    {
        TEAM mineTeam = mine.GetComponent<Mine>().team;
        bool goodPoint = team == mineTeam;

        fitness *= goodPoint ? 2f : 1f;
        genome.Fitness = fitness;

        if (goodPoint)
        {
            onUpdateScore?.Invoke(team);
        }
    }
    #endregion
}
