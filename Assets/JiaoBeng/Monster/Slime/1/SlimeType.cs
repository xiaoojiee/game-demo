using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlimeStateType
{
    Idle,
    Jump,
    Hurt,
    Die,

}
public enum SlimeActionPriority
{
    None=-1,
    Idle=10,
    Jump=20,
    Hurt=90,
    Die=100
}
public enum SlimeJumpType
{
    Patrol,
    Chase,
    Attack
}
