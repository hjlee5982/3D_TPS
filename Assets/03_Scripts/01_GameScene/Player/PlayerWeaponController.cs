using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : JBaseClass
{
    #region VARIABLES
    [Header("ÇöÀç ¹«±â")]
    private Weapon _currentWeapon     = null;
    private int    _currentWeaponSlot = 0;

    [Header("ÃÑ¾Ë SO")]
    public  List<BulletSO> BulletSOs            = new List<BulletSO>();
    private BulletSO       _currentBulletPrefab = null;
    #endregion



    

    #region OVERRIDE
    protected override void InitializeComponents()
    {
        _currentWeapon = GetComponent<Weapon>();
    }

    protected override void InitializeTransforms()
    {
    }

    protected override void InitializeValues()
    {
        _currentBulletPrefab = BulletSOs[0];        
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
        InitializeInputActions();
    }

    private void OnEnable()
    {
        JEventManager.Subscribe<ShotEvent>(OnShot);
        JEventManager.Subscribe<ReloadEvent>(OnReload);
        JEventManager.Subscribe<BulletChangeEvent>(OnBulletChange);
    }

    private void OnDisable()
    {
        JEventManager.Unsubscribe<ShotEvent>(OnShot);
        JEventManager.Unsubscribe<ReloadEvent>(OnReload);
        JEventManager.Unsubscribe<BulletChangeEvent>(OnBulletChange);
    }
    #endregion





    #region FUNCTIONS
    private void InitializeInputActions()
    {
    }

    private bool OnShot(ShotEvent e)
    {
        if(_currentWeapon is RangedWeapon weapon)
        {
            return weapon.Shot(_currentBulletPrefab, e);
        }

        return false;
    }

    private bool OnReload(ReloadEvent e)
    {
        if(_currentWeapon is RangedWeapon weapon)
        {
            return weapon.Reload(e);
        }

        return false;
    }
    
    private bool OnBulletChange(BulletChangeEvent e)
    {
        if(e.Slot == _currentWeaponSlot)
        {
            return false;
        }
        else
        {
            _currentWeaponSlot   = e.Slot;
            _currentBulletPrefab = BulletSOs[_currentWeaponSlot];

            JAudioManager.Instance.PlaySFX("Bullet_Change");

            return true;
        }
    }

    

    //[Header("ÃÑ¾Ë Ç®")]
    //private Queue<Bullet> _bulletPool   = new Queue<Bullet>();
    //private Transform     _bulletParent = null;
    //private int           _poolSize     = 10;

    //for (int i = 0; i < _poolSize; ++i)
    //{
    //    Bullet b = Instantiate(DefaultBulletPrefab);

    //    b.CacheBulletPool(this);

    //    b.transform.SetParent(_bulletParent);
    //    b.gameObject.SetActive(false);

    //    _bulletPool.Enqueue(b);
    //}

    //#region BULLETPOOL
    //private Bullet GetBullet(Vector3 position, Quaternion rotation)
    //{
    //    Bullet b;

    //    if (_bulletPool.Count > 0)
    //    { 
    //        b = _bulletPool.Dequeue();
    //    }
    //    else
    //    {
    //        b = Instantiate(_currentBulletPrefab);
    //        b.transform.SetParent(_bulletParent);
    //    }

    //    b.transform.position = position;
    //    b.transform.rotation = rotation;

    //    return b;
    //}

    //public void ReturnBullet(Bullet bullet)
    //{
    //    _bulletPool.Enqueue(bullet);
    //}
    //#endregion
    #endregion
}
