
public class PlayerIdleState : State<PlayerController>
{
    public override void Enter(PlayerController player)
    {
    }

    public override void Update(PlayerController player) 
    {
        player.BasicCharacterMove();
        player.BasicCameraMove();
    }

    public override void Exit(PlayerController player) 
    {
    }
}

public class PlayerAimingState : State<PlayerController>
{
    public override void Enter(PlayerController player)
    {
        JEventManager.SendEvent(new SwitchAimingModeEvent(true));

        player.Animator.SetBool("IsAiming", true);
    }

    public override void Update(PlayerController player)
    {
        player.AmingCharacterMove();
        player.AmingCameraMove();
        player.AmingTransformAdjust();
    }

    public override void Exit(PlayerController player)
    {
        JEventManager.SendEvent(new SwitchAimingModeEvent(false));

        player.Animator.SetBool("IsAiming", false);
    }
}