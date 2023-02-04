using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SO_WorldSettings _worldSettings;
    [SerializeField] private Transform _worldParent;
    [SerializeField] private AudioLine _audioLine;

    private Pot[,] _grid;
    private List<Vector2Int> _activePotPositions = new List<Vector2Int>();


    // Start is called before the first frame update
    void Start()
    {
        CreateNewWorld();
    }

    private void CreateNewWorld()
    {
        SetActivePotPositions();
        GeneratePots();
        SetAudioLine();
    }

    private void SetActivePotPositions()
    {
        var start = _worldSettings.StartPos;
        for (var x = 0; x < _worldSettings.StartWidth; x++)
        {
            for (var y=0; y < _worldSettings.StartHeight; y++)
            {
                var activePos = new Vector2Int(start.x + x, start.y + y);
                if (!_activePotPositions.Contains(activePos))
                {
                    _activePotPositions.Add(activePos);
                }
            }
        }
    }

    private void GeneratePots()
    {
        var height = _worldSettings.MaxHeight;
        var width = _worldSettings.MaxWidth;
        _grid = new Pot[width, height];

        var prefab = _worldSettings.PotPrefab;
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var pot = Instantiate(prefab, _worldParent).GetComponent<Pot>();
                var pos = new Vector2Int(x, y);
                pot.transform.localPosition = new Vector3(pos.x, 0, pos.y);
                pot.Set(active: _activePotPositions.Contains(pos));
                _grid[x,y] = pot; 
            }
        }
    }

    private void SetAudioLine()
    {
        _audioLine.Set(-1f, _worldSettings.StartWidth + 1f);
        _audioLine.OnNewPos += OnAudioHit;
        _audioLine.Play();
    }

    private void OnAudioHit(int x)
    {
        Debug.Log("On Audio Hit: " + x);
        foreach (var pot in _activePotPositions)
        {            
            if (pot.x == x)
            {
                var pitch = GetPitch(pot);
                _grid[pot.x, pot.y].TryPlay(pitch);
            }
        }
    }

    public float GetPitch(Vector2Int pos)
    {
        // TODO figure out level
        return 1f;
    }
}