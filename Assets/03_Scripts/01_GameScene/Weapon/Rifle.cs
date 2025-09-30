using UnityEngine;

public class Rifle : RangedWeapon
{
    #region VARIABLES
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

    public override bool Shot(BulletSO bulletPrefab, ShotEvent e)
    {
        if (_currentBulletCount != 0)
        {
            Instantiate(bulletPrefab.Projectile, e.Position, e.Rotation);

            JEventManager.SendEvent(new BulletCountChangeEvent(MaxBulletCount, --_currentBulletCount));
            JAudioManager.Instance.PlaySFX("Rifle_Shot");
            return true;
        }
        else
        {
            JAudioManager.Instance.PlaySFX("Rifle_Empty");
            return false;
        }
    }

    public override bool Reload(ReloadEvent e)
    {
        if (_currentBulletCount != MaxBulletCount)
        {
            _currentBulletCount = 30;
            JEventManager.SendEvent(new BulletCountChangeEvent(MaxBulletCount, _currentBulletCount, e.ReloadDelay));
            JAudioManager.Instance.PlaySFX("Rifle_Reload");

            return true;
        }
        else
        {
            return false;
        }
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
