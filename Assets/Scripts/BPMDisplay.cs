using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BPMDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _pulse;
    [SerializeField] private Vector3 _scaleTo;
    [SerializeField] private Button _btn;
    [SerializeField] private TextMeshProUGUI _text;

    private void Start()
    {
        _btn.onClick.AddListener(NextBPM);
        GameManager.OnBeat += Pulse;
    }

    private void Update()
    {
        _text.text = GameManager.CurrentBPM.ToString();
        if(_pulse.transform.localScale != Vector3.one)
        {
            _pulse.transform.localScale = Vector3.MoveTowards(_pulse.transform.localScale, Vector3.one, 2* Time.deltaTime);
        }
    }

    private void Pulse()
    {
        _pulse.transform.localScale = _scaleTo;
    }

    private void NextBPM()
    {
        GameManager.NextBPM();
    }
}
