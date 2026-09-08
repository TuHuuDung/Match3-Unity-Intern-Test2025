using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : ScriptableObject
{
    public int BoardSizeX = 5;

    public int BoardSizeY = 5;

    public int MatchesMin = 3;

    public int LevelMoves = 16;

    public float LevelTime = 30f;

    public float TimeForHint = 5f;

    [Header("Task 2 - Collect & Clear gameplay")]
    public int BottomCellsCount = 5;

    [Header("Task 3 - Time Attack mode")]
    public float TimeAttackDuration = 60f;
}