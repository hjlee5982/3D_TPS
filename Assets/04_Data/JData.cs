using System;

public abstract class BaseData
{
    public string StringKey;
    public int    IntKey;
}



[Serializable]
public class ItemData : BaseData
{
    public enum ItemType
    {
        ShortRange,
        LongRange
    }

    public string   ItemNameKR;
    public string   ItemNameEN;
    public ItemType Type;
    public int      AtkPower;
}
