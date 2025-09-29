using UnityEngine;

public class PlayerWeaponController : JBaseClass
{
    #region VARIABLES
    [Header("ÇöÀç ¹«±â")]
    private Weapon _currentWeapon = null;

    [Header("ÃÑ¾Ë ÇÁ¸®Æé")]
    public  Bullet DefaultBulletPrefab  = null;
    private Bullet _currentBulletPrefab = null;

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
        _currentBulletPrefab = DefaultBulletPrefab;        
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
    private bool OnShot(ShotEvent e)
    {
        if(_currentWeapon is RangedWeapon weapon)
        {
            weapon.Shot(_currentBulletPrefab, e);
        }

        return true;
    }

    private bool OnReload(ReloadEvent e)
    {
        if(_currentWeapon is RangedWeapon weapon)
        {
            weapon.Reload(e);
        }

        return true;
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
