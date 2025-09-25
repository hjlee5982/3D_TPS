
public abstract class RangedWeapon : Weapon
{
    public abstract void Shot(Bullet bulletPrefab, ShotEvent e);
    public abstract void Reload(ReloadEvent e);
}