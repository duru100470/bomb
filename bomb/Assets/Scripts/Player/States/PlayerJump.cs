using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : IState
{
    private PlayerStateManager player;
    private bool isChecked = false;

    public PlayerJump(PlayerStateManager player)
    {
        this.player = player;
    }

    public void OperateEnter()
    {
        player.anim.SetBool("isJumping", true);
        player.anim.SetTrigger("isJump");
        isChecked = false;
        player.rigid2d.velocity = new Vector2(player.rigid2d.velocity.x, player.JumpForce);
    }
    public void OperateExit()
    {
        //Debug.Log("JumpExit");
        player.anim.SetBool("isJumping", false);
    }
    public void OperateUpdate()
    {
        //Debug.Log("JumpUpdate");
        if(!isChecked && player.rigid2d.velocity.y < 0)
        {
            player.anim.SetTrigger("isJumpHigh");
            isChecked = true;
        }

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