using System;
using Entities;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private Transform _cameraTransform = null;

    [SerializeField]
    private float _movementSpeed = 5f;
    [SerializeField]
    private float _movementTime = 5f;
    [SerializeField]
    private float _rotationAmount = 5f;
    [SerializeField]
    private float _rotationMouseAmount = 5f;
    [SerializeField]
    private Vector3 _zoomAmount = new Vector3(0, -10, 10);
    [SerializeField]
    private Vector3 _minZoom = new Vector3(0, 15, -15);
    [SerializeField]
    private Vector3 _maxZoom = new Vector3(0, 600, -600);
    [SerializeField]
    private float _minViewAngle = -45f;
    [SerializeField]
    private float _maxViewAngle = 45f;

    [SerializeField]
    private PathfindGrid _grid = null;

    [SerializeField]
    private PostProcessVolume _postFX = null;

    [SerializeField]
    private Transform _energyBar = null;


    private Vector3 _newPosition;
    private Quaternion _newRotation;
    private Vector3 _newZoom;

    private Camera _camera;
    private Vector3 _dragStartPosition;
    private Vector3 _dragCurrentPosition;

    private Vector3 _rotateStartPosition;
    private Vector3 _rotateCurrentPosition;


    private Transform _boundAnimalTransform;
    private bool _animalIsFollowed;

    private DepthOfField _depthOfField = null;

    private readonly float _minBarScale = 0.25f;
    private readonly float _maxBarScale = 4f;
    private float _energyBarScaleFactor = 1f;


    private void Awake()
    {
        ClickSelectController.OnSelectedAnimalChanged += BindSelectedAnimal;
        BindSelectedAnimal(ClickSelectController.SelectedAnimal);

        _postFX.profile.TryGetSettings(out _depthOfField);
    }

    private void OnDestroy()
    {
        ClickSelectController.OnSelectedAnimalChanged -= BindSelectedAnimal;
    }

    private void BindSelectedAnimal(Animal animal)
    {
        if (animal != null)
            _boundAnimalTransform = animal.transform;
        else
            _boundAnimalTransform = null;
    }


    private void Start()
    {
        _newPosition = transform.position;
        _newRotation = transform.rotation;
        _newZoom = _cameraTransform.localPosition;

        _camera = _cameraTransform.GetComponent<Camera>();
    }

    private void Update()
    {
        HandleMouseInput();
        HandleKeyboardInput();
        HandleMovement();
    }


    private void HandleMouseInput()
    {
        MouseZoom();
        MouseDragPan();
        MouseDragRotation();
    }

    private void MouseZoom()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            _newZoom += Input.mouseScrollDelta.y * _zoomAmount;

            float distToMin = Vector3.Distance(_minZoom, _newZoom);
            float distToMax = Vector3.Distance(_maxZoom, _newZoom);
            float distMinToMax = Vector3.Distance(_minZoom, _maxZoom);

            if (distToMin > distMinToMax)
                _newZoom = _maxZoom;

            if (distToMax > distMinToMax)
                _newZoom = _minZoom;

            float normalizedZoom = Mathf.InverseLerp(0, distMinToMax, distToMin);
            _depthOfField.focalLength.value = 70f * (1f - normalizedZoom);

            SetEnergyBarScale(normalizedZoom);
        }
    }

    private void SetEnergyBarScale(float normalZoom)
    {
        float normalZoomThreshold = 0.15f;
        if (normalZoom > normalZoomThreshold)
        {
            _energyBarScaleFactor = (1f - _minBarScale) / (1f - normalZoomThreshold) * (1f - normalZoom) + _minBarScale;
        }
        else
        {
            _energyBarScaleFactor = (_maxBarScale - 1f) / normalZoomThreshold * (normalZoomThreshold - normalZoom) + 1f;
        }
    }

    private void MouseDragPan()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                _dragStartPosition = ray.GetPoint(entry);
            }
        }

        if (Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                _dragCurrentPosition = ray.GetPoint(entry);

                _newPosition = transform.position + _dragStartPosition - _dragCurrentPosition;

                ClampNewPosition();
            }
        }
    }

    private void ClampNewPosition()
    {
        if (_newPosition.x > 0.5f * _grid.GridWorldSize.x)
            _newPosition += Vector3.right * (0.5f * _grid.GridWorldSize.x - _newPosition.x);

        if (_newPosition.x < -0.5f * _grid.GridWorldSize.x)
            _newPosition += Vector3.right * (-0.5f * _grid.GridWorldSize.x - _newPosition.x);

        if (_newPosition.z > 0.5f * _grid.GridWorldSize.y)
            _newPosition += Vector3.forward * (0.5f * _grid.GridWorldSize.y - _newPosition.z);

        if (_newPosition.z < -0.5f * _grid.GridWorldSize.y)
            _newPosition += Vector3.forward * (-0.5f * _grid.GridWorldSize.y - _newPosition.z);
    }

    private void MouseDragRotation()
    {
        if (Input.GetMouseButtonDown(2))
        {
            _rotateStartPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(2))
        {
            _rotateCurrentPosition = Input.mousePosition;

            Vector3 difference = _rotateStartPosition - _rotateCurrentPosition;
            _rotateStartPosition = _rotateCurrentPosition;

            // order of rotations is important here (global vs local rotations)
            // so we don't use *= operator here
            if (Math.Abs(difference.x) >= Math.Abs(difference.y))
                _newRotation = Quaternion.Euler(Vector3.up * (-difference.x / _rotationMouseAmount)) * _newRotation;
            
            if (Math.Abs(difference.y) > Math.Abs(difference.x))
            {
                ClampNewVerticalRotation(difference);
            }
        }
    }

    private void ClampNewVerticalRotation(Vector3 difference)
    {
        Quaternion addedRotation = Quaternion.Euler(Vector3.right * (-difference.y / _rotationMouseAmount));
        Quaternion nextRot = _newRotation * addedRotation;

        float xAngleCorrected = (nextRot.eulerAngles.x > 180) ? nextRot.eulerAngles.x - 360 : nextRot.eulerAngles.x;
        if (xAngleCorrected > _minViewAngle && xAngleCorrected < _maxViewAngle)
            _newRotation *= addedRotation;
    }

    private void HandleKeyboardInput()
    {
        KeyboardPan();
        KeyboardRotation();
        KeyboardZoom();
    }

    private void KeyboardPan()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            _newPosition += transform.forward * _movementSpeed;

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            _newPosition += -transform.forward * _movementSpeed;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            _newPosition += transform.right * _movementSpeed;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            _newPosition += -transform.right * _movementSpeed;
    }

    private void KeyboardRotation()
    {
        if (Input.GetKey(KeyCode.Q))
            _newRotation *= Quaternion.Euler(Vector3.up * _rotationAmount);

        if (Input.GetKey(KeyCode.E))
            _newRotation *= Quaternion.Euler(-Vector3.up * _rotationAmount);
    }

    private void KeyboardZoom()
    {
        if (Input.GetKey(KeyCode.R))
            ChangeKeyboardNewZoom(_zoomAmount);

        if (Input.GetKey(KeyCode.F))
            ChangeKeyboardNewZoom(-_zoomAmount);
    }

    private void ChangeKeyboardNewZoom(Vector3 zoomAmount)
    {
        _newZoom += zoomAmount;

        float distToMin = Vector3.Distance(_minZoom, _newZoom);
        float distToMax = Vector3.Distance(_maxZoom, _newZoom);
        float distMinToMax = Vector3.Distance(_minZoom, _maxZoom);

        if (distToMin > distMinToMax)
            _newZoom = _maxZoom;

        if (distToMax > distMinToMax)
            _newZoom = _minZoom;
    }

    private void HandleMovement()
    {
        CheckIfFocusAnimal();
        if (_boundAnimalTransform != null && _animalIsFollowed)
            _newPosition = _boundAnimalTransform.position;

        transform.position = Vector3.Lerp(transform.position, _newPosition, Time.unscaledDeltaTime * _movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, _newRotation, Time.unscaledDeltaTime * _movementTime);

        _cameraTransform.localPosition = Vector3.Lerp(_cameraTransform.localPosition, _newZoom, Time.unscaledDeltaTime * _movementTime);

        _depthOfField.focusDistance.value = Vector3.Distance(transform.position, _cameraTransform.position);

        _energyBar.localScale = Vector3.Lerp(_energyBar.localScale, _energyBarScaleFactor * Vector3.one, Time.unscaledDeltaTime * _movementTime);
    }

    private void CheckIfFocusAnimal()
    {
        if (_boundAnimalTransform != null && Input.GetKeyDown(KeyCode.Return))
            _animalIsFollowed = !_animalIsFollowed;

        if (_boundAnimalTransform == null)
            _animalIsFollowed = false;
    }
}