using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "Scriptable Objects/Bullet")]
public class BulletSO : ScriptableObject
{
    // 필드에 있을 때 보여질 모양
    [SerializeField] public GameObject Object;

    [SerializeField] public GameObject Projectile;
    [SerializeField] public float Atk;
}
