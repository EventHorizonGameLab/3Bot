using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEenemyState 
{
    void Enter();
    void Process();
    void Exit();

}
