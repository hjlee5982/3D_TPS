using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletSlotDesc
{
    public bool     IsOccupied;
    public BulletSO BulletSO;

    public Image    UI;

    public int LoadedBullet;
    public int ReserveBullet;
    public int MagazineCapacity;
}

public class PlayerWeaponController : JBaseClass
{
    #region VARIABLES
    [Header("ÇöÀç ¹«±â")]
    private Weapon _currentWeapon = null;


    [Header("ÃÑ¾Ë SO")]
    public BulletSO DefaultBullet = null;


    [Header("¹«±â ½½·Ô")]
    private List<BulletSlotDesc> _bulletSlots      = new List<BulletSlotDesc>();
    private int                  _currentSlotIndex = 0;
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
        // Slot_1
        {
            BulletSlotDesc desc = new BulletSlotDesc();
            {
                desc.IsOccupied         = true;
                desc.BulletSO           = DefaultBullet;
                desc.LoadedBullet = 30;
                desc.ReserveBullet        = 90;
                desc.MagazineCapacity   = 30;
            }
            _bulletSlots.Add(desc);
        }
        // Slot_2
        {
            BulletSlotDesc desc = new BulletSlotDesc();
            {
                desc.IsOccupied = false;
                desc.BulletSO   = null;
            }
            _bulletSlots.Add(desc);
        }
        // Slot_3
        {
            BulletSlotDesc desc = new BulletSlotDesc();
            {
                desc.IsOccupied = false;
                desc.BulletSO   = null;
            }
            _bulletSlots.Add(desc);
        }
        JEventManager.SendEvent(new BulletCountChangeEvent(_bulletSlots[_currentSlotIndex].LoadedBullet, _bulletSlots[_currentSlotIndex].ReserveBullet));
        JEventManager.SendEvent(new BulletSlotChangeEvent(_currentSlotIndex, _bulletSlots));
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
    public void GetItem(ItemSO item)
    {
        switch(item)
        {
            case BulletSO so:

                for(int i = 0; i < _bulletSlots.Count; ++i)
                {
                    if (_bulletSlots[i].IsOccupied == false)
                    {
                        _bulletSlots[i].BulletSO = so;
                        _bulletSlots[i].IsOccupied = true;

                        _bulletSlots[i].ReserveBullet      = so.NumOfBullet;
                        _bulletSlots[i].MagazineCapacity = so.MagazineQuantity;

                        _bulletSlots[i].ReserveBullet       -= _bulletSlots[i].MagazineCapacity;
                        _bulletSlots[i].LoadedBullet = _bulletSlots[i].MagazineCapacity;

                        JEventManager.SendEvent(new BulletCountChangeEvent(_bulletSlots[i].LoadedBullet, _bulletSlots[i].ReserveBullet));
                        JEventManager.SendEvent(new BulletSlotChangeEvent(_currentSlotIndex, _bulletSlots));

                        break;
                    }
                }

                break;

            case ArmorSO so:
                break;
        }
    }

    private bool OnShot(ShotEvent e)
    {
        if(_currentWeapon is RangedWeapon weapon)
        {
            return weapon.Shot(_bulletSlots[_currentSlotIndex], e);
        }

        return false;
    }

    private bool OnReload(ReloadEvent e)
    {
        if(_currentWeapon is RangedWeapon weapon)
        {
            return weapon.Reload(_bulletSlots[_currentSlotIndex], e);
        }

        return false;
    }
    
    private bool OnBulletChange(BulletChangeEvent e)
    {
        if(e.Slot == _currentSlotIndex)
        {
            return false;
        }
        else if (_bulletSlots[e.Slot].IsOccupied == false)
        {
            return false;
        }
        else
        {
            _currentSlotIndex = e.Slot;

            JEventManager.SendEvent(new BulletSlotChangeEvent(_currentSlotIndex, _bulletSlots));

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
