#define Debug_Hotkeys

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStatesEnum
{
    IDLE,
    RUNNING,
    ATTACK,
    GETHIT,
    DODGE,
    NULL = 999
}

public enum AttackStatesEnum
{
    ATTACK1,
    ATTACK2,
    COUNT,

    //SKILL1,
    //SKILL2,
    //SKILL3,

    NULL = 999
}

public class PlayerMain : MonoBehaviour
{
    [SerializeField]
    private PlayerStatesEnum previousPlayerState = PlayerStatesEnum.IDLE;
    [SerializeField]
    private PlayerStatesEnum playerState = PlayerStatesEnum.IDLE;
    [SerializeField]
    private AttackStatesEnum attackState = AttackStatesEnum.NULL;

    private Animator anim;
    private InputManager inputManager;
    private CharacterController cc;
    public CamMain camMain;
    private Transform tr;

    public int attackChainable = -1;

    private bool bIsAlive = true;
    public bool bIsInvincible = true;
    private CapsuleCollider bodyCol;

    private float moveSpeed = 8f;
    private float rotSpeed = 15f;
    private float gotHitTimer = 0f;

    public Transform rawDir;
    public Collider swordAttackCol;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        cc = GetComponent<CharacterController>();
        camMain = Camera.main.GetComponent<CamMain>();
        tr = transform;
        bodyCol = GetComponent<CapsuleCollider>();
        GameInfos.Instance.SetPlayerMain(this);
    }

    private void Start()
    {
        SetInvincible(false);
        StartCoroutine(FSM());
    }

    IEnumerator FSM()
    {
        while (bIsAlive)
        {
            anim.SetInteger("State", (int)playerState);
            yield return StartCoroutine(playerState.ToString());
            Debug.Log("<color=green>Leaving " + previousPlayerState.ToString() + ", going to  " + playerState.ToString() + "</color>");
        }
    }

    IEnumerator IDLE()
    {
        while (playerState == PlayerStatesEnum.IDLE)
        {
            UpdateLocomotion();
            UpdatePlayerButtons();

            if (inputManager.inputMoveMagnitude > 0.1f)
                SetPlayerState(PlayerStatesEnum.RUNNING, "Started Running");

            yield return null;
        }
    }

    IEnumerator RUNNING()
    {
        anim.CrossFadeInFixedTime("ANI_Move", 0.2f);
        while (playerState == PlayerStatesEnum.RUNNING)
        {
            UpdateLocomotion();
            UpdatePlayerButtons();

            if (inputManager.inputMoveMagnitude < 0.1f)
                SetPlayerState(PlayerStatesEnum.IDLE, "Stopped running");

            yield return null;
        }
    }

    IEnumerator ATTACK()
    {
        attackChainable = -1;

        while (playerState == PlayerStatesEnum.ATTACK)
        {
            // Normal attacks loop
            bool bChaining = false;
            bool bEnd = false;
            bool bAttackColOn = false;
            float postAttackTimer = 0.5f;
            bool bNextAnimStarted = false;
            while (!bEnd)
            {
                // Confort rotation
                if (inputManager.inputMoveMagnitude > 0.2f)
                {
                    rawDir.eulerAngles = new Vector3(0, camMain.transform.eulerAngles.y + Mathf.Atan2(inputManager.horizontal, inputManager.vertical) * 180 / Mathf.PI, 0);
                    tr.rotation = Quaternion.Lerp(tr.rotation, rawDir.rotation, Time.deltaTime * 0.6f);
                }

                if (playerState != PlayerStatesEnum.ATTACK)
                    bEnd = true;

                // attackChainable == 0 means you start listening to input to chain attack
                // attackChainable == 0 also triggers the attack collider.
                // attackChainable value is modified via Animation Events from the animation clip settings.
                if (attackChainable == 0)
                {
                    if (!bAttackColOn)
                    {
                        swordAttackCol.enabled = true;
                        bAttackColOn = true;
                    }

                    if (inputManager.P == InputManager.buttonState.PRESS)
                        bChaining = true;
                }
                // attackChainable == 1 means the next animation can start if chaining.
                else if (attackChainable == 1)
                {
                    if (bChaining)
                    {
                        swordAttackCol.enabled = false;

                        Attack();
                        postAttackTimer = 0.3f;
                        bChaining = false;
                        bNextAnimStarted = false;
                        bAttackColOn = false;
                        attackChainable = -1;
                    }
                    else
                    {
                        if (!bNextAnimStarted)
                        {
                            postAttackTimer = 0.1f;
                            bNextAnimStarted = true;
                        }
                        else
                        {
                            if (inputManager.P == InputManager.buttonState.PRESS)
                                bChaining = true;
                            else if (inputManager.SPACE == InputManager.buttonState.PRESS)
                                SetPlayerState(PlayerStatesEnum.DODGE, "Dodge after an attack");

                            postAttackTimer -= Time.deltaTime;
                            if (postAttackTimer <= 0f)
                                bEnd = true;
                        }
                    }
                }

                yield return null;
            }

            if (playerState == PlayerStatesEnum.ATTACK)
                SetPlayerState(PlayerStatesEnum.IDLE, "Attack ended.");

            yield return null;
        }
        swordAttackCol.enabled = false;
    }

    IEnumerator GETHIT()
    {
        while (playerState == PlayerStatesEnum.GETHIT)
        {
            if (gotHitTimer > 0.5f)
                SetPlayerState(PlayerStatesEnum.IDLE, "Back from got hit state");
            gotHitTimer += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator DODGE()
    {
        SetInvincible(true);
        anim.Play("ANI_Dodge");

        float confortRotTimer = 0.3f;
        while (playerState == PlayerStatesEnum.DODGE)
        {
            // Confort rotation
            if (confortRotTimer > 0f)
            {
                if (inputManager.inputMoveMagnitude > 0.2f)
                {
                    rawDir.eulerAngles = new Vector3(0, camMain.transform.eulerAngles.y + Mathf.Atan2(inputManager.horizontal, inputManager.vertical) * 180 / Mathf.PI, 0);
                    tr.rotation = Quaternion.Lerp(tr.rotation, rawDir.rotation, Time.deltaTime * (rotSpeed / 2));
                }
                confortRotTimer -= Time.deltaTime;
            }

            if (!bIsInvincible)
            {
                if (inputManager.inputMoveMagnitude > 0.2f)
                    SetPlayerState(PlayerStatesEnum.RUNNING, "Running after dodge");
                else
                    SetPlayerState(PlayerStatesEnum.IDLE, "Idle after dodge");
            }

            cc.Move(tr.forward * Time.deltaTime * anim.GetFloat("DodgeSpeed") * moveSpeed);
            yield return null;
        }
    }

    public void SetInvincible(bool bOnOff)
    {
        bIsInvincible = bOnOff;
        bodyCol.enabled = !bOnOff;
    }

    void UpdateLocomotion()
    {
        if (inputManager.inputMoveMagnitude > 0.2f)
        {
            rawDir.eulerAngles = new Vector3(0, camMain.transform.eulerAngles.y + Mathf.Atan2(inputManager.horizontal, inputManager.vertical) * 180 / Mathf.PI, 0);
            tr.rotation = Quaternion.Lerp(tr.rotation, rawDir.rotation, Time.deltaTime * rotSpeed);
        }

        cc.Move((inputManager.inputMoveMagnitude > 1f ? inputManager.inputMoveVector.normalized : inputManager.inputMoveVector) * Time.deltaTime * moveSpeed);
    }

    void UpdatePlayerButtons()
    {
        if (inputManager.P == InputManager.buttonState.PRESS)
            Attack();
        else if (inputManager.SPACE == InputManager.buttonState.PRESS)
            SetPlayerState(PlayerStatesEnum.DODGE, "Pressed dodge");
    }

    void Attack()
    {
        if (playerState != PlayerStatesEnum.ATTACK)
        {
            attackState = AttackStatesEnum.ATTACK1;
            SetPlayerState(PlayerStatesEnum.ATTACK, "Starting a neutral attack");
            anim.CrossFadeInFixedTime(attackState.ToString(), 0.2f);
        }
        else
        {
            int attackIdx = (int)attackState;
            attackIdx++;
            if (attackIdx == (int)AttackStatesEnum.COUNT)
                attackIdx = 0;
            attackState = (AttackStatesEnum)attackIdx;
            anim.CrossFadeInFixedTime(attackState.ToString(), 0.2f);
        }
    }

    public void SetPlayerState(PlayerStatesEnum newState, string reason)
    {
        previousPlayerState = playerState;
        playerState = newState;
        Debug.Log("<color=green>Leaving " + previousPlayerState.ToString() + ", going to  " + playerState.ToString() + "</color>. Reason : " + reason);
    }

#if Debug_Hotkeys
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.M))
            camMain.ShakeCam(0.1f);
    }
#endif

    private void OnTriggerEnter(Collider other)
    {
        if (bIsInvincible)
            return;

        if (other.gameObject.layer == LayerMask.NameToLayer("AttackPlayer"))
        {
            SetPlayerState(PlayerStatesEnum.GETHIT, "Got hit!");
            anim.CrossFadeInFixedTime("ANI_Hurt", 0.1f);
            gotHitTimer = 0f;
        }
    }
}
