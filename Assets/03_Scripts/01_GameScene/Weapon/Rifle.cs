using System.Collections;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Rifle : RangedWeapon
{
    #region VARIABLES
    [Header("재장전 소리 딜레이")]
    private bool _emptyInterval = false;
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
    }

    public override bool Shot(BulletSlotDesc bulletSlot, ShotEvent e)
    {
        if (bulletSlot.LoadedBullet != 0)
        {
            Instantiate(bulletSlot.BulletSO.Projectile, e.Position, e.Rotation);

            JEventManager.SendEvent(new BulletCountChangeEvent(--bulletSlot.LoadedBullet, bulletSlot.ReserveBullet));
            JAudioManager.Instance.PlaySFX("Rifle_Shot");

            return true;
        }
        else
        {
            if(_emptyInterval == false)
            {
                _emptyInterval = true;

                JAudioManager.Instance.PlaySFX("Rifle_Empty");

                Invoke("EmptySoundDelay", 0.5f);
            }
            return false;
        }
    }

    public override bool Reload(BulletSlotDesc bulletSlot, ReloadEvent e)
    {
        if (bulletSlot.ReserveBullet == 0)
        {
            if (_emptyInterval == false)
            {
                _emptyInterval = true;

                JAudioManager.Instance.PlaySFX("Rifle_Empty");

                Invoke("EmptySoundDelay", 0.5f);
            }

            return false;
        }
        else
        {
            if (bulletSlot.LoadedBullet != bulletSlot.MagazineCapacity)
            {
                int needed = bulletSlot.MagazineCapacity - bulletSlot.LoadedBullet;
                int toLoad = Mathf.Min(needed, bulletSlot.ReserveBullet);

                bulletSlot.LoadedBullet  += toLoad;
                bulletSlot.ReserveBullet -= toLoad;

                JEventManager.SendEvent(new BulletCountChangeEvent(bulletSlot.LoadedBullet, bulletSlot.ReserveBullet, e.ReloadDelay));
                JAudioManager.Instance.PlaySFX("Rifle_Reload");

                return true;
            }
            else
            {
                return false;
            }
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
    private void EmptySoundDelay()
    {
        _emptyInterval = false;
    }
    #endregion
}
