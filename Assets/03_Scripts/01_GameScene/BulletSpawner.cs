using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    #region VARIABLES
    [Header("ÃÑ¾Ë ÇÁ¸®Æé")]
    public GameObject Bullet;
    #endregion





    #region MONOBEHAVIOUR
    private void OnEnable()
    {
        JEventManager.Subscribe<ShotEvent>(SpawnBullet);
    }

    private void OnDisable()
    {
        JEventManager.Unsubscribe<ShotEvent>(SpawnBullet);
    }
    #endregion


    // (15), 4, 26 ±âº»
    // 5, 18 ÃâÇ÷
    // 12,24 µ¶
    // 14, (17) ºù°á
    // 3, (25) È­»ó

    #region FUNCTIONS
    private void SpawnBullet(ShotEvent e)
    {
        GameObject go = Instantiate(Bullet, e.Position, e.Rotation);

        
    }
    #endregion
}
