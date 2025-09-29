using System;

public abstract class BaseData
{
    public string StringKey;
    public int    IntKey;
}

[Serializable]
public class BulletData : BaseData
{
    public float Atk;
}