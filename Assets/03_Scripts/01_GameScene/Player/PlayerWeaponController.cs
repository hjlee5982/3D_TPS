using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : JBaseClass
{
    #region VARIABLES
    [Header("¹«±â ½½·Ô")]
    public  List<Weapon> WeaponPrefabs       = new List<Weapon>();
    private int          _currentWeaponIndex = 0;

    [Header("ÇöÀç ¹«±â")]
    private Transform _weaponParent  = null;
    private Weapon    _currentWeapon = null;

    [Header("ÃÑ¾Ë ÇÁ¸®Æé")]
    public  Bullet DefaultBulletPrefab  = null;
    private Bullet _currentBulletPrefab = null;

    //[Header("ÃÑ¾Ë Ç®")]
    //private Queue<Bullet> _bulletPool   = new Queue<Bullet>();
    //private Transform     _bulletParent = null;
    //private int           _poolSize     = 10;
    #endregion



    

    #region OVERRIDE
    protected override void InitializeComponents()
    {
    }

    protected override void InitializeTransforms()
    {
        _weaponParent = transform.Find("WeaponSlot");
        // _bulletParent = transform.Find("BulletPool");
    }

    protected override void InitializeValues()
    {
        _currentWeapon = Instantiate(WeaponPrefabs?[_currentWeaponIndex]);
        _currentWeapon.transform.SetParent(_weaponParent);

        _currentBulletPrefab = DefaultBulletPrefab;

        //for (int i = 0; i < _poolSize; ++i)
        //{
        //    Bullet b = Instantiate(DefaultBulletPrefab);

        //    b.CacheBulletPool(this);

        //    b.transform.SetParent(_bulletParent);
        //    b.gameObject.SetActive(false);

        //    _bulletPool.Enqueue(b);
        //}
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
        JEventManager.Subscribe<ShotEvent>(OnShot);
        JEventManager.Subscribe<ReloadEvent>(OnReload);
    }

    private void OnDisable()
    {
        JEventManager.Unsubscribe<ShotEvent>(OnShot);
        JEventManager.Unsubscribe<ReloadEvent>(OnReload);
    }
    #endregion





    #region FUNCTIONS
    private void OnShot(ShotEvent e)
    {
        if(_currentWeapon is RangedWeapon weapon)
        {
            weapon.Shot(_currentBulletPrefab, e);
        }
    }

    private void OnReload(ReloadEvent e)
    {
        if(_currentWeapon is RangedWeapon weapon)
        {
            weapon.Reload(e);
        }
    }





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
