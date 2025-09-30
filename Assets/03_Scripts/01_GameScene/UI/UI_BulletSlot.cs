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
        JEventManager.Subscribe<BulletChangeEvent>(OnBulletChanged);
    }

    private void OnDisable()
    {
        JEventManager.Unsubscribe<BulletChangeEvent>(OnBulletChanged);
    }
    #endregion





    #region FUNCTIONS
    private bool OnBulletChanged(BulletChangeEvent e)
    {
        for(int i = 0; i < _slots.Count; ++i)
        {
            if(e.Slot == i)
            {
                _slots[i].color = Color.red;
            }
            else
            {
                _slots[i].color = Color.black;
            }
        }

        return true;
    }
    #endregion
}
