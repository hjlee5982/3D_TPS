using UnityEngine;

[CreateAssetMenu(fileName = "ArmorSO", menuName = "Scriptable Objects/ArmorSO")]
public class ArmorSO : ItemSO
{
    [Header("规绢备 可记")]
    [SerializeField] public float Armor;
}
