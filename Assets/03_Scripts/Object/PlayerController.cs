using UnityEngine;
using VFavorites.Libs;
using VInspector;

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
    [Header("애니메이터")]
    private Animator     _animator;
    private EPlayerState _playerState = EPlayerState.Default;

    [Header("카메라")]
    public Camera     PlayerCamera;  // 플레이어를 비추는 카메라
    private Transform _lookAtPos;    // 카메라가 바라볼 위치
    private Transform _aimingPos;    // 조준 시 카메라가 이동 할 위치
    private Transform _aimingLook;   // 조준 시 카메라가 바라 볼 위치

    [Header("이동 변수")]
    public float       MoveSpeed        = 3f;
    private int        _isRun           = 0;
    private Vector3    _moveDir;
    private Quaternion _targetRotation;
    private float      _currentSpeed    = 0f;

    [Header("마우스 변수")]
    public  float MouseMoveSpeed  = 0.12f;
    private float _yaw            = 0f;
    private float _pitch          = 0f;
    private float _pitchClamp     = 89f;

    [Header("마우스 휠 변수")]
    public  float MouseWheelSpeed  = 0.5f;
    public  float ZoomLerpSpeed    = 3f;
    public  float MinDistance      = 1f; // 플레이어와 카메라의 최소 거리
    public  float MaxDistance      = 4f; // 플레이어와 카메라의 최대 거리
    private float _currentDistance = 0f; // 플레이어와 카메라의 현재 거리
    private float _targetDistance  = 0f; // 카메라가 가야 할 위치

    [Header("조준 변수")]
    private Transform _spine; // 위, 아래 조준 애니메이션이 없어서 허리를 x축으로 회전시킴
    private Transform _rifle; // 위처럼 회전하면 총은 그대로여서 같이 회전시켜주려고 추가함
    private bool _isAiming = false;
    
    [Header("걷기 변수")]
    public float WalkSpeed = 1f;

    [Header("대시 변수")]
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

        _spine = transform.Find("spine_01");
        _rifle = transform.Find("add_weapon_r");
    }

    private void Start()
    {
        // 인풋 바인딩
        {
            JInputManager.Instance.BindBasicPlayerMovement(OnMove, OnDash, OnWalk, OnJump, OnAiming);
            JInputManager.Instance.BindBasicCameraMovement(OnLook, OnZoom);
        }
    }

    private void LateUpdate()
    {
        UpdateTransform();

        UpdateCameraRaycasting();

        UpdatePlayerAnimation();
    }
    #endregion





    #region FUNCTIONS
    private void UpdateTransform()
    {
        switch (_playerState)
        {
            case EPlayerState.Default:
                // 플레이어 위치, 회전 변경
                {
                    _currentSpeed = MoveSpeed;

                    transform.position += _moveDir * Time.deltaTime * MoveSpeed;
                    transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * 20f);
                }
                // 카메라 위치, 회전 변경
                {
                    _currentDistance = Mathf.Lerp(_currentDistance, _targetDistance, Time.deltaTime * ZoomLerpSpeed);

                    Vector3 offset = Quaternion.Euler(_pitch, _yaw, 0f) * new Vector3(0, 0, -_currentDistance);

                    PlayerCamera.transform.position = _lookAtPos.position + offset;
                    PlayerCamera.transform.LookAt(_lookAtPos);
                }
                break;

            case EPlayerState.Aiming:
                // 플레이어 위치, 회전 변경
                {
                    _currentSpeed = MoveSpeed;

                    transform.position += _moveDir * Time.deltaTime * MoveSpeed;
                    transform.rotation = Quaternion.Euler(0f, _yaw, 0f);
                }
                // 카메라 위치, 회전 변경
                {
                    Vector3 offset = Quaternion.Euler(_pitch, _yaw, 0f) * new Vector3(0, 0, -(_aimingLook.position - _aimingPos.position).magnitude);

                    _aimingPos.position = _aimingLook.position + offset;
                    _aimingPos.transform.LookAt(_aimingLook.position);

                    PlayerCamera.transform.position = _aimingPos.position;
                    PlayerCamera.transform.rotation = _aimingPos.rotation;
                }

                // 공통 회전 변수
                Vector3    worldRight = transform.right;
                Quaternion pitchRotation = Quaternion.AngleAxis(_pitch, worldRight);

                // Spine 회전
                {
                    Quaternion originalSpineRotation = _spine.rotation;

                    _spine.rotation = pitchRotation * originalSpineRotation;
                }
                // Rifle 회전
                {
                    Quaternion originalRifleRotation = _rifle.rotation;

                    _rifle.rotation = pitchRotation * originalRifleRotation;

                    float mag = (_rifle.position - _spine.position).magnitude;

                    Vector3 ro = Quaternion.Euler(_pitch, _yaw, 0f) * new Vector3(mag - 0.24f, mag - 0.04f, mag - 0.18f);

                    _rifle.position = _spine.position + ro;
                }
                break;

            case EPlayerState.Dash:
                // 플레이어 위치, 회전 변경
                {
                    _currentSpeed = DashSpeed;

                    transform.position += _moveDir * Time.deltaTime * DashSpeed;
                    transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * 20f);
                }
                // 카메라 위치, 회전 변경
                {
                    _currentDistance = Mathf.Lerp(_currentDistance, _targetDistance, Time.deltaTime * ZoomLerpSpeed);

                    Vector3 offset = Quaternion.Euler(_pitch, _yaw, 0f) * new Vector3(0, 0, -_currentDistance);

                    PlayerCamera.transform.position = _lookAtPos.position + offset;
                    PlayerCamera.transform.LookAt(_lookAtPos);
                }
                break;

            case EPlayerState.Walk:
                // 플레이어 위치, 회전 변경
                {
                    _currentSpeed = WalkSpeed;

                    transform.position += _moveDir * Time.deltaTime * WalkSpeed;
                    transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * 20f);
                }
                // 카메라 위치, 회전 변경
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

    private void UpdateCameraRaycasting()
    {
        Ray ray = new Ray(PlayerCamera.transform.position, _lookAtPos.position - PlayerCamera.transform.position);

        float dist = (_lookAtPos.position - PlayerCamera.transform.position).magnitude;

        int layerMask = LayerMask.GetMask("Obstacle");

        if(Physics.Raycast(ray, out RaycastHit hit, dist, layerMask) == true)
        {
            Debug.Log("충돌 : " + hit.transform.name);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(PlayerCamera.transform.position, _lookAtPos.position - PlayerCamera.transform.position);
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
