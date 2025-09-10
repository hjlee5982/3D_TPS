using UnityEngine;

public class JUIManager : MonoBehaviour
{
    #region VARIABLES
    [Header("UI 요소")]
    private GameObject _crossHair;
    #endregion





    #region MONOBEHAVIOUR
    private void Awake()
    {
        _crossHair = transform.Find("CrossHair").gameObject;
        _crossHair.SetActive(false);
    }

    private void OnEnable()
    {
        // 이벤트 수신 등록
        JEventManager.Subscribe<SwitchAimingModeEvent>(SwitchCrossHair);
    }

    private void OnDisable()
    {
        // 이벤트 수신 해제
        JEventManager.Unsubscribe<SwitchAimingModeEvent>(SwitchCrossHair);

    }
    #endregion





    #region FUNCTIONS
    private void SwitchCrossHair(SwitchAimingModeEvent e)
    {
        _crossHair.SetActive(e.Value);
    }
    #endregion
}
