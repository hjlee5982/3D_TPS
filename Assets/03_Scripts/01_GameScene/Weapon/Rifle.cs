using UnityEngine;

public class Rifle : RangedWeapon
{
    #region VARIABLES
    [Header("무기 정보")]
    public  int MaxBulletCount      = 30; // 최대 탄창
    private int _currentBulletCount = 30; // 현재 탄창
    #endregion




    #region OVERRIDE
    protected override void InitializeComponents()
    {
    }

    protected override void InitializeTransforms()
    {
    }

    protected override void InitializeValues()
    {
        JEventManager.SendEvent(new BulletCountChangeEvent(MaxBulletCount, _currentBulletCount));
    }
    #endregion





    #region INTERFACE
    public override void Shot(Bullet bulletPrefab, ShotEvent e)
    {
        if (_currentBulletCount == 0)
        {
            JAudioManager.Instance.PlaySFX("Rifle_Empty");
        }
        else
        {
            Instantiate(bulletPrefab, e.Position, e.Rotation);

            JEventManager.SendEvent(new BulletCountChangeEvent(MaxBulletCount, --_currentBulletCount));
            JAudioManager.Instance.PlaySFX("Rifle_Shot");

            if (_currentBulletCount == 0)
            {
                JEventManager.SendEvent(new BulletCountCheckEvent(false));
            }
        }
    }

    public override void Reload()
    {
        _currentBulletCount = 30;
        JEventManager.SendEvent(new BulletCountChangeEvent(MaxBulletCount, _currentBulletCount));
        JEventManager.SendEvent(new BulletCountCheckEvent(true));
        JAudioManager.Instance.PlaySFX("Rifle_Reload");
    }
    #endregion





    #region MONOBEHAVIOUR
    private void Awake()
    {
        InitializeComponents();
        InitializeTransforms();
    }

    private void Start()
    {
        InitializeValues();
    }
    #endregion





    #region FUNCTIONS
    #endregion
}
