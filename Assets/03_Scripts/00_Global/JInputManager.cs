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
    [Header("인풋액션 에셋")]
    public InputActionAsset InputActions;

    [Header("인풋액션(키설정 불가")]
    private InputAction Move;
    private InputAction Look;
    public  Action<Vector3> OnMove;
    public  Action<Vector2> OnLook;


    [Header("인풋액션(키설정 가능)")]
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
    }

    private void OnEnable()
    {
        // JEventManager.Subscribe<StartRebindKeyEvent>(OnRebindKeyEvent);

        Move.Enable();
        Look.Enable();

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
            Look = InputActions.FindAction("Look");
        }
        // 키세팅을 바꿀 수 있는 키들은 한번 감싸서 초기화
        {
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

    public void BindBasicMovement(Action<Vector3> move, Action<Vector2> look)
    {
        OnMove += move;
        OnLook += look;
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