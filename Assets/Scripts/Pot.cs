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
    [SerializeField] private Material _highlightMaterial;
    [SerializeField] private Material _normalMaterial;

    [SerializeField] private AudioSource _source;
    [SerializeField] private float _startingVegScaleMultiplier;

    [Header("Unlock")]
    [SerializeField] private GameObject _unlockState;
    [SerializeField] private GameObject _sign;
    [SerializeField] private TMPro.TextMeshPro _cost;

    private GameObject _currentVeg;
    private SO_Veg _veg;
    private float _wateringLevel = 0f;
    private Vector2Int _gridPos;
    private bool _active;
    private Vector3 _finalScale;

    private float _growthSecondsElapsed = 0f;

    private bool _showSign = false;

    private void Start()
    {
        _sign.gameObject.SetActive(false);
    }

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
        _unlockState.SetActive(false);

        
        if (vegToSpawn != null)
        {
            _veg = vegToSpawn;
            _currentVeg = Instantiate(vegToSpawn.Prefab, _vegSpawnPoint);
            _growthSecondsElapsed = 0f;
            var modelTransform = _currentVeg.transform.GetChild(0); 
            _finalScale = modelTransform.localScale;
            modelTransform.localScale = Vector3.one * _startingVegScaleMultiplier;
            _wateringLevel = 0f;
        }
    }

    public void SetUnlockVisuals(Material mat, bool showSign = false, string signText = "")
    {
        _unlockState.GetComponent<MeshRenderer>().material = mat;
        _showSign = showSign;
        _cost.text = signText;
    }

    private void OnShopToggle(bool active)
    {
        if (_activeState.activeSelf)
        {
            return;
        }
        _unlockState.SetActive(active);
        _sign.SetActive(_showSign);
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
        int growthStagesPassed =  (int)(_growthSecondsElapsed / timePerStage);
        int reward = 0;
        if (growthStagesPassed < _veg.LifeStages.Length)
        {
            reward = _veg.LifeStages[growthStagesPassed].HarvestValue;
        }
        else
        {
            reward =_veg.LifeStages.Last().HarvestValue;
        }
        return reward; 
        

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

    public void TryPlay(int pitch)
    {
        if (!HasVeg)
        {
            return;
        }

        // TODO remove - just doing this to visualize when it's trying to play audio
        GetComponent<Animation>().Play();

        _source.clip = _veg.Clip(pitch);
        Debug.Log("playing clip: " + _source.clip);
        _source.Play();
    }

    public void SetHighlight(bool isHighlighted)
    {
        _activeMesh.material = isHighlighted ? _highlightMaterial : _normalMaterial;
        
    }
}
