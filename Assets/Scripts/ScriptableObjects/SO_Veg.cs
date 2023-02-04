using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Veg_Settings", menuName = "Beets/Veg")]
public class SO_Veg : ScriptableObject
{
    [Header("Metadata")]
    public string Name;

    [Header("Audio")]
    [Tooltip("Eg. bass, piano, harp")]
    public string AudioType; 
    public AudioClip Clip;

    [Header("Visuals")]
    public GameObject Prefab;
    public Sprite Icon;

    [Header("Economy")]
    public int UnlockCost;
    public LifeStage[] LifeStages;

    [System.Serializable]
    public class LifeStage
    {
        public int LevelNum;
        public int HarvestValue;
        public int WateringScoreToUnlock;
    }

    public bool Unlocked
    {
        get { return PlayerPrefs.GetInt(Name + "_Unlocked", 0) == 1; }
        set { PlayerPrefs.SetInt(Name + "_Unlocked", value ? 1 : 0); }
    }
}
