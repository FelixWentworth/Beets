using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryBtn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static Action OnDragEnd;
    public static Action<SO_Veg> OnSelected;
    [SerializeField] private Image _icon;
    private SO_Veg _veg;
    
    public void Init(SO_Veg veg)
    {
        _veg = veg;
        _icon.sprite = veg.Icon;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnSelected?.Invoke(_veg);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnDragEnd?.Invoke();
    }
}
