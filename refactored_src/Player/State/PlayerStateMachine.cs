public class PlayerStateMachine
{
    public PlayerState currentState { get; private set; }

    public void Initialize(PlayerState _state)
    {
        currentState = _state;
        currentState.Enter();
    }

    public void ChangeState(PlayerState _newstate)
    {
        currentState.Exit();
        currentState = _newstate;
        currentState.Enter();
    }
}
