using UnityEngine;

public abstract class PlayerState
{
    protected PlayerStateMachine machine;

    public PlayerState(PlayerStateMachine machine)
    {
        this.machine = machine;
    }

    public abstract StateType Type { get; }

    // 进入状态时调用一次
    public virtual void Enter() { }
    // 每帧调用
    public virtual void Update() { }
    // 每物理帧调用
    public virtual void FixedUpdate() { }
    // 离开状态时调用一次
    public virtual void Exit() { }
}
