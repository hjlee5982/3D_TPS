using System.Collections;
using TMPro;
using UnityEngine;

public class UI_Magazine : JBaseClass
{
    #region VARIABLES
    [Header("탄창 디스플레이")]
    private TextMeshProUGUI _currentBulletCount;
    private TextMeshProUGUI _maxBulletCount;
    #endregion





    #region OVERRIDE
    protected override void InitializeComponents()
    {
        _currentBulletCount = transform.Find("CurrentBulletCounter").GetComponent<TextMeshProUGUI>();
        _maxBulletCount     = transform.Find("MaxBulletCounter")    .GetComponent<TextMeshProUGUI>();
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
    private void OnDisplayValueChanged(BulletCountChangeEvent e)
    {
        StartCoroutine(UpdateDelay(e));
    }

    private IEnumerator UpdateDelay(BulletCountChangeEvent e)
    {
        yield return new WaitForSeconds(e.Delay);

        _currentBulletCount.text = e.CurrentBulletCount.ToString();
        _maxBulletCount    .text = e.MaxBulletCount.ToString();
    }
    #endregion
}
