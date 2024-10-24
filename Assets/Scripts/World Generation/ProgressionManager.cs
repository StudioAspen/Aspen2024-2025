using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    private int empowerTokens;
    private int weakenTokens;

    public void RestockTokens(int landCount)
    {
        empowerTokens = landCount / 2;
        weakenTokens = landCount / 2;
    }

    public void ConsumeWeakenToken()
    {
        weakenTokens--;
    }

    public void ConsumeEmpowerToken()
    {
        empowerTokens--;
    }
}
