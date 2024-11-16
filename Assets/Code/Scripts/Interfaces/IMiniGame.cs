using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMiniGame
{
    bool IsEnemy { get; }
    bool IsDoor { get; }
    string Password {  get; }

    public void MinigameWon();
}
