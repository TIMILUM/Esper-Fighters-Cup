using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour
{
    private int _cameraViewState = 1;

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
            ViewStateChanger();
        }

        CalcAllBounds();
        Move();
        Zoom();
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


    private void ViewStateChanger()
    {
        if (_cameraViewState == 1) { _cameraViewState = 2; Debug.Log("ViewState Changed to [" + _cameraViewState + "]"); }
        else if (_cameraViewState == 2) { _cameraViewState = 1; Debug.Log("ViewState Changed to [" + _cameraViewState + "]"); }
        CameraViewChanger(_cameraViewState);
    }

    private void CameraViewChanger(int viewState)
    {
        if (viewState == 1)        //60
        {
            transform.rotation = Quaternion.Euler(new Vector3(60, 0, 0));
            _offset.z = -11;
            _minZoom = 80;
            _maxZoom = 20;
        }
        else if (viewState == 2)   //45
        {
            transform.rotation = Quaternion.Euler(new Vector3(45, 0, 0));
            _offset.z = -20;
            _minZoom = 65;
            _maxZoom = 15;
        }
    }

    public void CameraViewStateFirstInput(int inputedViewState)
    {
        Debug.Log("Fitst ViewState Inputer Loaded. ViewState [" + inputedViewState + "] Inputed");
        if (inputedViewState == 1)
        {
            _cameraViewState = inputedViewState;
            transform.rotation = Quaternion.Euler(new Vector3(60, 0, 0));
            _offset.z = -11;
            _minZoom = 80;
            _maxZoom = 20;
        }
        else if (inputedViewState == 2)
        {
            _cameraViewState = inputedViewState;
            transform.rotation = Quaternion.Euler(new Vector3(45, 0, 0));
            _offset.z = -18;
            _minZoom = 65;
            _maxZoom = 15;
        }
    }

}
