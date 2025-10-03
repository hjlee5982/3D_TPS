using UnityEngine;

public abstract class ItemSO : ScriptableObject
{
    [Header("필드에 있을 때 보여질 오브젝트 프리펩")]
    [SerializeField] public GameObject Object;

    [Header("아이템 이름")]
    [SerializeField] public string Name;
}
