using System.Collections;
using TMPro;
using UnityEngine;

public class UI_Magazine : JBaseClass
{
    #region VARIABLES
    [Header("탄창 디스플레이")]
    private TextMeshProUGUI _loadedBulletCount;
    private TextMeshProUGUI _reserveBullet;
    #endregion





    #region OVERRIDE
    protected override void InitializeComponents()
    {
        _loadedBulletCount = transform.Find("LoadedBulletCounter") .GetComponent<TextMeshProUGUI>();
        _reserveBullet     = transform.Find("ReserveBulletCounter").GetComponent<TextMeshProUGUI>();
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
        JEventManager.Subscribe<BulletCountChangeEvent>(OnDisplayValueChanged);
    }

    private void OnDisable()
    {
        JEventManager.Unsubscribe<BulletCountChangeEvent>(OnDisplayValueChanged);
    }
    #endregion





    #region FUNCTIONS
    private bool OnDisplayValueChanged(BulletCountChangeEvent e)
    {
        StartCoroutine(UpdateDelay(e));

        return true;
    }

    private IEnumerator UpdateDelay(BulletCountChangeEvent e)
    {
        yield return new WaitForSeconds(e.Delay);

        _loadedBulletCount.text = e.LoadedBullet .ToString();
        _reserveBullet    .text = e.ReserveBullet.ToString();
    }
    #endregion
}
