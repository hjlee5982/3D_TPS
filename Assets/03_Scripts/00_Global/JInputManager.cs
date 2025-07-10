using System;
using System.Collections.Generic;
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
    [Header("��ǲ�׼� ����")]
    public InputActionAsset InputActions;

    [Header("�⺻ ��ǲ�׼�")]
    private InputAction Move;
    private InputAction Look;
    private InputAction Zoom;
    private InputAction Dash;
    private InputAction Walk;
    private InputAction Jump;
    private InputAction Aimm;
    public  Action<Vector3> OnMove;
    public  Action<Vector2> OnLook;
    public  Action<float>   OnZoom;
    public  Action<bool>    OnDash;
    public  Action<bool>    OnWalk;
    public  Action          OnJump;
    public  Action          OnAimm;

    [Header("Ŀ���� ��ǲ�׼�")]
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

        foreach (RebindableInputAction action in _inputActionDict.Values)
        {
            action.Action.Disable();
        }
    }
    #endregion





    #region FUNCTIONS
    private void InitializeInputAction()
    {
        // Ű������ �ٲ� �� ���� Ű���� �ٷ� �ʱ�ȭ
        {
            // ��ǲ�׼� ���¿� ���ǵǾ� �ִ� �׼��� ������
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
        }
        // Ű������ �ٲ� �� �ִ� Ű���� �ѹ� ���μ� �ʱ�ȭ
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

    public void BindBasicPlayerMovement(Action<Vector3> move, Action<bool> dash, Action<bool> walk, Action jump, Action aimm)
    {
        OnMove += move;
        OnDash += dash;
        OnWalk += walk;
        OnAimm += aimm;
    }

    public void BindBasicCameraMovement(Action<Vector2> look, Action<float> zoom)
    {
        OnLook += look;
        OnZoom += zoom;
    }

    public void BindKey(Action callback, string actionName)
    {
        if (_inputActionDict.TryGetValue(actionName, out RebindableInputAction action) == false)
        {
            Debug.LogError($"[JInputManager] : {actionName} �׼��� �����!!");
            return;
        }

        action.Callback += callback;
    }

    public void OnRebindKeyEvent(string actionName)
    {
        if (_inputActionDict.TryGetValue(actionName, out RebindableInputAction action) == false)
        {
            Debug.LogError($"[JInputManager] : {actionName} �׼��� �����!!");
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