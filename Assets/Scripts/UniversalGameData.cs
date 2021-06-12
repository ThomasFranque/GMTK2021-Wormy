using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// THE shit show of a class
/// Use at will
/// But with a condom
/// </summary>
public static class UniversalGameData
{
    public static event System.Action leafAddedCallback;
    private static int leafs;
    public static int TOTAL_GAME_LEAFS = 1;
    public static int Leafs 
    {
        get
        {
            return leafs;
        }
        set
        {
            leafs = value;
            leafs = Mathf.Clamp(leafs, 1, TOTAL_GAME_LEAFS);
            leafAddedCallback?.Invoke();
        }
    }
    public static float TotalLeafs => Leafs / TOTAL_GAME_LEAFS;
}
