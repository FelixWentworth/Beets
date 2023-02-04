using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_WorldSettings", menuName = "Beets/Settings")]
public class SO_WorldSettings : ScriptableObject
{
    [Header("Pitch")]
    public float MinPitch;
    public float MaxPitch;
    [Tooltip("The number of steps between min and max pitch, eg. min = 0, max = 1, steps = 10 would make an increment of 0.1")]
    public float PitchSteps;

    [Header("Level Generation")]
    public int StartWidth;
    public int StartHeight;
    public int MaxWidth;
    public int MaxHeight;
    public Vector2Int StartPos;
    [Space(10)]
    public GameObject PotPrefab;
}
