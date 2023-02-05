using System;
using System.Collections.Generic;
using MalbersAnimations.Controller.AI;
using UnityEngine;
using Random = UnityEngine.Random;

public class BirdController : MonoBehaviour
{
    [SerializeField] private GameObject _birdPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private List<Transform> _fenceTargets;
    [SerializeField] private float _birdDurationMin;
    [SerializeField] private float _birdDurationMax;
    [SerializeField] private int _birdSpawnRoundsMin;
    [SerializeField] private int _birdSpawnRoundsMax;
    
    private float _currentBirdOnFenceTargetTime;
    private float _currentBirdOnFenceElapsedTime;
    private MAnimalAIControl currentBirdAI;
    private GameObject currentBirdGO;
    private int _currentBirdFenceIndex = 0;
    [SerializeField] private List<AudioClip> _birdSongVar1;
    [SerializeField] private AudioSource _source;
    [SerializeField] private List<AudioClip> _birdSongVar2;
    private AudioClip _currentBirdClip;
    private bool _currentBirdOnFence = false;
    private int roundsElapsed = 0;
    private int roundsUntilNextBird = 0;


    private void Awake()
    {
        GameManager.OnColumnAudioHit += OnColumnAudioHit;
    }

    private void Start()
    {
        roundsUntilNextBird = Random.Range(_birdSpawnRoundsMin, _birdSpawnRoundsMax);
    }

    private void OnColumnAudioHit(int columnIndex)
    {
        if (columnIndex == _currentBirdFenceIndex)
        {
            PlayBirdSong();
        }
        
        if (currentBirdGO == null && columnIndex == 0)
        {
            roundsElapsed++;
            Debug.Log($"{roundsElapsed}/{roundsUntilNextBird} elapsed");
            if (roundsElapsed >= roundsUntilNextBird)
            {
                
                SpawnBird();
            }
        }
    }

    public void OnBirdArrivedToFence()
    {
        _currentBirdOnFence = true;
    }

    [ContextMenu("spawn")]
    void SpawnBird()
    {
        currentBirdGO = Instantiate(_birdPrefab, _spawnPoint);
        currentBirdAI = currentBirdGO.GetComponentInChildren<MAnimalAIControl>();
        currentBirdAI.SetTarget(GetNewTarget());
        _currentBirdOnFence = false;
        _currentBirdOnFenceElapsedTime = 0f;
        _currentBirdOnFenceTargetTime = Random.Range(_birdDurationMin, _birdDurationMax);
        var roll = Random.Range(0, 2);
        var list = roll == 0 ? _birdSongVar1 : _birdSongVar2;
        _currentBirdClip = list[Random.Range(0, list.Count)];
    }

    void PlayBirdSong()
    {
        if (_currentBirdOnFence)
        {
            _source.PlayOneShot(_currentBirdClip);
        }
    }


    private void Update()
    {
        if (_currentBirdOnFence)
        {
            _currentBirdOnFenceElapsedTime += Time.deltaTime;
            if (_currentBirdOnFenceElapsedTime > _currentBirdOnFenceTargetTime)
            {
                TriggerBirdLeave();
            }
        }

        
        if (Input.GetKeyDown(KeyCode.G))
        {
            SpawnBird();
        }
        
        if (Input.GetKeyDown(KeyCode.H))
        {
            TriggerBirdLeave();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            PlayBirdSong();
        }
    }

    public void OnBirdArrivedAtExit()
    {
        Destroy(currentBirdGO);
        currentBirdAI = null;
        currentBirdGO = null;
        roundsUntilNextBird = Random.Range(_birdSpawnRoundsMin, _birdSpawnRoundsMax + 1);
        roundsElapsed = 0;
    }

    [ContextMenu("leave")]
    void TriggerBirdLeave()
    {
        _currentBirdOnFence = false;
        currentBirdAI.WaitTimeMult = 0.001f;
        currentBirdAI.MovetoNextTarget();
    }

    Transform GetNewTarget()
    {
        _currentBirdFenceIndex = Random.Range(0, _fenceTargets.Count); 
        return _fenceTargets[_currentBirdFenceIndex];
    }
}
