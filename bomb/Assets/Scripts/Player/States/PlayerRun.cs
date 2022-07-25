using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRun : IState
{
    private PlayerStateManager player;

    public PlayerRun(PlayerStateManager player)
    {
        this.player = player;
    }

    public void OperateEnter()
    {
        Debug.Log("RunEnter");
        player.anim.SetBool("isRunning", true);
    }
    public void OperateExit()
    {
        player.anim.SetBool("isRunning", false);
    }
    public void OperateUpdate()
    {
        float direction = Input.GetAxisRaw("Horizontal");
        if(player.isWallAttached && (player.isHeadingRight ? 1 : -1) * direction > 0) return;
        //isHeadingRight는 현재 누르고 있는 방향을 가리킴, 중립 상태에서는 가장 마지막으로 눌렀던 방향을 가리킴
        if (direction != 0) player.CmdIsHeadingSync(direction > 0 ? true : false);
        Rigidbody2D rbody = player.rigid2d;
        float curAccel = Mathf.Abs(rbody.velocity.x) < player.MinSpeed ? player.Accelaration * 2 : player.Accelaration;
        float xVelocity = rbody.velocity.x + direction * curAccel * Time.deltaTime;
        float curMaxSpeed = player.isBerserk ? player.BerserkMaxSpeed : player.MaxSpeed;
        rbody.velocity = new Vector2(Mathf.Clamp(xVelocity, -curMaxSpeed, curMaxSpeed), rbody.velocity.y);
    }
}