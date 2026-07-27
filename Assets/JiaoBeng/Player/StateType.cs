using UnityEngine;

public enum StateType
{
    Idle, Run, Jump, DoubleJump, Fall, Sprint, SprintAttack, Attack
}

public enum ActionPriority
{
    None = -1,
    Idle = 10,
    Run = 20,
    Fall = 30,
    Jump = 40,
    DoubleJump = 42,
    Sprint = 45,
    Attack = 50,
    SprintAttack = 55
}
