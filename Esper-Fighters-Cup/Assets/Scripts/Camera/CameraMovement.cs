using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private List<Transform> _movementTargets;
 
    [SerializeField]
    private Vector3 _offset;
    [SerializeField]
    private float _smoothTime = 0.1f;
 
    [SerializeField]
    private float _minZoom = 70f;
    [SerializeField]
    private float _maxZoom = 20f;
    [SerializeField]
    private float _zoomLimiter = 50f;
 
    private Vector3 _velocityVector3;
    private float _velocityFloat;
    private Bounds _totalBounds;

    private void Reset()
    {
        _camera = GetComponent<Camera>();
    }
 
    private void Update()
    {
        if (_movementTargets.Count == 0) return;
        CalcAllBounds();
        Move();
        Zoom();
    }
 
    private void Zoom()
    {
        var newZoom = Mathf.Lerp(_maxZoom, _minZoom, Mathf.Max(_totalBounds.size.x, _totalBounds.size.z * 1.5f) / _zoomLimiter);
        _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, newZoom, Time.deltaTime);
        _camera.fieldOfView = Mathf.SmoothDamp(_camera.fieldOfView, newZoom, ref _velocityFloat, _smoothTime);
    }
 
 
    private void Move()
    {
        var centerPoint = _totalBounds.center;
        var newPosition = centerPoint + _offset;
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref _velocityVector3, _smoothTime);
    }

    private void CalcAllBounds()
    {
        _totalBounds = new Bounds(_movementTargets[0].position, Vector3.zero);
        foreach (var target in _movementTargets)
        {
            _totalBounds.Encapsulate(target.position);
        }
    }
}
