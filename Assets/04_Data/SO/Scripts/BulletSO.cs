using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "Scriptable Objects/Bullet")]
public class BulletSO : ItemSO
{
    [Header("투사체 옵션")]
    [SerializeField] public GameObject Projectile;
    [SerializeField] public float      Atk;
    [SerializeField] public int        NumOfBullet;
    [SerializeField] public int        MagazineQuantity;
}
