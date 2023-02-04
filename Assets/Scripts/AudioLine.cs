using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLine : MonoBehaviour
{
    public Action<int> OnNewPos;

    [SerializeField]private float _bpm;
    private int _lastPos;

    [SerializeField]private float _startXPosition;
    [SerializeField]private float _endXPosition;

    private bool _isPlaying;

    private void Start()
    {
        _bpm = 0f;
        _lastPos = -1;
        StartCoroutine(Move());
    }

    public void Set(float startX, float endX, float bpm)
    {
        _bpm = bpm;
        _startXPosition = startX;
        _endXPosition = endX;
    }

    public void Play()
    {
        _isPlaying = true;
    }

    public void Pause()
    {
        _isPlaying = false;
        Application.targetFrameRate = 60;
    }

    private IEnumerator Move()
    {
        
        while (true)
        {
            if (_isPlaying)
            {
                transform.position += new Vector3(_bpm/60 * Time.deltaTime, 0f, 0f);
                var pos = GetCurrentPos();
                if (pos != _lastPos)
                {
                    _lastPos = pos;
                    OnNewPos?.Invoke(pos);
                }
                if (transform.position.x > _endXPosition)
                {
                    ResetPosition();
                }
            }
            yield return null;
        }
    }

    private void ResetPosition()
    {
        transform.position = new Vector3(_startXPosition, transform.position.y, transform.position.z);
    }

    private int GetCurrentPos()
    {
        return Mathf.FloorToInt(transform.position.x - 0.5f) + 1;
    }

}
