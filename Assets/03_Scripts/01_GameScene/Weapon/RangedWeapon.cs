
// 원거리 무기
using UnityEngine;

public abstract class RangedWeapon : Weapon
{
    public abstract bool Shot  (BulletSlotDesc bulletSlot, ShotEvent   e);
    public abstract bool Reload(BulletSlotDesc bulletSlot, ReloadEvent e);

}