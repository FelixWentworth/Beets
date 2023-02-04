using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryBtn : MonoBehaviour
{
    public static Action<SO_Veg> OnSelected;
    [SerializeField] private Image _icon;
    private SO_Veg _veg;
    
    public void Init(SO_Veg veg)
    {
        _veg = veg;
        _icon.sprite = veg.Icon;
    }

    public void OnBtnPressed()
    {
        OnSelected?.Invoke(_veg);
    }
}
