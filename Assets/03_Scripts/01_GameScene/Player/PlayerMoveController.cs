using System.Collections;
using UnityEngine;

public class PlayerController : JBaseClass
{
    #region VARIABLES
    [Header("상태 머신")]
    public JStateMachine<PlayerController> StateMachine { get; private set; } = null;
    public PlayerIdleState                 IdleState    { get; private set; } = null;
    public PlayerAimingState               AimState     { get; private set; } = null;


    [Header("애니메이터")]
    public Animator Animator { get; private set; } = null;
    

    [Header("카메라")]
    public Camera     PlayerCamera       = null;  // 플레이어를 비추는 카메라
    private Transform _cameraLookTarget  = null;  // 카메라가 바라볼 위치
    private Transform _aimCameraPosition = null;  // 조준 시 카메라가 이동 할 위치
    private Transform _aimLookTarget     = null;  // 조준 시 카메라가 바라 볼 위치


    [Header("이동 변수")]
    public  float      MoveSpeed       = 3f;                  // 캐릭터의 기준 속도
    private int        _isMoving       = 0;                   // 현재 이동하고 있는지 체크
    private float      _currentSpeed   = 0f;                  // 현재 이동 속도(내부 계산용)
    private Vector3    _moveDir        = Vector3.zero;        // 캐릭터가 이동하는 방향
    private Quaternion _targetRotation = Quaternion.identity; // 캐릭터가 이동하는 방향으로의 쿼터니온


    [Header("걷기 변수")]
    public float WalkSpeed = 1f; // 캐릭터의 걷는 속도


    [Header("대시 변수")]
    public float DashSpeed = 5f; // 캐릭터의 대시 속도


    [Header("마우스 이동 변수")]
    public float MouseMoveSensitivity = 0.12f; // 마우스 이동 감도
    public float _yaw                 = 0f;    // Z축 회전
    public float _pitch               = 0f;    // X축 회전
    public float _pitchClamp          = 89f;   // X축 회전 제한 각도

    
    [Header("마우스 휠 변수")]
    public  float MouseWheelSensitivity  = 0.5f; // 마우스 휠 감도
    public  float ZoomLerpSpeed          = 3f;   // 줌 인,아웃 속도
    public  float MinDistance            = 1f;   // 플레이어와 카메라의 최소 거리
    public  float MaxDistance            = 4f;   // 플레이어와 카메라의 최대 거리
    private float _currentDistance       = 0f;   // 플레이어와 카메라의 현재 거리
    private float _targetDistance        = 0f;   // 카메라가 가야 할 위치


    [Header("조준 변수")]
    private Transform _spine           = null;  // 위, 아래 조준 애니메이션이 없어서 허리를 x축으로 회전시킴
    private Transform _rifle           = null;  // 위처럼 회전하면 총은 그대로여서 같이 회전시켜주려고 추가함
    private bool      _isAiming        = false; // 조준상태인지 체크
    private float     _amingPitch      = 0;     // 조준상태에서 마우스 X축 회전
    private float     _amingPitchClamp = 45f;   // 조준상태에서 마우스 X축 회전 제한
    private float     _aimDistance     = 0f;    // 조준상태에서 카메라 위치와 카메라가 바라보는 위치 사이의 거리


    [Header("캐릭터 컨트롤러")]
    private CharacterController _controller = null;
    private Vector3             _velocity   = Vector3.zero;
    private float               _gravity    = -20f;
    private float               _jumpHeight = 1.1f;


    [Header("총알 생성 위치")]
    private Transform _shotPoint;


    [Header("카메라 흔들림 오프셋")]
    private Vector3 _cameraShakeOffset      = Vector3.zero;
    private Vector3 _cameraShakeAngleOffset = Vector3.zero;


    [Header("총기 반동")]
    public float RifleRecoilRate  = 10f;
    public float CameraRecoilRate = 0.05f;


    [Header("걷는 소리")]
    private bool  _footStepSwitch  = false;
    private float _stepTimer       = 0f;
    private float _walkInterval    = 0.55f;
    private float _runInterval     = 0.33f;
    private float _dashInterval    = 0.2f;
    private float _aimWalkInterval = 0.6f;
    private float _aimRunInterval  = 0.4f;
    private float _aimDashInterval = 0.27f;

    [Header("총알 확인 변수")]
    private bool _hasBullet = true;
    #endregion





    #region MONOBEHAVIOUR
    private void Awake()
    {
        InitializeComponents();
        InitializeTransforms();
        InitializeStateMachine();
    }
    
    private void Start()
    {
        InitializeValues();
        InitializeInputActions();
    }

    private void LateUpdate()
    {
        UpdateStateMachine();
        ApplyGravity();
        UpdateCameraRaycasting();
        UpdatePlayerAnimation();
        PlayerFootStepSound();
    }

    private void OnEnable()
    {
        JEventManager.Subscribe<BulletCountCheckEvent>(BulletCountCheck);
    }

    private void OnDisable()
    {
        JEventManager.Unsubscribe<BulletCountCheckEvent>(BulletCountCheck);
    }
    #endregion





    #region OVERRIDE
    protected override void InitializeComponents()
    {
        Animator    = transform.GetComponent<Animator>();
        _controller = transform.GetComponent<CharacterController>();
    }

    protected override void InitializeTransforms()
    {
        _cameraLookTarget  = transform.Find("CameraLookTarget");
        _aimCameraPosition = transform.Find("AimCameraPosition");
        _aimLookTarget     = transform.Find("AimLookTarget");
        _spine             = transform.Find("root/pelvis/spine_01");
        _rifle             = transform.Find("root/add_weapon_r");
        _shotPoint         = transform.Find("root/add_weapon_r/Weapon_Rifle/ShotPoint");
    }

    protected override void InitializeValues()
    {
        _currentSpeed    = MoveSpeed;
        _aimDistance     = (_aimLookTarget.position - _aimCameraPosition.position).magnitude;
        _currentDistance = _targetDistance = Mathf.Abs(PlayerCamera.transform.position.z - _cameraLookTarget.position.z);
    }
    #endregion





    #region FUNCTIONS
    private void InitializeStateMachine()
    {
        StateMachine = new JStateMachine<PlayerController>(this);
        {
            IdleState = new PlayerIdleState();
            AimState  = new PlayerAimingState();
        }
        StateMachine.ChangeState(IdleState);
    }

    private void InitializeInputActions()
    {
        JInputManager.Instance.BindButtonAction(OnMove, OnDash, OnWalk, OnJump, OnAiming, OnReload);
        JInputManager.Instance.BindHoldingAction(OnShot, 0.1f);
        JInputManager.Instance.BindCameraAction(OnLook, OnZoom);
    }

    private void UpdateStateMachine()
    {
        StateMachine.Update();
    }

    private void UpdateCameraRaycasting()
    {
        // 캐릭터에서 카메라로 Ray를 쏴야 오브젝트 두께를 고려 할 필요가 없음
        Vector3 dir = PlayerCamera.transform.position - _cameraLookTarget.position;
        Ray     ray = new Ray(_cameraLookTarget.position, dir);

        float dist = dir.magnitude;

        if (Physics.Raycast(ray, out RaycastHit hit, dist, LayerMask.GetMask("Obstacle")) == true)
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
        Animator.SetFloat("Speed", _currentSpeed * _isMoving, 0.05f, Time.deltaTime);

        Animator.speed = _currentSpeed == DashSpeed ? 1.5f : 1f;
    }

    public void BasicCharacterMove()
    {
        // 캐릭터 기본 이동
        _controller.Move(_moveDir * Time.deltaTime * _currentSpeed);

        // 캐릭터 기본 회전
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * 20f);
    }

    public void BasicCameraMove()
    {
        // 줌인, 줌아웃
        _currentDistance = Mathf.Lerp(_currentDistance, _targetDistance, Time.deltaTime * ZoomLerpSpeed);

        // 캐릭터를 중심으로 카메라의 위치와 회전을 조정
        Vector3 offset = Quaternion.Euler(_pitch, _yaw, 0f) * new Vector3(0, 0, -_currentDistance);

        PlayerCamera.transform.position = _cameraLookTarget.position + offset;
        PlayerCamera.transform.LookAt(_cameraLookTarget);
    }

    public void AmingCharacterMove()
    {
        // 캐릭터 기본 이동
        _controller.Move(_moveDir * Time.deltaTime * _currentSpeed);

        // 조준 상태의 캐릭터 회전
        transform.rotation = Quaternion.Euler(0f, _yaw, 0f);
    }

    public void AmingCameraMove()
    {
        Vector3 offset = Quaternion.Euler(_amingPitch, _yaw, 0f) * new Vector3(0, 0, -_aimDistance);

        _aimCameraPosition.position = _aimLookTarget.position + offset;
        _aimCameraPosition.transform.LookAt(_aimLookTarget.position);

        PlayerCamera.transform.position = _aimCameraPosition.position + _cameraShakeOffset;
        PlayerCamera.transform.rotation = _aimCameraPosition.rotation;
    }

    public void AmingTransformAdjust()
    {
        // 조준 중 위, 아래를 바라보는 애니메이션이 없어서
        // 정면을 조준하는 애니메이션에서 Spine과 Rifle을 회전시키기로 함
        {
            // X축 회전 쿼터니온
            Quaternion pitchRotation = Quaternion.AngleAxis(_amingPitch, transform.right);

            // Spine 회전
            _spine.rotation = pitchRotation * _spine.rotation;

            // Rifle 회전
            _rifle.rotation = pitchRotation * _rifle.rotation;

            // Rifle 위치 보정
            Vector3 localOffset  = _rifle.position - _spine.position;
            Vector3 rotateOffset = pitchRotation * localOffset;
            _rifle.position = _spine.position + rotateOffset;
        }
        // 총구 정렬
        {
            Ray ray = PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                _shotPoint.LookAt(hit.point);
            }
            else
            {
                _shotPoint.LookAt(ray.GetPoint(50f));
            }
        }
    }

    private void ApplyGravity()
    {
        // 바닥 체크 (isGrounded는 CC가 제공하는 속성)
        if (_controller.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f; // 바닥에 붙어 있게 약간 음수 유지
        }

        // 중력 적용
        _velocity.y += _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void BulletCountCheck(BulletCountCheckEvent e)
    {
        _hasBullet = e.Value;
    }





    #region INPUT
    private void OnMove(Vector3 dir)
    {
        Vector3 cameraLook = PlayerCamera.transform.forward;
        Vector3 cameraRight = PlayerCamera.transform.right;

        cameraLook.y = 0f;
        cameraRight.y = 0f;

        cameraLook.Normalize();
        cameraRight.Normalize();

        _moveDir = cameraLook * dir.z + cameraRight * dir.x;

        if (dir.magnitude != 0f)
        {
            _targetRotation = Quaternion.LookRotation(_moveDir);
            _isMoving = 1;
        }
        else
        {
            _isMoving = 0;
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

    private void OnLook(Vector2 delta)
    {
        _yaw += delta.x * MouseMoveSensitivity;
        _pitch -= delta.y * MouseMoveSensitivity;
        _pitch = Mathf.Clamp(_pitch, -_pitchClamp, _pitchClamp);

        // Aming Clamp
        if (_isAiming == true)
        {
            _amingPitch -= delta.y * MouseMoveSensitivity;
            _amingPitch = Mathf.Clamp(_amingPitch, -_amingPitchClamp, _amingPitchClamp);
        }
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
            _amingPitch = Mathf.Clamp(_pitch, -_amingPitchClamp, +_amingPitchClamp);
            StateMachine.ChangeState(AimState);
        }
        else
        {
            StateMachine.ChangeState(IdleState);

            Vector3 cameraDir = PlayerCamera.transform.forward;
            cameraDir.y = 0f;
            cameraDir.Normalize();

            _targetRotation = Quaternion.LookRotation(cameraDir);
        }
    }

    private void OnShot()
    {
        // 일단 쐈다고 신호를 보내고
        JEventManager.SendEvent(new ShotEvent(_shotPoint.position, _shotPoint.rotation));

        // 무기측에서 총알이 있다고 신호를 보내오면 그 때 실행해주는 모양이면 될듯
        if (_hasBullet == true)
        {
            // 기본 상태일 때 바로 조준 상태로 바꿔줌
            if (_isAiming == false)
            {
                StateMachine.ChangeState(AimState);
                _isAiming = true;
            }
            // Pitch 제한
            {
                _amingPitch -= Time.deltaTime * RifleRecoilRate;
                _amingPitch = Mathf.Clamp(_amingPitch, -_amingPitchClamp, _amingPitchClamp);
            }
            // 카메라 흔들림 이펙트
            {
                StartCoroutine(CameraShake());
            }

            Animator.Play("AimIdle_Shoot", 1, 0f);
        }
    }

    private void OnReload()
    {
        JEventManager.SendEvent(new ReloadEvent());
        Animator.Play("Reload", 1, 0f);
    }
    #endregion





    #region EFFECT
    public IEnumerator CameraShake()
    {
        float elapsed = 0f;

        while (elapsed < 0.1f)
        {
            elapsed += Time.deltaTime;

            _cameraShakeOffset.x = Random.Range(-CameraRecoilRate, CameraRecoilRate);
            _cameraShakeOffset.y = Random.Range(-CameraRecoilRate, CameraRecoilRate);

            yield return null;
        }

        elapsed = 0;
        _cameraShakeOffset = Vector3.zero;
    }

    private void PlayerFootStepSound()
    {
        float speed = _currentSpeed * _isMoving;

        if (speed > 0.1f)
        {
            _stepTimer -= Time.deltaTime;

            float interval = 0f;

            if (speed < 1.1f)
            {
                interval = _isAiming == true ? _aimWalkInterval : _walkInterval;
            }
            else if (speed < 3.1f)
            {
                interval = _isAiming == true ? _aimRunInterval : _runInterval;
            }
            else
            {
                interval = _isAiming == true ? _aimDashInterval : _dashInterval;
            }

            if (_stepTimer <= 0f && _controller.isGrounded == true)
            {
                if (_footStepSwitch == true)
                {
                    JAudioManager.Instance.PlaySFX("FootStep_Concrete_1");
                    _footStepSwitch = false;
                }
                else
                {
                    JAudioManager.Instance.PlaySFX("FootStep_Concrete_2");
                    _footStepSwitch = true;
                }

                _stepTimer = interval;
            }
        }
    }
    #endregion
    #endregion
}
