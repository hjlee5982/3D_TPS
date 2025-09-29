using UnityEngine;

#region PLAYER
public class SwitchAimingModeEvent
{
    public bool Value;

    public SwitchAimingModeEvent(bool value)
    {
        Value = value;
    }
}

public class RequestShotEvent { }
public class ShotEvent
{
    public Vector3 Position;
    public Quaternion Rotation;
    
    public ShotEvent(Vector3 shotPosition, Quaternion shotRotation)
    {
        Position = shotPosition;
        Rotation = shotRotation;
    }
}

public class RequestReloadEvent{ }
public class ReloadEvent
{
    public float ReloadDelay;

    public ReloadEvent(float reloadDelay)
    {
        ReloadDelay = reloadDelay;
    }
}
#endregion





#region UI
public class BulletCountChangeEvent
{
    public int   MaxBulletCount;
    public int   CurrentBulletCount;
    public float Delay;

    public BulletCountChangeEvent(int maxBulletCount, int currentBulletCount, float delay = 0f)
    {
        MaxBulletCount     = maxBulletCount;
        CurrentBulletCount = currentBulletCount;
        Delay              = delay;
    }
}
#endregion