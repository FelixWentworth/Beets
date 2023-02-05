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
    public static Action OnBeat;
    public static Action<GameAction, Vector2Int, string> InteractWithPot { get; private set; }
    public static Action<int> OnMoneyChanged;
    public static Action<int> OnColumnAudioHit;
    public static Action<SO_Veg> OnVegBought;
    public static Action NextBPM;

    private int _money;
    public int Money
    {
        get => _money;
        set
        {
            _money = value;
            OnMoneyChanged?.Invoke(_money);
        }
    }

    [SerializeField] private SO_WorldSettings _worldSettings;
    [SerializeField] private Transform _worldParent;
    [SerializeField] private AudioLine _audioLine;

    [SerializeField] private Material _expandRowMaterial;
    [SerializeField] private Material _expandColumnMaterial;

    public Vector2Int TopRightPos => _topRightUnlockedPos;
    private Pot[,] _grid;
    private List<Vector2Int> _activePotPositions = new List<Vector2Int>();
    private Vector2Int _topRightUnlockedPos = new Vector2Int(0, 0);

    public SO_Veg[] Veg;

    public static int CurrentBPM;
    public int[] BPMs;
    private int _nextRowCost => _worldSettings.RowCost(_topRightUnlockedPos.y + 1 - _worldSettings.StartHeight);
    private int _nextColumnCost => _worldSettings.RowCost(_topRightUnlockedPos.x + 1 - _worldSettings.StartWidth);

    private int _currentBPMIndex = 0;

    private void Awake()
    {
        ShopItemBtn.OnShopItemPressed += OnSeedBought;
        NextBPM = GoToNextBPM;
    }

    private void OnSeedBought(SO_Veg veg)
    {
        if (Money >= veg.SeedPrice)
        {
            Money -= veg.SeedPrice;
            OnVegBought?.Invoke(veg);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InteractWithPot = HandleGameAction;
        CurrentBPM = BPMs[_currentBPMIndex];
        CreateNewWorld();
    }

    public void GoToNextBPM()
    {
        _currentBPMIndex++;
        _currentBPMIndex %= BPMs.Length;
        CurrentBPM = BPMs[_currentBPMIndex];
        _audioLine.SetBPM(BPMs[_currentBPMIndex]);
    }

    private void CreateNewWorld()
    {
        SetActivePotPositions();
        GeneratePots();
        SetAudioLine();
        SetExpandCosts();

        InteractWithPot(GameAction.Plant, new Vector2Int(0, 0), "Potato");
        //InteractWithPot(GameAction.Plant, new Vector2Int(1, 2), "Potato");
        InteractWithPot(GameAction.Plant, new Vector2Int(2, 0), "Potato");
        InteractWithPot(GameAction.Plant, new Vector2Int(3, 2), "Potato");
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
                    if (activePos.x > _topRightUnlockedPos.x || activePos.y > _topRightUnlockedPos.y)
                    {
                        _topRightUnlockedPos = activePos;
                    }
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

    private void SetExpandCosts()
    {
        
        var rowCoord = _topRightUnlockedPos.y;
        var rowSignSet = false;
        var rows = GetExpansionRow();
        foreach(var coord in rows)
        {
            var pot = _grid[coord.x, coord.y];
            if (!rowSignSet)
            {
                pot.SetUnlockState(_expandRowMaterial, TryUnlockRow, true, $"{_nextRowCost}");
                rowSignSet = true;
            }
            else
            {
                pot.SetUnlockState(_expandRowMaterial, TryUnlockRow);
            }
            pot.Refresh();
        }   

        var columns = GetExpansionColumn();
        var colSignSet = false;
        foreach (var coord in columns)
        {
            var pot = _grid[coord.x, coord.y];
            if (!colSignSet)
            {
                pot.SetUnlockState(_expandColumnMaterial, TryUnlockColumn, true, $"{_nextColumnCost}");
                colSignSet = true;
            }
            else
            {
                pot.SetUnlockState(_expandColumnMaterial, TryUnlockColumn);
            }
            pot.Refresh();
        }
    }

    private List<Vector2Int> GetExpansionRow()
    {
        var list = new List<Vector2Int>();
        var rowCoord = _topRightUnlockedPos.y;
        if (rowCoord < _worldSettings.MaxHeight)
        {
            for (var i = 0; i < _worldSettings.RowsUnlockedPerPurchase; i++)
            {
                rowCoord++;
                if (rowCoord >= _worldSettings.MaxHeight)
                {
                    break;
                }
                // color the row
                for (var x = 0; x <= _topRightUnlockedPos.x; x++)
                {
                    list.Add(new Vector2Int(x, rowCoord));
                }
            }
        }
        return list;
    }

    private List<Vector2Int> GetExpansionColumn()
    {
        var list = new List<Vector2Int>();
        var colCoord = _topRightUnlockedPos.x;
        if (colCoord < _worldSettings.MaxWidth)
        {
            for (var i = 0; i < _worldSettings.ColumnsUnlockedPerPurchase; i++)
            {
                colCoord++;
                if (colCoord >= _worldSettings.MaxWidth)
                {
                    break;
                }
                // color the row
                for (var y = 0; y <= _topRightUnlockedPos.y; y++)
                {
                    list.Add(new Vector2Int(colCoord, y));
                }
            }
        }
        return list;
    }

    private void TryUnlockRow()
    {
        if(Money < _nextRowCost)
        {
            Debug.Log("not enough money");
            return;
        }
        Money -= _nextRowCost;

        var pots = GetExpansionRow();
        foreach (var coord in pots)
        {
            var pot = _grid[coord.x, coord.y];
            pot.Set(coord, active: true);
            if (!_activePotPositions.Contains(coord))
            {
                _activePotPositions.Add(coord);
                if (coord.x > _topRightUnlockedPos.x || coord.y > _topRightUnlockedPos.y)
                {
                    _topRightUnlockedPos = coord;
                }
            }
        }
        SetExpandCosts();
    }

    private void TryUnlockColumn()
    {
        if (Money < _nextColumnCost)
        {
            Debug.Log("not enough money");
            return;
        }

        Money -= _nextColumnCost;

        var pots = GetExpansionColumn();
        foreach (var coord in pots)
        {
            var pot = _grid[coord.x, coord.y];
            pot.Set(coord, active: true);
            if (!_activePotPositions.Contains(coord))
            {
                _activePotPositions.Add(coord);
                if (coord.x > _topRightUnlockedPos.x || coord.y > _topRightUnlockedPos.y)
                {
                    _topRightUnlockedPos = coord;
                }
            }
        }
        _audioLine.Set(-0.5f, _topRightUnlockedPos.x + 0.5f, BPMs[_currentBPMIndex]);
        SetExpandCosts();
    }

    private void SetAudioLine()
    {
        _audioLine.Set(-0.5f, _worldSettings.StartWidth - 0.5f, BPMs[_currentBPMIndex]);
        _audioLine.OnNewPos += OnAudioHit;
        _audioLine.Play();
    }

    private void OnAudioHit(int x)
    {
        OnBeat?.Invoke();
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
