using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemBtn : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _price;
    [SerializeField] private Image _img;

    public void Set(SO_Veg veg)
    {
        _img.sprite = veg.Icon;
        _price.text = $"${veg.SeedPrice:N0}";
    }
}
