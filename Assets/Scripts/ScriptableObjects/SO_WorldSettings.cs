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
}
