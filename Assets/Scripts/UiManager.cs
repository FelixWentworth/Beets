using System;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    private GameManager _gameManager;
    [SerializeField] private Transform _inventoryParent;
    [SerializeField] private InventoryBtn _inventoryBtnPrefab;
    
    private Inventory _inventory;

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        
        FirstLoad();
    }
    
    void FirstLoad()
    {
        InventoryBtn.OnSelected += OnVegSelected;
        foreach (var vegConfig in _gameManager.Veg)
        {
            var newBtn = Instantiate(_inventoryBtnPrefab, _inventoryParent);
            newBtn.Init(vegConfig);
        }
    }

    void OnVegSelected(SO_Veg veg)
    {
        
    }
}

[Serializable]
public class Inventory
{
    public string Item;
    public int Quantity;
}
