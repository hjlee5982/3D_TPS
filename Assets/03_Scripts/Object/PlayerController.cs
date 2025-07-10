using UnityEngine;
using VFavorites.Libs;

public class PlayerController : MonoBehaviour
{
    #region ENUM
    public enum EPlayerState
    {
        Default,
        Aiming,
        Dash,
        Walk,
        Jump
    }
    #endregion





    #region VARIABLES
    [Header("�ִϸ�����")]
    private Animator     _animator;
    private EPlayerState _playerState = EPlayerState.Default;

    [Header("ī�޶�")]
    public Camera     PlayerCamera;  // �÷��̾ ���ߴ� ī�޶�
    private Transform _lookAtPos;    // ī�޶� �ٶ� ��ġ
    private Transform _aimingPos;    // ���� �� ī�޶� �̵� �� ��ġ
    private Transform _aimingLook;   // ���� �� ī�޶� �ٶ� �� ��ġ

    [Header("�̵� ����")]
    public float       MoveSpeed        = 3f;
    private int        _isRun           = 0;
    private Vector3    _moveDir;
    private Quaternion _targetRotation;
    private float      _currentSpeed    = 0f;

    [Header("���콺 ����")]
    public  float MouseMoveSpeed  = 0.12f;
    private float _yaw            = 0f;
    private float _pitch          = 0f;
    private float _pitchClamp     = 89f;

    [Header("���콺 �� ����")]
    public  float MouseWheelSpeed  = 0.5f;
    public  float ZoomLerpSpeed    = 3f;
    public  float MinDistance      = 1f; // �÷��̾�� ī�޶��� �ּ� �Ÿ�
    public  float MaxDistance      = 4f; // �÷��̾�� ī�޶��� �ִ� �Ÿ�
    private float _currentDistance = 0f; // �÷��̾�� ī�޶��� ���� �Ÿ�
    private float _targetDistance  = 0f; // ī�޶� ���� �� ��ġ

    [Header("���� ����")]
    public Transform Spine;
    private bool _isAiming = false;


    [Header("�ȱ� ����")]
    public float WalkSpeed = 1f;

    [Header("��� ����")]
    public float DashSpeed = 5f;
    #endregion





    #region MONOBEHAVIOUR
    private void Awake()
    {
        _animator = transform.GetComponent<Animator>();

        _lookAtPos       = transform.Find("LookAtPosition");
        _aimingPos       = transform.Find("AimingPosition");
        _aimingLook      = transform.Find("AimingLook");

        _currentDistance  = _targetDistance = Mathf.Abs(PlayerCamera.transform.position.z);
        _targetRotation   = Quaternion.identity;
    }

    private void Start()
    {
        // ��ǲ ���ε�
        {
            JInputManager.Instance.BindBasicPlayerMovement(OnMove, OnDash, OnWalk, OnJump, OnAiming);
            JInputManager.Instance.BindBasicCameraMovement(OnLook, OnZoom);
        }
    }

    private void LateUpdate()
    {
        UpdateTransform();
        UpdatePlayerAnimation();
    }
    #endregion





    #region FUNCTIONS
    private void UpdateTransform()
    {
        switch (_playerState)
        {
            case EPlayerState.Default:
                // �÷��̾� ��ġ, ȸ�� ����
                {
                    _currentSpeed = MoveSpeed;

                    transform.position += _moveDir * Time.deltaTime * MoveSpeed;
                    transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * 20f);
                }
                // ī�޶� ��ġ, ȸ�� ����
                {
                    _currentDistance = Mathf.Lerp(_currentDistance, _targetDistance, Time.deltaTime * ZoomLerpSpeed);

                    Vector3 offset = Quaternion.Euler(_pitch, _yaw, 0f) * new Vector3(0, 0, -_currentDistance);

                    PlayerCamera.transform.position = _lookAtPos.position + offset;
                    PlayerCamera.transform.LookAt(_lookAtPos);
                }
                break;

            case EPlayerState.Aiming:
                // �÷��̾� ��ġ, ȸ�� ����
                {
                    _currentSpeed = MoveSpeed;

                    transform.position += _moveDir * Time.deltaTime * MoveSpeed;
                    transform.rotation = Quaternion.Euler(0f, _yaw, 0f);

                    Quaternion originalRotation = Spine.rotation;

                    Vector3 worldRight = transform.right;
                    Quaternion pitchRotation = Quaternion.AngleAxis(_pitch, worldRight);
                    Spine.rotation = pitchRotation * originalRotation;

                    Vector3 offset = Quaternion.Euler(_pitch, _yaw, 0f) * new Vector3(0, 0, -(_aimingLook.position - _aimingPos.position).magnitude);

                    _aimingPos.position = _aimingLook.position + offset;
                    _aimingPos.transform.LookAt(_aimingLook.position);
                }
                // ī�޶� ��ġ, ȸ�� ����
                {
                    PlayerCamera.transform.position = _aimingPos.position;
                    PlayerCamera.transform.rotation = _aimingPos.rotation;
                }
                break;

            case EPlayerState.Dash:
                // �÷��̾� ��ġ, ȸ�� ����
                {
                    _currentSpeed = DashSpeed;

                    transform.position += _moveDir * Time.deltaTime * DashSpeed;
                    transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * 20f);
                }
                // ī�޶� ��ġ, ȸ�� ����
                {
                    _currentDistance = Mathf.Lerp(_currentDistance, _targetDistance, Time.deltaTime * ZoomLerpSpeed);

                    Vector3 offset = Quaternion.Euler(_pitch, _yaw, 0f) * new Vector3(0, 0, -_currentDistance);

                    PlayerCamera.transform.position = _lookAtPos.position + offset;
                    PlayerCamera.transform.LookAt(_lookAtPos);
                }
                break;

            case EPlayerState.Walk:
                // �÷��̾� ��ġ, ȸ�� ����
                {
                    _currentSpeed = WalkSpeed;

                    transform.position += _moveDir * Time.deltaTime * WalkSpeed;
                    transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * 20f);
                }
                // ī�޶� ��ġ, ȸ�� ����
                {
                    _currentDistance = Mathf.Lerp(_currentDistance, _targetDistance, Time.deltaTime * ZoomLerpSpeed);

                    Vector3 offset = Quaternion.Euler(_pitch, _yaw, 0f) * new Vector3(0, 0, -_currentDistance);

                    PlayerCamera.transform.position = _lookAtPos.position + offset;
                    PlayerCamera.transform.LookAt(_lookAtPos);
                }
                break;

            case EPlayerState.Jump:
                break;
        }
    }

    private void UpdatePlayerAnimation()
    {
        _animator.SetFloat("Speed", _currentSpeed * _isRun, 0.1f, Time.deltaTime);

        _animator.SetFloat("AimingMoveX", _moveDir.x, 0.1f, Time.deltaTime);
        _animator.SetFloat("AimingMoveZ", _moveDir.z, 0.1f, Time.deltaTime);

        _animator.SetBool("IsAiming", _playerState == EPlayerState.Aiming);

        _animator.speed = _playerState == EPlayerState.Dash ? 1.5f : 1f;
    }

    private void OnMove(Vector3 dir)
    {
        Vector3 cameraLook  = PlayerCamera.transform.forward;
        Vector3 cameraRight = PlayerCamera.transform.right;

        cameraLook.y = 0f;
        cameraRight.y = 0f;

        cameraLook.Normalize();
        cameraRight.Normalize();

        _moveDir = cameraLook * dir.z + cameraRight * dir.x;

        _isRun = dir.magnitude == 1f ? 1 : 0;

        if(dir.magnitude != 0f)
        {
            _targetRotation  = Quaternion.LookRotation(_moveDir);
        }
    }

    private void OnLook(Vector2 delta)
    {
        _yaw   += delta.x * MouseMoveSpeed;
        _pitch -= delta.y * MouseMoveSpeed;
        _pitch = Mathf.Clamp(_pitch, -_pitchClamp, _pitchClamp);
    }

    private void OnZoom(float delta)
    {
        _targetDistance -= delta * MouseWheelSpeed;
        _targetDistance = Mathf.Clamp(_targetDistance, MinDistance, MaxDistance);
    }

    private void OnAiming()
    {
        _isAiming = !_isAiming;

        if (_isAiming == true)
        {
            _playerState = EPlayerState.Aiming;
        }
        else
        {
            _playerState = EPlayerState.Default;

            Vector3 cameraDir = PlayerCamera.transform.forward;
            cameraDir.y = 0f;
            cameraDir.Normalize();

            _targetRotation = Quaternion.LookRotation(cameraDir);
        }
    }

    private void OnDash(bool isDash)
    {
        if(_playerState == EPlayerState.Aiming)
        {
            return;
        }

        _playerState = isDash == true ? EPlayerState.Dash : EPlayerState.Default;
    }

    private void OnWalk(bool isWalk)
    {
        if (_playerState == EPlayerState.Aiming)
        {
            return;
        }

        _playerState = isWalk == true ? EPlayerState.Walk : EPlayerState.Default;
    }

    private void OnJump()
    {
        _playerState = EPlayerState.Jump;
    }
    #endregion
}
