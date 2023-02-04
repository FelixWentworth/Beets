using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : MonoBehaviour
{
    public bool HasVeg => _veg != null;

    [SerializeField] private Transform _vegSpawnPoint;
    [SerializeField] private GameObject _activeState;
    [SerializeField] private GameObject _inactiveState;
    [SerializeField] private MeshRenderer _activeMesh;
    [SerializeField] private Color _highlightColor;
    [SerializeField] private Color _originalColor;

    [SerializeField] private AudioSource _source;

    private GameObject _currentVeg;
    private SO_Veg _veg;
    private float _wateringLevel = 0f;

    public void Set(bool active = true, SO_Veg vegToSpawn = null)
    {
        _activeState.SetActive(active);
        _inactiveState.SetActive(!active);


        if (vegToSpawn != null)
        {
            _veg = vegToSpawn;
            _currentVeg = Instantiate(vegToSpawn.Prefab, _vegSpawnPoint);
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
        foreach(var stage in _veg.LifeStages)
        {
            if (_wateringLevel >= stage.WateringScoreToUnlock)
            {
                value = stage.HarvestValue;
            }
        }
        return value;
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
