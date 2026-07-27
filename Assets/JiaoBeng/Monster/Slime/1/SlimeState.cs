using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SlimeState
{
    protected SlimeStateMachine machine;
    public SlimeState(SlimeStateMachine machine)
    {
        this.machine=machine;
    }
    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
    
}
