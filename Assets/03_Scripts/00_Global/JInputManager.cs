using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindableInputAction
{
    public InputAction Action;
    public InputActionReference ActionRef;
    public Action Callback;

    public void BindAction(InputAction inputAction)
    {
        Action = inputAction;
        ActionRef = InputActionReference.Create(Action);

        Action.performed += ctx => Callback?.Invoke();
    }
}

public class JInputManager : MonoBehaviour
{
    #region SINGLETON
    public static JInputManager Instance { get; private set; }

    private bool SingletonInitialize(bool dontDestroy = true)
    {
        if (Instance == null)
        {
            Instance = this;

            if (dontDestroy == true)
            {
                DontDestroyOnLoad(gameObject);
            }
            return true;
        }
        else
        {
            Destroy(gameObject);
            return false;
        }
    }
    #endregion





    #region VARIABLES
    [Header("인풋액션 에셋")]
    public InputActionAsset InputActions;

    [Header("기본 인풋액션")]
    private InputAction Move;
    private InputAction Look;
    private InputAction Zoom;
    private InputAction Dash;
    private InputAction Walk;
    private InputAction Jump;
    private InputAction Aimm;
    private InputAction Shot;
    private InputAction Reload;
    public  Action<Vector3> OnMove;
    public  Action<Vector2> OnLook;
    public  Action<float>   OnZoom;
    public  Action<bool>    OnDash;
    public  Action<bool>    OnWalk;
    public  Action          OnJump;
    public  Action          OnAimm;
    public  Action<bool>    OnShot;
    public  Action    OnReload;

    [Header("홀딩 인풋 플래그")]
    private bool  _isHolding     = false;
    private float _interval      = 0.1f;
    private float _nextInputTime = 0f;
    
    [Header("커스텀 인풋액션")]
    public Dictionary<string, RebindableInputAction> _inputActionDict = new Dictionary<string, RebindableInputAction>();
    #endregion

    





    #region MONOBEHAVIOUR
    private void Awake()
    {
        if(SingletonInitialize() == false)
        {
            return;
        }

        InitializeInputAction();
    }

    private void Update()
    {
        Vector3 move = Move.ReadValue<Vector3>();

        if (move != Vector3.zero)
        {
            OnMove?.Invoke(move);
        }

        Vector2 look = Look.ReadValue<Vector2>();

        if(look != Vector2.zero)
        {
            OnLook?.Invoke(look);
        }

        float zoom = Zoom.ReadValue<float>();

        if (zoom != 0f)
        {
            OnZoom?.Invoke(zoom);
        }

        if(_isHolding == true && Time.time >= _nextInputTime)
        {
            OnShot?.Invoke(true);
            _nextInputTime = Time.time + _interval;
        }
    }

    private void OnEnable()
    {
        // JEventManager.Subscribe<StartRebindKeyEvent>(OnRebindKeyEvent);

        Move.Enable();
        Look.Enable();
        Zoom.Enable();
        Dash.Enable();
        Walk.Enable();
        Jump.Enable();
        Shot.Enable();
        Reload.Enable();

        foreach(RebindableInputAction action in _inputActionDict.Values)
        {
            action.Action.Enable();
        }
    }

    private void OnDisable()
    {
        // JEventManager.Unsubscribe<StartRebindKeyEvent>(OnRebindKeyEvent);

        Move.Disable();
        Look.Disable();
        Zoom.Disable();
        Dash.Disable();
        Walk.Disable();
        Jump.Disable();
        Shot.Disable();
        Reload.Disable();

        foreach (RebindableInputAction action in _inputActionDict.Values)
        {
            action.Action.Disable();
        }
    }
    #endregion





    #region FUNCTIONS
    private void InitializeInputAction()
    {
        // 키세팅을 바꿀 수 없는 키들은 바로 초기화
        {
            // 인풋액션 에셋에 정의되어 있는 액션을 가져옴
            Move = InputActions.FindAction("Move");
            Move.canceled += ctx => OnMove?.Invoke(Vector3.zero);

            Look = InputActions.FindAction("Look");
            Look.canceled += ctx => OnLook?.Invoke(Vector2.zero);

            Zoom = InputActions.FindAction("Zoom");
            Zoom.canceled += ctx => OnZoom?.Invoke(0f);

            Dash = InputActions.FindAction("Dash");
            Dash.performed += ctx => OnDash?.Invoke(true);
            Dash.canceled  += ctx => OnDash?.Invoke(false);

            Walk = InputActions.FindAction("Walk");
            Walk.performed += ctx => OnWalk?.Invoke(true);
            Walk.canceled  += ctx => OnWalk?.Invoke(false);

            Jump = InputActions.FindAction("Jump");
            Jump.performed += ctx => OnJump?.Invoke();

            Aimm = InputActions.FindAction("Aiming");
            Aimm.performed += ctx => OnAimm?.Invoke();
            
            Reload = InputActions.FindAction("Reload");
            Reload.started  += ctx => OnReload?.Invoke();

            Shot = InputActions.FindAction("Shot");
            Shot.started  += ctx => { _isHolding = true; };
            Shot.canceled += ctx => { _isHolding = false; OnShot?.Invoke(false); };


        }
        // 키세팅을 바꿀 수 있는 키들은 한번 감싸서 초기화
        {
            RebindableInputAction aiming = new RebindableInputAction();
            {
                aiming.BindAction(InputActions.FindAction("Aiming"));
                _inputActionDict["Aiming"] = aiming;
            }
            RebindableInputAction jump = new RebindableInputAction();
            {
                jump.BindAction(InputActions.FindAction("Jump"));
                _inputActionDict["Jump"] = jump;
            }
            RebindableInputAction interaction = new RebindableInputAction();
            {
                interaction.BindAction(InputActions.FindAction("Interaction"));
                _inputActionDict["Interaction"] = interaction;
            }
            RebindableInputAction inventoryOpen = new RebindableInputAction();
            {
                inventoryOpen.BindAction(InputActions.FindAction("InventoryOpen"));
                _inputActionDict["InventoryOpen"] = inventoryOpen;
            }
        }
    }
    
    public void BindButtonAction(Action<Vector3> move, Action<bool> dash, Action<bool> walk, Action jump, Action aimm, Action Reload)
    {
        OnMove   += move;
        OnDash   += dash;
        OnWalk   += walk;
        OnJump   += jump;
        OnAimm   += aimm;
        OnReload += Reload;
    }

    public void BindHoldingAction(Action<bool> hold, float interval)
    {
        OnShot += hold;
        _interval = interval;
    }
    
    public void BindCameraAction(Action<Vector2> look, Action<float> zoom)
    {
        OnLook += look;
        OnZoom += zoom;
    }

    public void BindKey(Action callback, string actionName)
    {
        if (_inputActionDict.TryGetValue(actionName, out RebindableInputAction action) == false)
        {
            Debug.LogError($"[JInputManager] : {actionName} 액션이 없어요!!");
            return;
        }

        action.Callback += callback;
    }

    public void OnRebindKeyEvent(string actionName)
    {
        if (_inputActionDict.TryGetValue(actionName, out RebindableInputAction action) == false)
        {
            Debug.LogError($"[JInputManager] : {actionName} 액션이 없어요!!");
            return;
        }

        InputAction          inputAction    = action.Action;
        InputActionReference inputActionRef = action.ActionRef;

        if(inputAction.enabled)
        {
            inputAction.Disable();
        }

        InputActionRebindingExtensions.RebindingOperation rebindOperation;

        rebindOperation = inputActionRef.action.PerformInteractiveRebinding()
            .WithTargetBinding(0)
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(op =>
            {
                op.Dispose();
                inputAction.Enable();

                InputBinding binding = inputActionRef.action.bindings[0];
                String key = InputControlPath.ToHumanReadableString(
                    binding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice
                    );

                // JEventManager.SendEvent(new CompleteRebindKeyEvent(key));
            })
            .Start();
    }
    #endregion
}