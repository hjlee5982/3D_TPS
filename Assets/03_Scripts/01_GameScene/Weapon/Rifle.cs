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

    protected override bool RequestShot(RequestShotEvent e)
    {
        if (_currentBulletCount != 0)
        {
            JAudioManager.Instance.PlaySFX("Rifle_Shot");
            return true;
        }
        else
        {
            JAudioManager.Instance.PlaySFX("Rifle_Empty");
            return false;
        }
    }

    protected override bool RequestReload(RequestReloadEvent e)
    {
        if (_currentBulletCount != MaxBulletCount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void Shot(BulletSO bulletPrefab, ShotEvent e)
    {
        var projectile = bulletPrefab.Projectile;

        Instantiate(projectile, e.Position, e.Rotation);

        JEventManager.SendEvent(new BulletCountChangeEvent(MaxBulletCount, --_currentBulletCount));
    }

    public override void Reload(ReloadEvent e)
    {
        _currentBulletCount = 30;
        JEventManager.SendEvent(new BulletCountChangeEvent(MaxBulletCount, _currentBulletCount, e.ReloadDelay));
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

    private void OnEnable()
    {
        JEventManager.Subscribe<RequestShotEvent>(RequestShot);   
        JEventManager.Subscribe<RequestReloadEvent>(RequestReload);
    }

    private void OnDisable()
    {
        JEventManager.Unsubscribe<RequestShotEvent>(RequestShot);
        JEventManager.Unsubscribe<RequestReloadEvent>(RequestReload);
    }
    #endregion





    #region FUNCTIONS
    #endregion
}
