using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MoneyChangeDisplay : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _holdTime = 0f;
    [SerializeField] private float _fadeOutTime;

    private bool _visible => _canvasGroup.alpha > 0.1f;
    private int _lastCachedMoney;
    private float _fadeOutElapsed;

    private void Awake()
    {
        _lastCachedMoney = 0;
        _canvasGroup.alpha = 0f;
    }
    void Start()
    {
        GameManager.OnMoneyChanged += ShowMoneyChange;
    }

    private void ShowMoneyChange(int newAmount)
    {
        var diff = newAmount - _lastCachedMoney;
        var str = (diff >= 0 ? "+" : "") + diff;
        _text.SetText(str);
        if (!_visible)
        {
            StartCoroutine(FadeOut(newAmount));
        }
        else
        {
            _fadeOutElapsed = 0f;
        }

    }

    private IEnumerator FadeOut(int newAmount)
    {
        _canvasGroup.alpha = 1f;
        _fadeOutElapsed = 0f;
        while (_fadeOutElapsed <= _fadeOutTime)
        {
            _fadeOutElapsed += Time.deltaTime;
            if (_fadeOutElapsed > _holdTime) {
                var a = Mathf.Lerp(1f, 0f, (_fadeOutElapsed - _holdTime) / (_fadeOutTime - _holdTime));
                _canvasGroup.alpha = a;
            }
            else
            {
                _canvasGroup.alpha = 1f;
            }
            yield return null;

        }
        _canvasGroup.alpha = 0f;
        _lastCachedMoney = newAmount;
    }
}
