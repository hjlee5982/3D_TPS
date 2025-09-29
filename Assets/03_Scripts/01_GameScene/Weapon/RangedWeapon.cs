
// 원거리 무기
public abstract class RangedWeapon : Weapon
{
    public    int MaxBulletCount      = 30; // 최대 탄창
    protected int _currentBulletCount = 30; // 현재 탄창

    protected abstract bool RequestShot(RequestShotEvent e);
    protected abstract bool RequestReload(RequestReloadEvent e);

    public abstract void Shot(Bullet bulletPrefab, ShotEvent e);
    public abstract void Reload(ReloadEvent e);

}