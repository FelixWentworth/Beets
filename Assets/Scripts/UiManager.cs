using System;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [SerializeField] private Transform _inventoryParent;
    [SerializeField] private InventoryBtn _inventoryBtnPrefab;
    [SerializeField] private CanvasGroup _inventoryCg;
    [SerializeField] private TextMeshProUGUI _moneyText;
    private Inventory _inventory;
    private GameManager _gameManager;

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        InventoryBtn.OnSelected += OnSelected;
        InventoryBtn.OnDragEnd += OnDragEnd;
        GameManager.OnMoneyChanged += OnMoneyChanged;
    }
    
    private void Start()
    {
        FirstLoad();
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
    }

    private void OnDragEnd()
    {
        
        SetInventoryActive(true);
    }

    private void OnSelected(SO_Veg obj)
    {
        SetInventoryActive(false);
    }
    
    void FirstLoad()
    {
        foreach (var vegConfig in _gameManager.Veg)
        {
            var newBtn = Instantiate(_inventoryBtnPrefab, _inventoryParent);
            newBtn.Init(vegConfig);
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
    public string Item;
    public int Quantity;
}
