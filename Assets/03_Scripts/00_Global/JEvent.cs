using System.Collections.Generic;
using UnityEngine;

public class SwitchAimingModeEvent
{
    public bool Value;

    public SwitchAimingModeEvent(bool value)
    {
        Value = value;
    }
}

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

public class ReloadEvent
{
    public float ReloadDelay;

    public ReloadEvent(float reloadDelay)
    {
        ReloadDelay = reloadDelay;
    }
}

public class BulletChangeEvent
{
    public int Slot;

    public BulletChangeEvent(int slot)
    {
        Slot = slot;
    }
}





public class BulletCountChangeEvent
{
    public int   ReserveBullet;
    public int   LoadedBullet;
    public float Delay;

    public BulletCountChangeEvent(int loadedBullet, int reserveBullet, float delay = 0f)
    {
        LoadedBullet  = loadedBullet;
        ReserveBullet = reserveBullet;
        Delay         = delay;
    }
}

public class BulletSlotChangeEvent
{
    public int                  CurrentSlotIndex;
    public List<BulletSlotDesc> BulletSlot = new List<BulletSlotDesc>();

    public BulletSlotChangeEvent(int currentSlotIndex, List<BulletSlotDesc> bulletSlot)
    {
        CurrentSlotIndex = currentSlotIndex;
        BulletSlot       = bulletSlot;
    }
}
