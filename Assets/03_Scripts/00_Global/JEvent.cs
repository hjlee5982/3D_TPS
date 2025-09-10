using UnityEngine;


public class ButtonClickEvent
{
    public int Value;

    public ButtonClickEvent(int valuie)
    {
        Value = valuie;
    }
}

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