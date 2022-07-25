using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdle : IState
{
    private PlayerStateManager player;

    public PlayerIdle(PlayerStateManager player)
    {
        this.player = player;
    }

    public void OperateEnter()
    {
        player.anim.SetBool("isJumping", false);
        Debug.Log("IdleEnter");
    }

    public void OperateExit()
    {

    }
    public void OperateUpdate()
    {

    }
}
