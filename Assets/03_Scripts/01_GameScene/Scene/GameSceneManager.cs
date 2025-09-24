using UnityEngine;

public class GameSceneManager : JBaseClass
{
    #region OVERRIDE
    protected override void InitializeComponents()
    {
    }

    protected override void InitializeTransforms()
    {
    }

    protected override void InitializeValues()
    {
    }
    #endregion





    #region MONOBEHAVIOUR
    private void Start()
    {
        JCursorManager.LockCursor();

        //JAudioManager.Instance.PlayBGM("BGM_0");
    }
    #endregion
}
    