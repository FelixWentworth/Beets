using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameAction
    {
        Plant,
        Water,
        Harvest
    }
    public static Action<GameAction, Vector2Int, string> InteractWithPot { get; private set; }
    public static Action<int> OnMoneyChanged;
    public static Action<int> OnColumnAudioHit;
    public static Action<SO_Veg> OnVegBought;

    [HideInInspector] public int Money;

    [SerializeField] private SO_WorldSettings _worldSettings;
    [SerializeField] private Transform _worldParent;
    [SerializeField] private AudioLine _audioLine;

    private Pot[,] _grid;
    private List<Vector2Int> _activePotPositions = new List<Vector2Int>();

    public SO_Veg[] Veg;

    public int BPM;

    private void Awake()
    {
        ShopItemBtn.OnShopItemPressed += OnSeedBought;
    }

    private void OnSeedBought(SO_Veg veg)
    {
        if (Money >= veg.SeedPrice)
        {
            Money -= veg.SeedPrice;
            OnMoneyChanged?.Invoke(Money);
            OnVegBought?.Invoke(veg);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InteractWithPot = HandleGameAction;
        CreateNewWorld();
    }

    private void CreateNewWorld()
    {
        SetActivePotPositions();
        GeneratePots();
        SetAudioLine();

        GameManager.InteractWithPot(GameAction.Plant, new Vector2Int(0, 4), "Beet");
    }

    private void HandleGameAction(GameAction action, Vector2Int target, string veg)
    {
        switch (action)
        {
            case GameAction.Plant:
                CreateVeg(target, veg);
                break;
            case GameAction.Water:
                WaterVeg(target);
                break;
            case GameAction.Harvest:
                HarvestVeg(target);
                break;
        }
    }


    private void CreateVeg(Vector2Int pos, string veg)
    {
        if (_grid[pos.x, pos.y].HasVeg)
        {
            Debug.Log("Cannot plant on top of veg");
            return;
        }
        var vegToPlant = Veg.FirstOrDefault(v => v.Name == veg);
        if (vegToPlant == null)
        {
            Debug.Log("Unable to find veg: " + veg);
            return;
        }

        _grid[pos.x, pos.y].Set(new Vector2Int(pos.x, pos.y), vegToSpawn: vegToPlant);
    }

    private void HarvestVeg(Vector2Int pos)
    {
        var pot = _grid[pos.x, pos.y];
        if (!pot.HasVeg)
        {
            Debug.Log("Cannot harvest an empty pot");
            return;
        }
        var value = pot.GetHarvestValue();
        var accuracy = GetInputAccuracy();
        Money += Mathf.RoundToInt(value * accuracy);
        OnMoneyChanged?.Invoke(Money);
        pot.Uproot();
    }

    private void WaterVeg(Vector2Int pos)
    {
        var pot = _grid[pos.x, pos.y];
        if (!pot.HasVeg)
        {
            Debug.Log("Cannot Water an empty pot");
            return;
        }

        var accuracy = GetInputAccuracy();
        pot.AddToWateringLevel(accuracy);
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
                pot.Set(new Vector2Int(x,y), active: _activePotPositions.Contains(pos));
                _grid[x,y] = pot; 
            }
        }
    }

    private void SetAudioLine()
    {
        _audioLine.Set(-0.5f, _worldSettings.StartWidth - 0.5f, BPM);
        _audioLine.OnNewPos += OnAudioHit;
        _audioLine.Play();
    }

    private void OnAudioHit(int x)
    {
        // Debug.Log("On Audio Hit: " + x);
        foreach (var pot in _activePotPositions)
        {            
            if (pot.x == x)
            {
                var pitch = GetPitch(pot);
                _grid[pot.x, pot.y].TryPlay(pitch);
            }
        }
        OnColumnAudioHit?.Invoke(x);
    }

    public int GetPitch(Vector2Int pos)
    {
        return pos.y;
    }

    public float GetInputAccuracy()
    {
        return 1f;
    }
}
