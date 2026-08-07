using UnityEngine;

public abstract class PlayerState
{
    protected PlayerStateMachine machine;
    protected Rigidbody rb;

    public PlayerState(PlayerStateMachine machine)
    {
        this.machine = machine;
        this.rb = machine.rb;
    }

    public abstract StateType Type { get; }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
}
