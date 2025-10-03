using System.Collections.Generic;
using UnityEngine;

public class PlayerItemController : JBaseClass
{
    #region VARIABLES
    [Header("»πµÊ«— æ∆¿Ã≈€SO")]
    private Dictionary<string, ItemSO> _itemSOs;
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
    #endregion






    #region MONOBEHAVIOUR
    #endregion




    #region FUNCTIONS
    public void GetItemSO(ItemSO itemSO)
    {
        _itemSOs.Add(itemSO.Name, itemSO);
    }
    #endregion
}
