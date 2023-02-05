using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_WorldSettings", menuName = "Beets/Settings")]
public class SO_WorldSettings : ScriptableObject
{
    [Header("Level Generation")]
    public int StartWidth;
    public int StartHeight;
    public int MaxWidth;
    public int MaxHeight;
    public Vector2Int StartPos;
    [Space(10)]
    public GameObject PotPrefab;

    [Header("Level Expansion")]
    public int StarRowCost = 100;
    public int EndRowCost = 200;
    public int RowsToBuy => MaxHeight - StartHeight;
    public AnimationCurve RowCostCurve;
    public int RowCost(int nextRow) => Mathf.RoundToInt(RowCostCurve.Evaluate(nextRow / (float)RowsToBuy));

    public int StartColumnCost = 25;
    public int EndColumnCost = 1000;
    public int ColumnsToBuy => MaxWidth - StartWidth;
    public AnimationCurve ColumnCostCurve;
    public int ColumnCost(int nextColumn) => Mathf.RoundToInt(RowCostCurve.Evaluate(nextColumn / (float)ColumnsToBuy));
}
