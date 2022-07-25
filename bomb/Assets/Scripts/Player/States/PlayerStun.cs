using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStun : IState
{
    private PlayerStateManager player;

    public PlayerStun(PlayerStateManager player)
    {
        this.player = player;
    }

    public void OperateEnter()
    {
        player.stunVFX.SetActive(true);
        player.anim.SetTrigger("isStunning");
        player.anim.SetBool("isStunned", true);
        foreach(var rend in player.spriteRenderer)
        {
            rend.color = new Color(1f, 1f, 1f, 0.5f);    
        }
        player.coll.sharedMaterial = player.stunPhysicsMat;
    }
    public void OperateExit()
    {
        player.stunVFX.SetActive(false);
        player.anim.SetBool("isStunned", false);
        player.coll.sharedMaterial = player.idlePhysicsMat;
        player.rigid2d.velocity = new Vector2(Mathf.Clamp(player.rigid2d.velocity.x, -player.MaxSpeed, player.MaxSpeed), player.rigid2d.velocity.y);
        foreach(var rend in player.spriteRenderer)
        {
            rend.color = new Color(1f, 1f, 1f, 1f);    
        }
    }
    public void OperateUpdate()
    {

    }
}