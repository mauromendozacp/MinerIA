using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    public int redScore = 0;
    public int greenScore = 0;

    public void UpdateScore(TEAM team)
    {
        if (team == TEAM.A)
        {
            redScore++;
        }
        else
        {
            greenScore++;
        }
    }

    public void ResetScore()
    {
        redScore = 0;
        greenScore = 0;
    }

    public TEAM GetLoserTeam()
    {
        if (redScore == greenScore)
        {
            return TEAM.NONE;
        }

        return redScore < greenScore ? TEAM.A : TEAM.B;
    }
}