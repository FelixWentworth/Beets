using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : MonoBehaviour
{
    public bool HasVeg => _veg != null;

    [SerializeField] private Transform _vegSpawnPoint;
    [SerializeField] private GameObject _activeState;
    [SerializeField] private GameObject _inactiveState;

    [SerializeField] private AudioSource _source;

    private SO_Veg _veg;

    public void Set(bool active = true, SO_Veg vegToSpawn = null)
    {
        _activeState.SetActive(active);
        _inactiveState.SetActive(!active);


        if (vegToSpawn != null)
        {
            _veg = vegToSpawn;
            Instantiate(vegToSpawn.Prefab, _vegSpawnPoint);
        }
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
}
