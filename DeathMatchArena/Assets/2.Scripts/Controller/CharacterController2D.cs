using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class CharacterController2D : MonoBehaviour
{
    private enum AnimationType
    {
        Run,
        Idle,
        JumpUp,
        JumpDown,
        Dash
    }
    public SkeletonAnimation skAnim;
    public Transform CharactorTransform;
    private TrackEntry DashTrack;
    private TrackEntry MoveTrack;
    private TrackEntry AttackTrack;
    [Header("Animations")]
    [SpineAnimation] public string RunAnim;
    [SpineAnimation] public string IdleAnim;
    [SpineAnimation] public string JumpUpAnim;
    [SpineAnimation] public string JumpDownAnim;
    [SpineAnimation] public string DashAnim;

    [Header("movement")]
    public Rigidbody2D body;
    public float speed = 15;
    public float jumpSpeed = 150;


    private bool grounded = false;
    private int jumpCount = 2;
    private int jumpCountMax = 2;
    private bool isDashing = false;
    private bool canMove = true;
    private bool canAttack = true;
    private bool isFreezing = false;


    [Header("Weapon")]
    public BaseWeapon weapon;
    #region Unity

    private void Start()
    {
        SetupDefault();
    }
    private void Update()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, 0.4f);

        grounded = false;
        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.isTrigger == false && hit.collider.gameObject != gameObject)
            {
                grounded = true;
                jumpCount = jumpCountMax;
                break;
            }
        }

        UpdateAnimationType();
    }
    #endregion

    #region Private Actions
    private void SetupDefault()
    {
        if (body == null) body = gameObject.GetComponent<Rigidbody2D>();
    }
    private void FreezeSelf()
    {
        isFreezing = true;
        body.velocity = Vector2.zero;
        body.bodyType = RigidbodyType2D.Kinematic;
    }
    private void CancelFreezeSelf()
    {
        isFreezing = false;
        body.bodyType = RigidbodyType2D.Dynamic;
    }
    #endregion

    #region Public Actions
    public void Move(Vector2 direction)
    {
        if (isFreezing) return;
        if (canMove == false) return;
        if (direction.x > 0)
        {
            body.velocity = new Vector2(speed, body.velocity.y);
        }
        else if (direction.x < 0)
        {
            body.velocity = new Vector2(-speed, body.velocity.y);
        }
        else
        {
            body.velocity = new Vector2(0, body.velocity.y);
        }
    }
    public void Jump()
    {
        if (isFreezing) return;
        if (jumpCount > 0)
        {
            body.velocity = new Vector2(body.velocity.x, jumpSpeed);
            jumpCount--;
        }
    }
    public void Dash(Vector2 dir)
    {
        if (isFreezing) return;
        if (isDashing) return;
        if (IDashing != null) StopCoroutine(IDashing);

        if (dir.magnitude == 0)
        {
            if (CharactorTransform.localScale.x > 0) dir = Vector2.right;
            else dir = Vector2.left;
        }
        IDashing = IEDash(dir);
        StartCoroutine(IDashing);

    }
    public void AttackNormal()
    {
        if (isFreezing) return;
        if (canAttack == false) return;
        weapon.PerformNormal(this,
            (animName) => DoAttackAnimation(animName),
            (time) => StartCoroutine(IAttackWait(time)));
    }
    public void AttackE()
    {
        if (isFreezing) return;
        if (canAttack == false) return;
        weapon.PerformE(this,
            (animName) => DoAttackAnimation(animName),
            (time) => StartCoroutine(IAttackWait(time)));
    }
    public void AttackQ()
    {
        if (isFreezing) return;
        if (canAttack == false) return;
        weapon.PerformQ(this,
            (animName) => DoAttackAnimation(animName),
            (time) => StartCoroutine(IFreezeWhenQPerform(time)));
    }
    IEnumerator IDashing;
    IEnumerator IEDash(Vector2 dir)
    {
        //set Frezee
        isDashing = true;
        canMove = false;
        var _cachegravityScale = body.gravityScale;
        body.gravityScale = 0;
        body.velocity = Vector2.zero;

        //dash
        body.velocity = dir.normalized * speed * 5;
        UpdateAnimationType();
        yield return new WaitForSeconds(0.1f);

        // end dash
        isDashing = false;
        canMove = true;
        body.gravityScale = _cachegravityScale;
        body.velocity = Vector2.zero;
    }
    #endregion

    #region Animation
    private void DoAttackAnimation(string animationName)
    {
        //if (AttackTrack != null && AttackTrack.IsComplete == false) return;
        SetAttackAnimation(animationName);

    }
    private void UpdateAnimationType()
    {
        if (body.velocity.x < 0) CharactorTransform.localScale = new Vector3(-1, 1, 1);
        else if (body.velocity.x > 0) CharactorTransform.localScale = new Vector3(1, 1, 1);
        if (isDashing)
        {
            SetAnimationLoop(AnimationType.Dash);
        }
        else if (!grounded)
        {
            if (body.velocity.y > 0) SetAnimationLoop(AnimationType.JumpUp);
            else SetAnimationLoop(AnimationType.JumpDown);
        }
        else if (Mathf.Abs(body.velocity.x) > 2)
        {
            SetAnimationLoop(AnimationType.Run);
        }
        else
        {
            SetAnimationLoop(AnimationType.Idle);
        }
    }
    private void SetAnimationLoop(AnimationType _type)
    {
        if (_type == AnimationType.Run)
        {
            if (MoveTrack == null || MoveTrack.Animation.Name != RunAnim)
            {
                MoveTrack = skAnim.state.SetAnimation(0, RunAnim, true);
            }
        }
        else if (_type == AnimationType.Idle)
        {
            if (MoveTrack == null || MoveTrack.Animation.Name != IdleAnim)
            {
                MoveTrack = skAnim.state.SetAnimation(0, IdleAnim, true);
            }
        }
        else if (_type == AnimationType.JumpUp)
        {
            if (MoveTrack == null || MoveTrack.Animation.Name != JumpUpAnim)
            {
                MoveTrack = skAnim.state.SetAnimation(0, JumpUpAnim, true);
            }
        }
        else if (_type == AnimationType.JumpDown)
        {
            if (MoveTrack == null || MoveTrack.Animation.Name != JumpDownAnim)
            {
                MoveTrack = skAnim.state.SetAnimation(0, JumpDownAnim, true);
            }
        }
        else if (_type == AnimationType.Dash)
        {

            DashTrack = skAnim.state.SetAnimation(1, DashAnim, true);
            DashTrack.Complete += (track) =>
            {
                skAnim.state.SetEmptyAnimation(1, 0);
            };

        }

    }
    private void SetAttackAnimation(string animName)
    {
        try
        {

            AttackTrack = skAnim.state.SetAnimation(2, animName, false);
            AttackTrack.Complete += (track) =>
            {
                skAnim.state.SetEmptyAnimation(2, 0.1f);
            };
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Animation name {animName} doesn't exsit.");
        }
    }
    private IEnumerator IFreezeWhenQPerform(float time)
    {
        FreezeSelf();
        yield return new WaitForSeconds(time);
        CancelFreezeSelf();
    }

    private IEnumerator IAttackWait(float time)
    {
        canAttack = false;
        yield return new WaitForSeconds(time);
        canAttack = true;
    }
    #endregion
}
