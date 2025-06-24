using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region VARIABLES
    [Header("카메라")]
    private Camera _playerCamera;

    [Header("플레이어 속도")]
    private float _playerSpeed = 1f;
    #endregion

    #region MONOBEHAVIOUR
    private void Awake()
    {
        _playerCamera = transform.Find("PlayerCamera").GetComponent<Camera>();
    }

    private void Start()
    {
        BindInput();   
    }
    #endregion





    #region FUNCTIONS
    private void BindInput()
    {
        JInputManager.Instance.BindBasicMovement(OnMove, OnLook);
        JInputManager.Instance.BindKey(OnJump, "Jump");
    }

    private void OnMove(Vector3 dir)
    {
        // 들어오는 dir은 0,0,1 | -1,0,0 | 0,0,-1 | 1,0,0 이 normalized된 절댓값이 들어옴
        // 이걸 플레이어 트랜스폼 기준으로 바꿔줘야 함
        Vector3 worldDir = transform.TransformDirection(dir);

        transform.position += worldDir * Time.deltaTime * _playerSpeed;
    }

    private void OnLook(Vector2 delta)
    {
        transform.Rotate(Vector3.up * delta.x * 3);
    }

    private void OnJump()
    {   
        Debug.Log("Jump 입력");
    }
    #endregion
}
