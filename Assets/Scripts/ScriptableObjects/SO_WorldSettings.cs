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
    public int StartRowCost = 100;
    public int EndRowCost = 200;
    public int RowsUnlockedPerPurchase = 1;
    public int CountRowPurchases => MaxHeight - StartHeight;
    public AnimationCurve RowCostCurve;
    public int RowCost(int nextRow) => Mathf.RoundToInt(
        StartRowCost +
        ((EndRowCost - StartRowCost) * RowCostCurve.Evaluate(nextRow / (float)CountRowPurchases))
    );

    public int StartColumnCost = 25;
    public int EndColumnCost = 1000;
    public int ColumnsUnlockedPerPurchase = 2;
    public int CountColumnPurchases => MaxWidth - StartWidth;
    public AnimationCurve ColumnCostCurve;
    public int ColumnCost(int nextColumn) => Mathf.RoundToInt(
        StartColumnCost +
        ((EndColumnCost - StartColumnCost) * RowCostCurve.Evaluate(nextColumn / (float)CountColumnPurchases))
    );
}
