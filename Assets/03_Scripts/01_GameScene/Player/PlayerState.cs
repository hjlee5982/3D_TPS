
public class PlayerIdleState : State<PlayerInputController>
{
    public override void Enter(PlayerInputController player)
    {
    }

    public override void Update(PlayerInputController player) 
    {
        player.BasicCharacterMove();
        player.BasicCameraMove();
    }

    public override void Exit(PlayerInputController player) 
    {
    }
}

public class PlayerAimingState : State<PlayerInputController>
{
    public override void Enter(PlayerInputController player)
    {
        JEventManager.SendEvent(new SwitchAimingModeEvent(true));

        player.Animator.SetBool("IsAiming", true);
    }

    public override void Update(PlayerInputController player)
    {
        player.AmingCharacterMove();
        player.AmingCameraMove();
        player.AmingTransformAdjust();
    }

    public override void Exit(PlayerInputController player)
    {
        JEventManager.SendEvent(new SwitchAimingModeEvent(false));

        player.Animator.SetBool("IsAiming", false);
    }
}