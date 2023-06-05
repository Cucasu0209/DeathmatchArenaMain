using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun.UtilityScripts;
using Photon.Pun;
using TMPro;

public class CharacterController2D : MonoBehaviour
{
    public PhotonView photonView;

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
    private TrackEntry TakeDmgTrack;
    [Header("Animations")]
    [SpineAnimation] public string RunAnim;
    [SpineAnimation] public string IdleAnim;
    [SpineAnimation] public string JumpUpAnim;
    [SpineAnimation] public string JumpDownAnim;
    [SpineAnimation] public string DashAnim;
    [SpineAnimation] public string TakeDmgAnim;

    [Header("movement")]
    public Rigidbody2D body;
    public float speed = 15;
    public float jumpSpeed = 25;
    public Vector2 MaxVelocity = new Vector2(15, 25);
    private float lastTimeUpdatePhy = 0;

    [Header("Tranforms")]
    public Transform handLeft;
    public Transform handRight;

    private bool grounded = false;
    private int jumpCount = 2;
    private int jumpCountMax = 2;
    private bool isDashing = false;
    private bool canMove = true;
    private bool canAttack = true;
    private bool isFreezing = false;
    private int dashPhysical = 25;
    private int physicalIncreaseOverPeriod = 15;

    [Header("Weapon")]
    public BaseWeapon weapon;

    [Header("UI")]
    public TextMeshProUGUI Myname;
    #region Unity

    private void Start()
    {
        if (photonView.enabled == true && photonView.IsMine == false) body.gravityScale = 0;
        SetupDefault();

        Debug.Log(" photonView.id +" + photonView.InstantiationId);
    }
    private void Update()
    {

    }
    private void FixedUpdate()
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
        if (Mathf.Abs(body.velocity.x) > MaxVelocity.x)
        {

            body.velocity = new Vector2(Mathf.Clamp(body.velocity.x, -MaxVelocity.x, MaxVelocity.x), body.velocity.y);
        }
        if (Mathf.Abs(body.velocity.y) > MaxVelocity.y)
        {

            body.velocity = new Vector2(body.velocity.x, Mathf.Clamp(body.velocity.y, -MaxVelocity.y, MaxVelocity.y));
        }

        //physical
        if (Time.time - lastTimeUpdatePhy > 1)
        {
            IncreasePhysicalOverPriod();
            lastTimeUpdatePhy = Time.time;
        }
    }

    private void OnEnable()
    {
        NetworkController_PUN.ActionOnPlayerPropertiesUpdate += UpdateDisplayName;
    }
    private void OnDisable()
    {
        NetworkController_PUN.ActionOnPlayerPropertiesUpdate -= UpdateDisplayName;
    }
    #endregion

    #region Private Actions
    private void UpdateDisplayName()
    {
        Myname.SetText(RoomController.Instance.GetName(photonView.Owner));
    }
    private void SetupDefault()
    {
        if (body == null) body = gameObject.GetComponent<Rigidbody2D>();
        photonView = GetComponent<PhotonView>();
        if (photonView == null || photonView.enabled == false || photonView.IsMine) CameraController.Instance.Target = transform;
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
            body.AddForce(Vector2.right * speed * 100);
        }
        else if (direction.x < 0)
        {
            body.AddForce(Vector2.right * -speed * 100);
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
            body.AddForce(Vector2.up * jumpSpeed * 1000);
            jumpCount--;
        }
    }
    public void Dash(Vector2 dir)
    {
        if (CanDash() == false) return;
        if (isFreezing) return;
        if (isDashing) return;
        if (IDashing != null) StopCoroutine(IDashing);

        if (dir.magnitude == 0)
        {
            if (CharactorTransform.localScale.x > 0) dir = Vector2.right;
            else dir = Vector2.left;
        }

        DecreasePhysicalWhenDash();
        IDashing = IEDash(dir);
        StartCoroutine(IDashing);

    }
    public void AttackNormal()
    {
        photonView.RPC(nameof(RPCAttackNormal), RpcTarget.AllViaServer);
    }
    public void AttackE()
    {
        photonView.RPC(nameof(RPCAttackE), RpcTarget.AllViaServer);
    }
    public void AttackQ()
    {
        photonView.RPC(nameof(RPCAttackQ), RpcTarget.AllViaServer);
    }

    #region RPC callbacks
    [PunRPC]
    public void RPCAttackNormal()
    {
        if (isFreezing) return;
        if (canAttack == false) return;
        weapon.PerformNormal(this,
            (animName) => DoAttackAnimation(animName),
            (time) => StartCoroutine(IAttackWait(time)));
    }
    [PunRPC]
    public void RPCAttackE()
    {
        if (isFreezing) return;
        if (canAttack == false) return;
        weapon.PerformE(this,
            (animName) => DoAttackAnimation(animName),
            (time) => StartCoroutine(IAttackWait(time)));
    }
    [PunRPC]
    public void RPCAttackQ()
    {
        if (isFreezing) return;
        if (canAttack == false) return;
        weapon.PerformQ(this,
            (animName) => DoAttackAnimation(animName),
            (time) => StartCoroutine(IFreezeWhenQPerform(time)));
    }
    #endregion
    IEnumerator IDashing;
    IEnumerator IEDash(Vector2 dir)
    {
        //set Frezee
        MaxVelocity *= 30;
        isDashing = true;
        canMove = false;
        var _cachegravityScale = body.gravityScale;
        body.gravityScale = 0;
        body.velocity = Vector2.zero;

        //dash
        body.AddForce(dir.normalized * speed * 3000);
        //body.velocity = dir.normalized * speed * 5;
        UpdateAnimationType();
        yield return new WaitForSeconds(0.1f);

        // end dash
        isDashing = false;
        canMove = true;
        body.gravityScale = _cachegravityScale;
        body.velocity = Vector2.zero;
        MaxVelocity /= 30;
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
        if (body.velocity.x < -0.5f) CharactorTransform.localScale = new Vector3(-1, 1, 1);
        else if (body.velocity.x > 0.5f) CharactorTransform.localScale = new Vector3(1, 1, 1);
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
    private void SettakeDamageAnim()
    {
        TakeDmgTrack = skAnim.state.SetAnimation(3, TakeDmgAnim, false);
        TakeDmgTrack.Complete += (track) =>
        {
            skAnim.state.SetEmptyAnimation(3, 0.1f);
        };
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

    #region Take Dmg and Dash
    public void TakeDamage(int dmg)
    {
        SettakeDamageAnim();
        if (photonView.IsMine)
        {
            Debug.Log("take damg" + dmg);
            int newHealth = RoomController.Instance.GetHealth(photonView.Controller) - dmg;
            if (newHealth <= 0) newHealth = 0;
            NetworkController_PUN.Instance.UpdateNewHealth(newHealth);
        }

    }

    private bool CanDash()
    {
        return RoomController.Instance.GetPhysical(photonView.Controller) >= dashPhysical;
    }
    private void DecreasePhysicalWhenDash()
    {
        if (photonView.IsMine)
        {
            int newP = RoomController.Instance.GetPhysical(photonView.Controller) - dashPhysical;
            if (newP <= 0) newP = 0;
            NetworkController_PUN.Instance.UpdateNewPhysical(newP);
        }
    }
    private void IncreasePhysicalOverPriod()
    {
        if (photonView.IsMine)
        {
            int newP = RoomController.Instance.GetPhysical(photonView.Controller) + physicalIncreaseOverPeriod;
            if (newP >= NetworkController_PUN.MAX_PHYSICAL) newP = NetworkController_PUN.MAX_PHYSICAL;
            NetworkController_PUN.Instance.UpdateNewPhysical(newP);
        }
    }
    #endregion
}
