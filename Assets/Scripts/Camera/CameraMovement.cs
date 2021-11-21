using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour
{
    private int cameraViewState = 1;

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
        if (cameraViewState == 1) { cameraViewState = 2; Debug.Log("ViewState Changed to [" + cameraViewState + "]"); }
        else if (cameraViewState == 2) { cameraViewState = 1; Debug.Log("ViewState Changed to [" + cameraViewState + "]"); }
        CameraViewChanger(cameraViewState);
    }

    private void CameraViewChanger(int ViewState)
    {
        if(ViewState==1)
        {
            transform.rotation = Quaternion.Euler(new Vector3(60, 0, 0));
            _offset.z = -11;
        }
        else if(ViewState==2)
        {
            transform.rotation = Quaternion.Euler(new Vector3(45, 0, 0));
            _offset.z = -18;
        }
    }

    public void CameraViewStateFirstInput(int InputedViewState)
    {
        Debug.Log("Fitst ViewState Inputer Loaded. ViewState [" + InputedViewState + "] Inputed");
        if (InputedViewState == 1)
        {
            cameraViewState = InputedViewState;
            transform.rotation = Quaternion.Euler(new Vector3(60, 0, 0));
            _offset.z = -11;
        }
        else if(InputedViewState==2)
        {
            cameraViewState = InputedViewState;
            transform.rotation = Quaternion.Euler(new Vector3(45, 0, 0));
            _offset.z = -18;
        }
    }

}
