using UnityEngine;
using VFavorites.Libs;

public class PlayerController : MonoBehaviour
{
    #region ENUM
    public enum EPlayerState
    {
        Default,
        Aiming,
    }
    #endregion





    #region VARIABLES
    [Header("애니메이터")]
    private Animator     _animator;
    private EPlayerState _playerState = EPlayerState.Default;

    [Header("카메라")]
    public Camera        PlayerCamera; // 플레이어를 비추는 카메라 (플레이어 하위에 있어야 함)
    private Transform    _lookAtPos;    // 카메라가 바라볼 위치
    private Transform    _aimingPos;    // 조준 시 카메라가 이동 할 위치

    [Header("이동 변수")]
    public float       MoveSensitivity   = 1f;
    private Vector3    _moveDir;
    private float      _currentMagnitude;
    private float      _targetMagnitude;
    private Quaternion _targetRotation;

    [Header("마우스 변수")]
    public  float MouseMoveSensitivity  = 0.12f;
    private float _yaw                  = 0f;
    private float _pitch                = 0f;
    private float _pitchClamp           = 89f;

    [Header("마우스 휠 변수")]
    public  float MouseWheelSensitivity = 0.5f;
    public  float ZoomLerpSpeed         = 3f;
    public  float MinDistance           = 1f; // 플레이어와 카메라의 최소 거리
    public  float MaxDistance           = 4f; // 플레이어와 카메라의 최대 거리
    private float _currentDistance      = 0f; // 플레이어와 카메라의 현재 거리
    private float _targetDistance       = 0f; // 카메라가 가야 할 위치

    [Header("조준 변수")]
    private bool _isAiming = false;
    #endregion





    #region MONOBEHAVIOUR
    private void Awake()
    {
        _animator = transform.GetComponent<Animator>();

        _lookAtPos       = transform.Find("LookAtPosition");
        _aimingPos       = transform.Find("AimingPosition");

        _currentDistance  = _targetDistance = Mathf.Abs(PlayerCamera.transform.position.z);
        _currentMagnitude = _targetMagnitude = 0f;
        _targetRotation   = Quaternion.identity;
    }

    private void Start()
    {
        BindInput();
    }

    private void LateUpdate()
    {
        UpdateTransform();
        UpdatePlayerAnimation();
    }
    #endregion





    #region FUNCTIONS
    private void BindInput()
    {
        JInputManager.Instance.BindBasicMovement(OnMove, OnLook, OnZoom);

        JInputManager.Instance.BindKey(OnJump,   "Jump");
        JInputManager.Instance.BindKey(OnAiming, "Aiming");
    }

    private void UpdateTransform()
    {
        switch (_playerState)
        {
            case EPlayerState.Default:

                // 플레이어 위치, 회전 변경
                {
                    transform.position += _moveDir * Time.deltaTime * MoveSensitivity;
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
                    transform.position += _moveDir * Time.deltaTime * MoveSensitivity;
                    transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
                }
                // 카메라 위치, 회전 변경
                {
                    PlayerCamera.transform.position = _aimingPos.position;
                    PlayerCamera.transform.rotation = _aimingPos.rotation;
                }
                break;
        }
    }

    private void UpdatePlayerAnimation()
    {
        // _currentMagnitude = Mathf.Lerp(_currentMagnitude, _targetMagnitude, Time.deltaTime * t);

        _animator.SetFloat("Speed", _targetMagnitude, 0.1f, Time.deltaTime);
        _animator.SetFloat("AimingMoveX", _moveDir.x, 0.1f, Time.deltaTime);
        _animator.SetFloat("AimingMoveZ", _moveDir.z, 0.1f, Time.deltaTime);
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

        _targetMagnitude = dir.magnitude;

        if(dir.magnitude != 0f)
        {
            _targetRotation  = Quaternion.LookRotation(_moveDir);
        }
    }

    private void OnLook(Vector2 delta)
    {
        _yaw   += delta.x * MouseMoveSensitivity;
        _pitch -= delta.y * MouseMoveSensitivity;
        _pitch = Mathf.Clamp(_pitch, -_pitchClamp, _pitchClamp);
    }

    private void OnZoom(float delta)
    {
        _targetDistance -= delta * MouseWheelSensitivity;
        _targetDistance = Mathf.Clamp(_targetDistance, MinDistance, MaxDistance);
    }

    private void OnAiming()
    {
        _isAiming = !_isAiming;

        if(_isAiming == true)
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

        _animator.SetBool("IsAiming", _isAiming);
    }

    private void OnJump()
    {   
        Debug.Log("Jump 입력");
    }
    #endregion
}
