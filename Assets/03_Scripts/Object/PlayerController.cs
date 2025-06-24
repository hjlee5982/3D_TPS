using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region VARIABLES
    [Header("ī�޶�")]
    private Camera _playerCamera;

    [Header("�÷��̾� �ӵ�")]
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
        // ������ dir�� 0,0,1 | -1,0,0 | 0,0,-1 | 1,0,0 �� normalized�� ������ ����
        // �̰� �÷��̾� Ʈ������ �������� �ٲ���� ��
        Vector3 worldDir = transform.TransformDirection(dir);

        transform.position += worldDir * Time.deltaTime * _playerSpeed;
    }

    private void OnLook(Vector2 delta)
    {
        transform.Rotate(Vector3.up * delta.x * 3);
    }

    private void OnJump()
    {   
        Debug.Log("Jump �Է�");
    }
    #endregion
}
