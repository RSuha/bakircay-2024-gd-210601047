using System;
using Common;
using UnityEngine;

public class DetectObject : MonoBehaviour
{
    private Camera _mainCamera;
    private Rigidbody _rigidbodyToJump = null;
    private Vector3 _totalForce = Vector3.zero;
    private float _forceFactor = 0.1f;

    void Start()
    {
        _mainCamera = Camera.main;

        TouchManager.Instance.OnTouchBegan += OnTouchBegan;
        TouchManager.Instance.OnTouchMoved += OnTouchMoved;
        TouchManager.Instance.OnTouchEnded += OnTouchEnded;
    }

    private void OnDestroy()
    {
        if (TouchManager.Instance == null)
            return;

        TouchManager.Instance.OnTouchBegan -= OnTouchBegan;
        TouchManager.Instance.OnTouchMoved -= OnTouchMoved;
        TouchManager.Instance.OnTouchEnded -= OnTouchEnded;
    }

    private void OnTouchBegan(TouchData data)
    {
        CastRay(data.position);
    }

    private void OnTouchMoved(TouchData data)
    {
        if (_rigidbodyToJump != null)
        {
            _totalForce += new Vector3(data.deltaPosition.x, 0.1f, data.deltaPosition.y);
        }
    }

    private void OnTouchEnded(TouchData data)
    {
        _rigidbodyToJump = null;
    }

    private void CastRay(Vector2 screenPosition)
    {
        Ray ray = _mainCamera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.rigidbody != null)
            {
                _rigidbodyToJump = hit.rigidbody;
                _totalForce = Vector3.zero; // Kuvveti sýfýrla
            }
        }
    }

    private void FixedUpdate()
    {
        if (_rigidbodyToJump != null)
        {
            _rigidbodyToJump.AddForce(_totalForce * _forceFactor, ForceMode.Impulse);
            _totalForce = Vector3.zero;
        }
    }
}
