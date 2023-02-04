using System;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private Transform _inventoryParent;
    [SerializeField] private InventoryBtn _inventoryBtnPrefab;
    [SerializeField] private CanvasGroup _inventoryCg;
    private Inventory _inventory;
    private GameManager _gameManager;

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        InventoryBtn.OnSelected += OnSelected;
        InventoryBtn.OnDragEnd += OnDragEnd;
    }
    
    private void OnDestroy()
    {
        InventoryBtn.OnSelected -= OnSelected;
        InventoryBtn.OnDragEnd -= OnDragEnd;
    }

    private void OnDragEnd()
    {
        SetInventoryActive(true);
    }

    private void OnSelected(SO_Veg obj)
    {
        SetInventoryActive(false);
    }

    private void Start()
    {
        FirstLoad();
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
