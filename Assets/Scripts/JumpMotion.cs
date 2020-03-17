using DG.Tweening;
using UnityEngine;

public class JumpMotion : MonoBehaviour
{
    [SerializeField]
    private float jumpLength = 2.5f;

    [SerializeField]
    private float jumpPower = 1f;

    [SerializeField]
    private float jumpDuration = 0.5f;

    [SerializeField]
    [Tooltip("Local scale on ground is default scale multiplied component by component by this vector factor")]
    private Vector3 _groundScaleFactor = new Vector3(1.3f, 0.7f, 1.3f);

    [SerializeField]
    [Tooltip("Local scale on air is default scale multiplied component by component by this vector factor")]
    private Vector3 _airScaleFactor = new Vector3(0.7f, 1.3f, 0.7f);

    bool isOnAir = false;

    private Vector3 _startingScale;
    private Vector3 _groundScale;
    private Vector3 _airScale;

    private void Awake()
    {
        _startingScale = transform.localScale;

        _groundScale = Vector3.Scale(_startingScale, _groundScaleFactor);
        _airScale = Vector3.Scale(_startingScale, _airScaleFactor);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && !isOnAir)
        {
            isOnAir = true;
            Jump();
        }
    }

    public void Jump()
    {
        // scale change - preparing for jump
        transform.DOScale(_groundScale, 0.5f * jumpDuration)
                 .OnComplete(MakeJump);
        // correction to y-coord so it sits on the ground
        float _correctionY = _startingScale.y - 0.5f * (_startingScale.y - _groundScaleFactor.y);
        transform.DOLocalMoveY(_correctionY, 0.5f * jumpDuration);
    }

    private void MakeJump()
    {
        // start jump with scale at roughly half jump
        transform.DOJump(transform.position + transform.forward * jumpLength, jumpPower, 1, jumpDuration);
        transform.DOScale(_airScale, 0.7f * jumpDuration)
                 .OnComplete(LandJump);
    }

    private void LandJump()
    {
        // landing
        transform.DOScale(_groundScale, 0.3f * jumpDuration)
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
        isOnAir = false;
    }
}
