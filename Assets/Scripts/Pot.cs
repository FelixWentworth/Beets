using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pot : MonoBehaviour
{
    public bool HasVeg => _veg != null;
    public Vector2Int GridPos => _gridPos;
    public bool Active => _active;
    [SerializeField] private Transform _vegSpawnPoint;
    [SerializeField] private GameObject _activeState;
    [SerializeField] private GameObject _inactiveState;
    [SerializeField] private MeshRenderer _activeMesh;
    [SerializeField] private Color _highlightColor;
    [SerializeField] private Color _originalColor;

    [SerializeField] private AudioSource _source;
    [SerializeField] private float _startingVegScaleMultiplier;

    private GameObject _currentVeg;
    private SO_Veg _veg;
    private float _wateringLevel = 0f;
    private Vector2Int _gridPos;
    private bool _active;
    private Vector3 _finalScale;

    private float _growthSecondsElapsed = 0f;
    
    private void Update()
    {
        if (HasVeg)
        {
            _growthSecondsElapsed += Time.deltaTime;
            float growthPercentage = Mathf.Clamp01(_growthSecondsElapsed / _veg.SecondsToFullGrowth);
            int growthStage = 0;
            if (growthPercentage > 0.5f && growthPercentage < 0.99f)
            {
                growthStage = 1;
            } else if (growthPercentage >= 0.99f)
            {
                growthStage = 2;
            }
            LerpToGrowthStage(growthStage);
            // _currentVeg.transform.localScale = _finalScale * growthPercentage;
        }
    }

    void LerpToGrowthStage(int stage)
    {
        float scaleMultiplier = _startingVegScaleMultiplier;
        if (stage == 1)
        {
            scaleMultiplier = 0.5f;
        } else if (stage > 1)
        {
            scaleMultiplier = 1f;
        }

        var modelTransform = _currentVeg.transform.GetChild(0).transform;
        var targetScale = _finalScale * scaleMultiplier; // startingScale
        var newScale = Vector3.Lerp(modelTransform.localScale, targetScale, 0.1f);
        modelTransform.localScale = newScale;
    }

    public void Set(Vector2Int gridPos, bool active = true, SO_Veg vegToSpawn = null)
    {
        _active = active;
        _gridPos = gridPos;
        _activeState.SetActive(active);
        _inactiveState.SetActive(!active);


        if (vegToSpawn != null)
        {
            _veg = vegToSpawn;
            _currentVeg = Instantiate(vegToSpawn.Prefab, _vegSpawnPoint);
            var modelTransform = _currentVeg.transform.GetChild(0); 
            _finalScale = modelTransform.localScale;
            modelTransform.localScale = Vector3.one * _startingVegScaleMultiplier;
            _wateringLevel = 0f;
        }
    }

    public int GetHarvestValue()
    {
        if (_veg == null)
        {
            return 0;
        }
        var value = 0;
        
        
        // TODO - decide whether to do growth over time or active watering. Trying growth over time here:
        float timePerStage = _veg.SecondsToFullGrowth / 3;
        int growthStagesPassed = (int)(_growthSecondsElapsed / timePerStage);
        print($"time per stage: {timePerStage}. elapsed: {_growthSecondsElapsed}. stages complete: {growthStagesPassed}");
        if (growthStagesPassed < _veg.LifeStages.Length)
        {
            int harvestReward = _veg.LifeStages[growthStagesPassed].HarvestValue;
            return harvestReward;
        }
        print("defaulting to last harvest value");
        return _veg.LifeStages.Last().HarvestValue;
        

        // foreach(var stage in _veg.LifeStages)
        // {
        //     if (_wateringLevel >= stage.WateringScoreToUnlock)
        //     {
        //         value = stage.HarvestValue;
        //     }
        // }
        // return value;
    }

    public void Uproot()
    {
        if (_currentVeg == null)
        {
            Debug.Log("Tried to uproot but no veg present");
            return;
        }
        Destroy(_currentVeg.gameObject);
        _veg = null;
    }

    public void AddToWateringLevel(float incrememnt)
    {
        _wateringLevel+= incrememnt;
    }

    public void TryPlay(float pitch)
    {
        // TODO remove - just doing this to visualize when it's trying to play audio
        GetComponent<Animation>().Play();

        if (_veg == null)
        {
            return;
        }

        _source.clip = _veg.Clip;
        _source.pitch = pitch;
        _source.Play();
    }

    public void SetHighlight(bool isHighlighted)
    {
        _activeMesh.material.color = isHighlighted ? _highlightColor : _originalColor;
    }
}
