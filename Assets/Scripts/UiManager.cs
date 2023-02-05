using System;
using System.Collections.Generic;
using System.Linq;
using Pixelplacement;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static Action<bool> OnShopToggled;
    [SerializeField] private Transform _inventoryParent;
    [SerializeField] private Transform _shopParent;
    [SerializeField] private InventoryBtn _inventoryBtnPrefab;
    [SerializeField] private ShopItemBtn _shopBtnPrefab;
    [SerializeField] private CanvasGroup _inventoryCg;
    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private Transform _shopRect;
    [SerializeField] private float _shopSlideOffset;
    private Dictionary<string, int> _inventory = new Dictionary<string, int>();
    private GameManager _gameManager;
    private bool _isShopOpen;
    
    private void Awake()
    {
        
        _gameManager = FindObjectOfType<GameManager>();
        InventoryBtn.OnSelected += OnSelected;
        InventoryBtn.OnDragEnd += OnDragEnd;
        GameManager.OnMoneyChanged += OnMoneyChanged;
        VegPlacer.OnVegPlaced += OnPotInteracted;
        GameManager.OnVegBought += OnSeedBought;
    }

    private void OnSeedBought(SO_Veg soVeg)
    {
        _inventory[soVeg.Name]++;
    }

    private void OnPotInteracted(string vegName)
    {
        _inventory[vegName]--;
    }

    private void Start()
    {
        FirstLoad();
        UpdateInventory();
        OnMoneyChanged(0);
    }
    
    private void OnMoneyChanged(int obj)
    {
        _moneyText.text = $"{obj:N0}";
    }

    private void OnDestroy()
    {
        InventoryBtn.OnSelected -= OnSelected;
        InventoryBtn.OnDragEnd -= OnDragEnd;
        GameManager.OnMoneyChanged -= OnMoneyChanged;
        VegPlacer.OnVegPlaced -= OnPotInteracted;
        ShopItemBtn.OnShopItemPressed -= OnSeedBought;
    }

    private void OnDragEnd()
    {
        SetInventoryActive(true);
    }

    public void ToggleShop()
    {
        _isShopOpen = !_isShopOpen;
        OnShopToggled?.Invoke(_isShopOpen);
        var targetPosOffset = _isShopOpen ? Vector3.up * _shopSlideOffset : Vector3.up * -1f * _shopSlideOffset;
        Tween.Position(_shopRect, _shopRect.position + targetPosOffset, 0.3f, 0f, Tween.EaseOutBack);
        SetInventoryActive(!_isShopOpen);
    }

    private void OnSelected(SO_Veg obj)
    {
        SetInventoryActive(false);
    }
    
    void FirstLoad()
    {
        
        foreach (var vegConfig in _gameManager.Veg)
        {
            _inventory[vegConfig.Name] = 1;
        }
    }

    void UpdateInventory()
    {
        foreach (var inventoryKey in _inventory.Keys)
        {
            var vegConfig = _gameManager.Veg.FirstOrDefault(v => v.Name == inventoryKey);
            var newBtn = Instantiate(_inventoryBtnPrefab, _inventoryParent);
            var newShopBtn = Instantiate(_shopBtnPrefab, _shopParent);
            newShopBtn.Set(vegConfig, _gameManager);
            newBtn.Init(vegConfig, _inventory[inventoryKey]);
        }
    }

    void SetInventoryActive(bool isActive)
    {
        _inventoryCg.interactable = isActive;
        _inventoryCg.alpha = isActive ? 1f : 0.2f;
    }

}

[Serializable]
public class Inventory
{
    
    public string VegName;
    public int Quantity;
}
