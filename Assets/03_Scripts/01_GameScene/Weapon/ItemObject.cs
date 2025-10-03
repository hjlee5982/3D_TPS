using UnityEngine;

public class ItemObject : MonoBehaviour
{
    #region VARIABLES
    [Header("æ∆¿Ã≈€ SO")]
    private ItemSO _itemSO = null;
    #endregion





    #region MONOBEHAVIOUR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) == true)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag == "Player")
        {
            Debug.Log($"æ∆¿Ã≈€ »πµÊ : {_itemSO.Name}");

            PlayerWeaponController pwc = collision.transform.GetComponent<PlayerWeaponController>();
            {
                pwc.GetItem(_itemSO);
            }

            Destroy(gameObject);
        }
    }
    #endregion





    #region FUNCTIONS
    public void SetItemSO(ItemSO itemSO)
    {
        _itemSO = itemSO; 
    }
    #endregion
}
