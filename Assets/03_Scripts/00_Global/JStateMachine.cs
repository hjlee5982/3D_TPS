
public abstract class State<TOwner>
{
    public abstract void Enter (TOwner owner);
    public abstract void Update(TOwner owner);
    public abstract void Exit  (TOwner owner);
}

public class JStateMachine<TOwner>
{
    private TOwner _owner;
    private State<TOwner> _currentState;

    public JStateMachine(TOwner owner)
    {
        _owner = owner;
    }

    public State<TOwner> CurrentState()
    {
        return _currentState;
    }

    public void ChangeState(State<TOwner> nextState)
    {
        _currentState?.Exit(_owner);
        _currentState = nextState;
        _currentState?.Enter(_owner);
    }

    public void Update()
    {
        _currentState?.Update(_owner);
    }
}
