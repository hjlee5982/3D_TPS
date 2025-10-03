using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_BulletSlot : JBaseClass
{
    #region VARIABLES
    [Header("선택된 총알 디스플레이")]
    private List<TextMeshProUGUI> _slots = new List<TextMeshProUGUI>();
    #endregion





    #region OVERRIDE
    protected override void InitializeComponents()
    {
        for(int i = 0; i < 3; ++i)
        {
            _slots.Add(transform.Find($"Slot{i}").GetChild(0).GetComponent<TextMeshProUGUI>());
        }
    }

    protected override void InitializeTransforms()
    {

    }

    protected override void InitializeValues()
    {

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

    private void OnEnable()
    {
        JEventManager.Subscribe<BulletSlotChangeEvent>(OnBulletChanged);
    }

    private void OnDisable()
    {
        JEventManager.Unsubscribe<BulletSlotChangeEvent>(OnBulletChanged);
    }
    #endregion





    #region FUNCTIONS
    private bool OnBulletChanged(BulletSlotChangeEvent e)
    {
        for(int i = 0; i < _slots.Count; ++i)
        {
            if(e.CurrentSlotIndex == i)
            {
                _slots[i].color = Color.red;
            }
            else 
            {
                if (e.BulletSlot[i].IsOccupied == true)
                {
                    _slots[i].color = Color.black;
                }
                else
                {
                    _slots[i].color = new Color32(176, 176, 176, 255);
                }
            }
        }

        return true;
    }
    #endregion
}
