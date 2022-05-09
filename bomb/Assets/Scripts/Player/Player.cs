using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{

    private enum PlayerState{
        Idle,
        Run,
        Jump,
        Stun,
        Dead,
        Cast
    }

    private StateMachine stateMachine;
    // 상태를 저장할 딕셔너리 생성
    private Dictionary<PlayerState, IState> dicState = new Dictionary<PlayerState, IState>();
    [SerializeField]
    private Item curItem;
    private SpriteRenderer spriteRenderer;
    public Rigidbody2D rigid2d;
    private Collider2D coll;
   
    [SerializeField]
    private float moveSpeed = 10f;
    [SerializeField]
    private float jumpForce = 5.0f;
    public float MoveSpeed => moveSpeed;
    public float JumpForce => jumpForce;
    [SerializeField]
    private bool isGround = false;
    public bool isHeadingRight = false;
    public bool isCasting = false;
    public GameObject curItemObj;
    private float refVelocity = 0f;
    public float dashTime = 0f;
    // Initialize states
    private void Start() {
        IState idle = new PlayerIdle(this);
        IState run = new PlayerRun(this);
        IState jump = new PlayerJump(this);
        IState stun = new PlayerStun(this);
        IState dead = new PlayerDead(this);
        IState cast = new PlayerCast(this);

        dicState.Add(PlayerState.Idle, idle);
        dicState.Add(PlayerState.Run, run);
        dicState.Add(PlayerState.Jump, jump);
        dicState.Add(PlayerState.Stun, stun);
        dicState.Add(PlayerState.Dead, dead);
        dicState.Add(PlayerState.Cast, cast);

        // 시작 상태를 Idle로 설정
        stateMachine = new StateMachine(dicState[PlayerState.Idle]);

        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid2d = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }

    // 키보드 입력 받기 및 CurruentState 실행
    private void Update() {
        // 로컬 플레이어가 아닐 경우 작동 X
        if(!isLocalPlayer) return;
        KeyboardInput();
        stateMachine.DoOperateUpdate();
        if(isCasting) rigid2d.velocity = new Vector2(Mathf.SmoothDamp(rigid2d.velocity.x, 0f, ref refVelocity, dashTime), rigid2d.velocity.y);

    }

    private void FixedUpdate(){
        RaycastHit2D raycastHit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.02f, LayerMask.GetMask("Ground"));
        if(raycastHit.collider != null) isGround = true;
        else isGround = false;
        spriteRenderer.flipX = isHeadingRight;
    }
    // 키보드 입력 제어
    private void KeyboardInput()
    {
        // Stun, Dead상태가 아니거나 돌진 중이 아닐 때 행동 가능
        if (stateMachine.CurruentState != dicState[PlayerState.Stun] && stateMachine.CurruentState != dicState[PlayerState.Dead] && !isCasting){
            // Run State
            if (Input.GetAxisRaw("Horizontal") != 0){
                stateMachine.SetState(dicState[PlayerState.Run]);
            }else{
                stateMachine.SetState(dicState[PlayerState.Idle]);
            }

            // Jump State
            if (Input.GetKeyDown(KeyCode.Space) && this.isGround){
                stateMachine.SetState(dicState[PlayerState.Jump]);
            }

            // Cast State
            if (Input.GetKeyDown(KeyCode.Q) && stateMachine.CurruentState != dicState[PlayerState.Cast]){
                stateMachine.SetState(dicState[PlayerState.Cast]);
            }
        }
    }

    // 다른 플레이어 및 아이템 충돌
    private void OnTriggerEnter2D(Collider2D other) {
        // 플레이어인 경우

        // 아이템인 경우, 현재 아이템을 가지고 있지 않은 상태여야 한다
        if(other.transform.CompareTag("Item") && curItem == null){
            AddItem(other.GetComponent<Item>());
            this.curItemObj = other.GetComponent<Item>().itemObj;
            other.gameObject.SetActive(false);
        }
        // Stone에 맞았을 때
        if(other.transform.CompareTag("Projectile")){
            StartCoroutine(Stunned(other.GetComponent<StoneProjectile>().stunTime));
            Destroy(other.gameObject);
        }
        // 서버에 로그 전송
    }

    // 아이템 획득
    private void AddItem(Item item){
        // 아이템 리스트에 추가
        item.player = this;
        curItem = item;
    }

    // 아이템 사용
    public void UseItem(){
        // 아이템이 없으면 작동 안함
        if(curItem != null){
            curItem.OnUse();
            curItem = null;
        }
    }

    private IEnumerator Stunned(float stunTime){
        stateMachine.SetState(dicState[PlayerState.Stun]);
        yield return new WaitForSeconds(stunTime);
        stateMachine.SetState(dicState[PlayerState.Idle]);
    }

    [Command]
    public void StoneSpawn(GameObject obj){
        GameObject projectile = Instantiate(curItemObj, transform.position + (isHeadingRight ? new Vector3(1,0,0) : new Vector3(-1,0,0)), Quaternion.identity);
        projectile.GetComponent<StoneProjectile>().dir = isHeadingRight;
        NetworkServer.Spawn(projectile);
    }
}