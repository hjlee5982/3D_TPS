using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using VFavorites.Libs;
using VInspector;

public class PlayerController : MonoBehaviour
{
    #region ENUM
    public enum EPlayerState
    {
        Default,
        Aiming,
        Jump
    }
    #endregion





    #region VARIABLES
    [Header("애니메이터")]
    private Animator     _animator    = null;                 // 애니메이터
    private EPlayerState _playerState = EPlayerState.Default; // 상태 변수


    [Header("카메라")]
    public Camera     PlayerCamera;       // 플레이어를 비추는 카메라
    private Transform _cameraLookTarget;  // 카메라가 바라볼 위치
    private Transform _aimCameraPosition; // 조준 시 카메라가 이동 할 위치
    private Transform _aimLookTarget;     // 조준 시 카메라가 바라 볼 위치


    [Header("이동 변수")]
    public float       MoveSpeed       = 3f;                   // 캐릭터의 이동 속도
    private float      _currentSpeed   = 0f;                   // 현재 이동 속도
    private int        _isRun          = 0;
    private Vector3    _moveDir        = Vector3.zero;         // 캐릭터가 이동하는 방향
    private Quaternion _targetRotation = Quaternion.identity;  // 캐릭터가 이동하는 방향으로의 쿼터니온


    [Header("걷기 변수")]
    public float WalkSpeed = 1f; // 캐릭터의 걷는 속도


    [Header("대시 변수")]
    public float DashSpeed = 5f; // 캐릭터의 대시 속도


    [Header("마우스 이동 변수")]
    public  float MouseMoveSensitivity  = 0.12f; // 마우스 이동 감도
    private float _yaw              = 0f;  // Z축 회전
    private float _pitch            = 0f;  // X축 회전
    private float _pitchClamp       = 89f; // 

    
    [Header("마우스 휠 변수")]
    public  float MouseWheelSensitivity  = 0.5f; // 마우스 휠 감도
    public  float ZoomLerpSpeed          = 3f;   // 줌 인,아웃 속도
    public  float MinDistance            = 1f;   // 플레이어와 카메라의 최소 거리
    public  float MaxDistance            = 4f;   // 플레이어와 카메라의 최대 거리
    private float _currentDistance       = 0f;   // 플레이어와 카메라의 현재 거리
    private float _targetDistance        = 0f;   // 카메라가 가야 할 위치


    [Header("조준 변수")]
    private Transform _spine; // 위, 아래 조준 애니메이션이 없어서 허리를 x축으로 회전시킴
    private Transform _rifle; // 위처럼 회전하면 총은 그대로여서 같이 회전시켜주려고 추가함
    private bool _isAiming = false;
    private float _amingPitch      = 0;
    private float _amingPitchClamp = 45f;


    [Header("캐릭터 컨트롤러")]
    private CharacterController _controller = null;
    private Vector3 _velocity               = Vector3.zero;
    private float   _gravity                = -20f;
    private float   _jumpHeight             = 1.1f;
    #endregion


    


    #region MONOBEHAVIOUR
    private void Awake()
    {
        _animator = transform.GetComponent<Animator>();

        _cameraLookTarget  = transform.Find("CameraLookTarget");
        _aimCameraPosition = transform.Find("AimCameraPosition");
        _aimLookTarget     = transform.Find("AimLookTarget");

        _currentDistance  = _targetDistance = Mathf.Abs(PlayerCamera.transform.position.z - _cameraLookTarget.position.z);
        _targetRotation   = Quaternion.identity;

        _spine = transform.Find("root/pelvis/spine_01");
        _rifle = transform.Find("root/add_weapon_r");

        _controller = transform.GetComponent<CharacterController>();

        _currentSpeed = MoveSpeed;
    }

    private void Start()
    {
        // 인풋 바인딩
        {
            JInputManager.Instance.BindBasicPlayerMovement(OnMove, OnDash, OnWalk, OnJump, OnAiming, OnShot);
            JInputManager.Instance.BindBasicCameraMovement(OnLook, OnZoom);
        }
    }

    private void Update()
    {
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

                BasicCharacterMove();
                BasicCameraMove();
                break;

            case EPlayerState.Aiming:

                AmingCharacterMove();
                AmingCameraMove();

                // 조준 중 위, 아래를 바라보는 애니메이션이 없어서
                // 정면을 조준하는 애니메이션에서 Spine과 Rifle을 회전시키기로 함
                {
                    // X축 회전 쿼터니온
                    Quaternion pitchRotation = Quaternion.AngleAxis(_amingPitch, transform.right);

                    // Spine 회전
                    _spine.rotation = pitchRotation * _spine.rotation;

                    // Rifle 회전
                    //_rifle.rotation = pitchRotation * _rifle.rotation;

                    //float mag = (_rifle.position - _spine.position).magnitude;

                    //Vector3 ro = Quaternion.Euler(_amingPitch, _yaw, 0f) * new Vector3(mag - 0.24f, mag - 0.04f, mag - 0.18f);

                    //_rifle.position = _spine.position + ro;
                }
               
                break;

            case EPlayerState.Jump:

                BasicCharacterMove();
                BasicCameraMove();

                if (_velocity.y <= 1f)
                {
                    _playerState = EPlayerState.Default;
                }

                break;
        }


        // 바닥 체크 (isGrounded는 CC가 제공하는 속성)
        if (_controller.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f; // 바닥에 붙어 있게 약간 음수 유지
        }

        // 중력 적용
        _velocity.y += _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void UpdateCameraRaycasting()
    {
        // 캐릭터에서 카메라로 Ray를 쏴야 오브젝트 두께를 고려 할 필요가 없음
        Vector3 dir = PlayerCamera.transform.position - _cameraLookTarget.position;
        Ray ray = new Ray(_cameraLookTarget.position, dir);

        float dist = dir.magnitude;

        int layerMask = LayerMask.GetMask("Obstacle");

        if (Physics.Raycast(ray, out RaycastHit hit, dist, layerMask) == true)
        {
            // 캐릭터와 카메라 사이에 무언가 있을 경우
            PlayerCamera.transform.position = _cameraLookTarget.position + dir.normalized * (hit.distance - 0.4f);
        }

        // 카메라 근접 투명처리
        float adjustedDist = (_cameraLookTarget.position - PlayerCamera.transform.position).magnitude;
        if (adjustedDist <= 0.7f)
        {
            PlayerCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Player"));
        }
        else
        {
            PlayerCamera.cullingMask |= (1 << LayerMask.NameToLayer("Player"));
        }
    }

    private void UpdatePlayerAnimation()
    {
        _animator.SetFloat("Speed", _currentSpeed * _isRun, 0.1f, Time.deltaTime);

        // _animator.SetFloat("AimingMoveX", _moveDir.x, 0.1f, Time.deltaTime);
        // _animator.SetFloat("AimingMoveZ", _moveDir.z, 0.1f, Time.deltaTime);

        _animator.SetBool("IsAiming", _playerState == EPlayerState.Aiming);

        _animator.speed = _currentSpeed == DashSpeed ? 1.5f : 1f;
    }

    private void BasicCharacterMove()
    {
        // 캐릭터 기본 이동
        _controller.Move(_moveDir * Time.deltaTime * _currentSpeed);

        // 캐릭터 기본 회전
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * 20f);
    }

    private void BasicCameraMove()
    {
        // 줌인, 줌아웃
        _currentDistance = Mathf.Lerp(_currentDistance, _targetDistance, Time.deltaTime * ZoomLerpSpeed);

        // 캐릭터를 중심으로 카메라의 위치와 회전을 조정
        Vector3 offset = Quaternion.Euler(_pitch, _yaw, 0f) * new Vector3(0, 0, -_currentDistance);

        PlayerCamera.transform.position = _cameraLookTarget.position + offset;
        PlayerCamera.transform.LookAt(_cameraLookTarget);
    }

    private void AmingCharacterMove()
    {
        // 캐릭터 기본 이동
        _controller.Move(_moveDir * Time.deltaTime * _currentSpeed);

        // 조준 상태의 캐릭터 회전
        transform.rotation = Quaternion.Euler(0f, _yaw, 0f);
    }

    private void AmingCameraMove()
    {
        Vector3 offset = Quaternion.Euler(_amingPitch, _yaw, 0f) * new Vector3(0, 0, -(_aimLookTarget.position - _aimCameraPosition.position).magnitude);

        _aimCameraPosition.position = _aimLookTarget.position + offset;
        _aimCameraPosition.transform.LookAt(_aimLookTarget.position);

        PlayerCamera.transform.position = _aimCameraPosition.position;
        PlayerCamera.transform.rotation = _aimCameraPosition.rotation;
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
        _yaw   += delta.x * MouseMoveSensitivity;
        _pitch -= delta.y * MouseMoveSensitivity;
        _pitch = Mathf.Clamp(_pitch, -_pitchClamp, _pitchClamp);

        // Aming Clamp
        _amingPitch -= delta.y * MouseMoveSensitivity;
        _amingPitch = Mathf.Clamp(_amingPitch, -_amingPitchClamp, _amingPitchClamp);
    }

    private void OnZoom(float delta)
    {
        _targetDistance -= delta * MouseWheelSensitivity;
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
        _currentSpeed = isDash == true ? DashSpeed : MoveSpeed;
    }

    private void OnWalk(bool isWalk)
    {
        _currentSpeed = isWalk == true ? WalkSpeed : MoveSpeed;
    }

    private void OnJump()
    {
        if (_controller.isGrounded == true)
        {
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
        }
    }

    private void OnShot()
    {
        if(_playerState == EPlayerState.Aiming)
        {
            _animator.Play("AimIdle_Shoot", 1, 0f);
        }
    }
    #endregion
}
