using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryBtn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static Action OnDragEnd;
    public static Action<SO_Veg> OnSelected;
    
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _quantityText;
    [SerializeField] private CanvasGroup _cg;
    bool hasAny => _quantity > 0;
    private SO_Veg _veg;
    private int _quantity;
    
    public void Init(SO_Veg veg, int startingQuantity)
    {
        VegPlacer.OnVegPlaced += OnVegPlaced;
        _quantity = startingQuantity;
        _veg = veg;
        _icon.sprite = veg.Icon;
        UpdateState();
    }

    private void OnDestroy()
    {
        VegPlacer.OnVegPlaced -= OnVegPlaced;
    }

    private void OnVegPlaced(string vegName)
    {
        if (_veg.Name == vegName)
        {
            _quantity--;
            UpdateState();
        }
    }

    void UpdateState()
    {
        _cg.alpha = hasAny ? 1f : 0.2f;
        _cg.interactable = hasAny;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (hasAny)
        {
            OnSelected?.Invoke(_veg);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (hasAny)
        {
            OnDragEnd?.Invoke();
        }
    }
    
    
}
