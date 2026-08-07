public enum StateType
{
    Idle, Walk, Run, Jump, Attack, Roll
}

public enum ActionPriority
{
    None = -1,
    Idle  = 10,
    Walk  = 20,
    Run   = 30,
    Jump  = 35,
    Attack = 45,
    Roll  = 50
}
