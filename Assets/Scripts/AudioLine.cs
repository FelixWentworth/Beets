using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLine : MonoBehaviour
{
    public Action<int> OnNewPos;

    [SerializeField] private float Speed;
    private float _speed;
    private int _lastPos;

    private float _startXPosition;
    private float _endXPosition;

    private void Start()
    {
        _speed = 0f;
        _lastPos = -1;
        StartCoroutine(Move());
    }

    public void Set(float startX, float endX)
    {
        _startXPosition = startX;
        _endXPosition = endX;
    }

    public void Play()
    {
        _speed = Speed;
    }

    public void Pause()
    {
        _speed = 0f;
    }

    private IEnumerator Move()
    {
        while (true)
        {
            // TODO smooth this
            transform.position += new Vector3(_speed * Time.deltaTime, 0f, 0f);
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
