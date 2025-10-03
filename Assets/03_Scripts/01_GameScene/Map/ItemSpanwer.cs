using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemSpanwer : JBaseClass
{
    #region VARIABLES
    [Header("스폰 할 아이템SO")]
    public List<ItemSO> Items = new List<ItemSO>();

    [Header("스폰 할 위치들")]
    public  Transform       SpawnPointParent = null;
    private List<Transform> SpawnPoints      = new List<Transform>();
    #endregion





    #region OVERRIDE
    protected override void InitializeComponents()
    {
    }

    protected override void InitializeTransforms()
    {
        foreach(Transform t in SpawnPointParent)
        {
            SpawnPoints.Add(t);
        }
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) == true)
        {
            SpawnItem();
        }
    }
    #endregion





    #region FUNCTIONS
    public void SpawnItem()
    {
        Transform parent = SpawnPoints[Random.Range(0, SpawnPoints.Count)];

        Vector3 pos = parent.position + new Vector3(Random.Range(0, 6), 0, Random.Range(0, 6));

        int itemIdx = Random.Range(0, Items.Count);

        ItemObject go = Instantiate(Items[itemIdx].Object, pos, parent.rotation).GetComponent<ItemObject>();
        {
            go.SetItemSO(Items[itemIdx]);
        }
    }
    #endregion
}
