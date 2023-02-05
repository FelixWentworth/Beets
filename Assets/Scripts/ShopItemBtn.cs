using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemBtn : MonoBehaviour
{
    public static Action<SO_Veg> OnShopItemPressed;
    [SerializeField] private TextMeshProUGUI _price;
    [SerializeField] private Image _img;
    private GameManager _man;
    private SO_Veg _veg;

    public void Set(SO_Veg veg, GameManager manager)
    {
        _veg = veg;
        _man = manager;
        _img.sprite = veg.Icon;
        _price.text = $"${veg.SeedPrice:N0}";
    }

    public void OnShopBtnPressed()
    {
        
        if (_man.Money >= _veg.SeedPrice)
        {
            OnShopItemPressed?.Invoke(_veg);
        }
    }
}
