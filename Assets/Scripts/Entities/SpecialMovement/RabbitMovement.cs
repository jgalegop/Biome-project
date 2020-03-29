using DG.Tweening;
using UnityEngine;
using System.Collections;
using Entities;

[RequireComponent(typeof(Animal))]
public class RabbitMovement : MonoBehaviour
{
    private float _jumpLength;

    [SerializeField]
    private float _jumpPower = 1f;

    private readonly float _jumpDuration = 0.5f;
    private readonly float _restTime = 0.5f;

    [SerializeField]
    [Tooltip("Local scale on ground is default scale multiplied component by component by this vector factor")]
    private Vector3 _groundScaleFactor = new Vector3(1.3f, 0.7f, 1.3f);

    [SerializeField]
    [Tooltip("Local scale on air is default scale multiplied component by component by this vector factor")]
    private Vector3 _airScaleFactor = new Vector3(0.7f, 1.3f, 0.7f);

    private bool _isJumping = false;

    private Vector3 _startingScale;
    private Vector3 _groundScale;
    private Vector3 _airScale;

    private Animal _animal;
    private float _moveSpeed;

    private void Awake()
    {
        SetDefaultScale(1f);
    }

    private void Start()
    {
        _animal = GetComponent<Animal>();
        _moveSpeed = _animal.GetMoveSpeed();

        if (Mathf.Abs(_jumpDuration + _restTime - 1) < Mathf.Epsilon)
            _jumpLength = _moveSpeed;
        else
            Debug.LogError("Jump duration + rest time must be equal to one");
    }


    public void Tick(Vector3 destination)
    {
        Vector3 _direction = Vector3.Normalize(destination - transform.position);
        _direction = new Vector3(_direction.x, 0f, _direction.z);
        Quaternion _desiredRotation = Quaternion.LookRotation(_direction);

        if (Vector3.Dot(transform.forward, _direction) < 0.8f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, _desiredRotation, 0.2f);
        }
        else
        {
            Move(destination);
        }
    }

    private void Move(Vector3 destination)
    {
        if (Vector3.Distance(transform.position, destination) >= _jumpLength)
        {
            if (_isJumping)
                return;
            Jump();
        }
        else
        {
            transform.Translate(Vector3.forward * _moveSpeed * Time.deltaTime);
        }
    }

    public void SetDefaultScale(float factor)
    {
        _startingScale = factor * transform.localScale;
        _groundScale = Vector3.Scale(_startingScale, _groundScaleFactor);
        _airScale = Vector3.Scale(_startingScale, _airScaleFactor);
    }

    private void Jump()
    {   
        _isJumping = true;
        // scale change - preparing for jump
        transform.DOScale(_groundScale, 0.5f * _jumpDuration)
                 .OnComplete(MakeJump);

        // correction to y-coord so it sits on the ground
        float _correctionY = _startingScale.y - 0.5f * (_startingScale.y - _groundScaleFactor.y);
        transform.DOLocalMoveY(_correctionY, 0.5f * _jumpDuration);
    }

    private void MakeJump()
    {
        // start jump with scale at roughly half jump
        // make sure that jumps into the same plane
        Vector3 correctForward = transform.forward - Vector3.up * transform.forward.y;
        transform.DOJump(transform.position + correctForward * _jumpLength, _jumpPower, 1, _jumpDuration);
        transform.DOScale(_airScale, 0.7f * _jumpDuration)
                 .OnComplete(LandJump);
    }

    private void LandJump()
    {
        // landing
        transform.DOScale(_groundScale, 0.3f * _jumpDuration)
                 .OnComplete(Recover);
    }

    private void Recover()
    {
        // recover original configuration
        transform.DOScale(_startingScale, 0.1f)
                 .OnComplete(NotOnAir);
        // correction to y-coord
        transform.DOLocalMoveY(_startingScale.y, 0.1f);
    }

    private void NotOnAir()
    {
        StartCoroutine(RestAfterJump());
    }

    IEnumerator RestAfterJump()
    {
        yield return new WaitForSeconds(_restTime);
        _isJumping = false;
    }

    public bool GetIsJumping()
    {
        return _isJumping;
    }

    private void OnDestroy()
    {
        if (DOTween.IsTweening(transform))
            DOTween.Kill(transform);
    }
}
