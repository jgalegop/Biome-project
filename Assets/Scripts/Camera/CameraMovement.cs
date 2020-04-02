using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using UnityEngine;

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


    private void Awake()
    {
        ClickSelectController.OnSelectedAnimalChanged += BindSelectedAnimal;
        BindSelectedAnimal(ClickSelectController.SelectedAnimal);
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
            }
        }
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

            _newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / _rotationMouseAmount));
        }
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

        transform.position = Vector3.Lerp(transform.position, _newPosition, Time.deltaTime * _movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, _newRotation, Time.deltaTime * _movementTime);

        _cameraTransform.localPosition = Vector3.Lerp(_cameraTransform.localPosition, _newZoom, Time.deltaTime * _movementTime);
    }

    private void CheckIfFocusAnimal()
    {
        if (_boundAnimalTransform != null && Input.GetKeyDown(KeyCode.Return))
            _animalIsFollowed = !_animalIsFollowed;

        if (_boundAnimalTransform == null)
            _animalIsFollowed = false;
    }
}
