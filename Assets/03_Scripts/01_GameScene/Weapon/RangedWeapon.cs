
// 원거리 무기
using UnityEngine;

public abstract class RangedWeapon : Weapon
{
    public    int MaxBulletCount      = 30; // 최대 탄창
    protected int _currentBulletCount = 30; // 현재 탄창

    public abstract bool Shot(BulletSO bulletPrefab, ShotEvent e);
    public abstract bool Reload(ReloadEvent e);

}