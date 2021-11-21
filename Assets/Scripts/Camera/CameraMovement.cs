using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour
{
    public int cameraViewState = 1;

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

    private Bounds _totalBounds;
    private float _velocityFloat;

    private Vector3 _velocityVector3;

    private void Reset()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (_movementTargets.Count == 0)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if(cameraViewState==1) { cameraViewState = 2; }
            else { cameraViewState = 1; }
        }


        CalcAllBounds();
        Move();
        Zoom();
        CameraViewChanger();

    }

    private void Zoom()
    {
        var newZoom = Mathf.Lerp(_maxZoom, _minZoom,
            Mathf.Max(_totalBounds.size.x, _totalBounds.size.z * 1.5f) / _zoomLimiter);
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

    public void AddTarget(Transform target)
    {
        _movementTargets.Add(target);
    }

    public void RemoveTarget(Transform target)
    {
        _movementTargets.Remove(target);
    }

    private void CameraViewChanger()
    {
        if(cameraViewState==1)
        {
            transform.rotation = Quaternion.Euler(new Vector3(60, 0, 0));
            _offset.z = -11;
        }
        else
        {
            transform.rotation = Quaternion.Euler(new Vector3(45, 0, 0));
            _offset.z = -18;
        }
    }

}
