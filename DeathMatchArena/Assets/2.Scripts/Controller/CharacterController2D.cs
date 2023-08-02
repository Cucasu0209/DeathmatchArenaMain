using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun.UtilityScripts;
using Photon.Pun;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

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
    public bool isdeath = false;
    public bool isTeamOne = true;
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
    public float defense = 100;
    public Vector2 MaxVelocity = new Vector2(15, 25);
    private float lastTimeUpdatePhy = 0;

    [Header("Tranforms")]
    public Transform handLeft;
    public Transform handRight;
    public Transform hatPos;
    public Transform legLeft;
    public Transform legRight;

    private string hatLink = "Skin/Hat/Hat";
    private string ShoeLink = "Skin/Shoe/Shoe";

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
    private int currentWeaponIndex = 0;
    private int currentHatIndex = 0;
    private int currentShoeIndex = 0;
    public Transform WeaponHolder;
    public BaseWeapon[] weapons;
    public GameObject gun2;

    public Hat hat;
    public Shoe shoeR;
    public Shoe shoeL;

    public float timeE = 0;
    public float timeQ = 0;
    public float MaxtimeE = 0;
    public float MaxtimeQ = 0;

    [Header("UI")]
    public TextMeshProUGUI Myname;
    public Image _pName;


    #region Unity

    private void Start()
    {
        if (photonView.enabled == true && photonView.IsMine == false) body.gravityScale = 0;
        SetupDefault();
        DisplayWeapon();

        Debug.Log(" photonView.id +" + photonView.InstantiationId);
    }
    private void Update()
    {
        timeE -= Time.deltaTime;
        timeQ -= Time.deltaTime;
        Debug.Log("Curent weapon + " + currentWeaponIndex);
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
        NetworkController_PUN.ActionOnPlayerPropertiesUpdate += UpdateWeapon;
        NetworkController_PUN.ActionOnPlayerPropertiesUpdate += UpdateHat;
        NetworkController_PUN.ActionOnPlayerPropertiesUpdate += UpdateShoe;
        NetworkController_PUN.ActionOnPlayerPropertiesUpdate += CheckDie;
        NetworkController_PUN.ActionOnPlayerPropertiesUpdate += UpdateTeam;
        NetworkController_PUN.ActionOnPlayerPropertiesUpdate += UpdateProps;
    }
    private void OnDisable()
    {
        NetworkController_PUN.ActionOnPlayerPropertiesUpdate -= UpdateDisplayName;
        NetworkController_PUN.ActionOnPlayerPropertiesUpdate -= UpdateWeapon;
        NetworkController_PUN.ActionOnPlayerPropertiesUpdate -= UpdateHat;
        NetworkController_PUN.ActionOnPlayerPropertiesUpdate -= UpdateShoe;
        NetworkController_PUN.ActionOnPlayerPropertiesUpdate -= CheckDie;
        NetworkController_PUN.ActionOnPlayerPropertiesUpdate -= UpdateTeam;
        NetworkController_PUN.ActionOnPlayerPropertiesUpdate -= UpdateProps;
    }
    #endregion

    #region Private Actions
    private void UpdateDisplayName()
    {
        Myname.SetText(NetworkController_PUN.Instance.GetPlayerProperties(photonView.Owner).playerName);
    }
    private void UpdateWeapon()
    {

        currentWeaponIndex = NetworkController_PUN.Instance.GetPlayerProperties(photonView.Owner).weaponIndex;
        MaxtimeE = weapons[currentWeaponIndex].props.AbilityCooldown_E;
        MaxtimeQ = weapons[currentWeaponIndex].props.AbilityCooldown_Q;
        DisplayWeapon();
    }
    private void UpdateHat()
    {
        currentHatIndex = NetworkController_PUN.Instance.GetPlayerProperties(photonView.Owner).hatIndex;
        Hat _hat = Resources.Load<Hat>(hatLink + (currentHatIndex + 1));
        if (_hat != null)
        {
            hat = Instantiate(_hat, hatPos);
        }
    }
    private void UpdateShoe()
    {
        currentShoeIndex = NetworkController_PUN.Instance.GetPlayerProperties(photonView.Owner).shoeIndex;
        Shoe _shoe = Resources.Load<Shoe>(ShoeLink + (currentShoeIndex + 1));
        if (_shoe != null)
        {
            shoeR = Instantiate(_shoe, legRight);
            shoeL = Instantiate(_shoe, legLeft);
        }

        DisplayWeapon();
    }
    private void UpdateProps()
    {
        speed = (shoeL.data.Speed + hat.data.Speed + weapons[currentWeaponIndex].data.Speed + 30) / 2f;
        defense = (shoeL.data.Defense + hat.data.Defense + weapons[currentWeaponIndex].data.Defense + 30);
    }
    private void UpdateTeam()
    {
        isTeamOne = NetworkController_PUN.Instance.GetPlayerProperties(photonView.Owner).slotInRoom <= 1;
        _pName.color = isTeamOne ? Color.green : Color.red;
    }

    private void CheckDie()
    {
        if (isdeath) return;
        isdeath = NetworkController_PUN.Instance.GetPlayerProperties(photonView.Owner).playerHealth <= 0;
        if (isdeath)
        {
            transform.DOScale(0, 0.2f);
            body.bodyType = RigidbodyType2D.Static;
            body.gravityScale = 0;
            body.velocity = Vector2.zero;
            transform.DOMove(new Vector3(0, 2.18f, transform.position.z), 0.5f).SetDelay(2);
            if (photonView.IsMine)
            {
                Camera.main.DOOrthoSize(20, 0.1f).SetDelay(2.5f);
                GamePlayController.instance.ShowDeath();
            }

            CharactorUserInput input = GetComponentInChildren<CharactorUserInput>();
            if (input != null) Destroy(input);

            ParticleSystem effect = Resources.Load<ParticleSystem>("Effect/EffectCloud");
            if (effect != null)
            {
                effect = Instantiate(effect, transform.position, Quaternion.identity);
                effect.Emit(40);
                Destroy(effect.gameObject, 1);
            }

        }
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
        if (timeE >= 0) return;
        timeE = weapons[currentWeaponIndex].props.AbilityCooldown_E;
        photonView.RPC(nameof(RPCAttackE), RpcTarget.AllViaServer);
    }
    public void AttackQ()
    {
        if (timeQ >= 0) return;
        timeQ = weapons[currentWeaponIndex].props.AbilityCooldown_Q;
        photonView.RPC(nameof(RPCAttackQ), RpcTarget.AllViaServer);
    }
    private void DisplayWeapon()
    {
        foreach (var we in weapons)
        {
            we.gameObject.SetActive(true);
            if (we != weapons[currentWeaponIndex])
            {
                we.transform.DOScale(0, 0.1f);
            }
        }
        weapons[currentWeaponIndex].transform.DOScale(1, 0.1f);
        WeaponHolder.localScale = Vector3.one;

        gun2.SetActive(true);
        if (currentWeaponIndex == 2) gun2.transform.DOScale(1, 0.1f);
        else gun2.transform.DOScale(0, 0.1f);
    }

    public void SwitchWeapon(int index)
    {
        NetworkController_PUN.Instance.UpdateMyProperty(PlayerPropertiesType.weapon, index);
    }
    #region RPC callbacks
    [PunRPC]
    public void RPCAttackNormal()
    {
        if (isFreezing) return;
        if (canAttack == false) return;
        weapons[currentWeaponIndex].PerformNormal(this,
            (animName) => DoAttackAnimation(animName),
            (time) => StartCoroutine(IAttackWait(time)));
    }
    [PunRPC]
    public void RPCAttackE()
    {
        if (isFreezing) return;
        if (canAttack == false) return;
        weapons[currentWeaponIndex].PerformE(this,
            (animName) => DoAttackAnimation(animName),
            (time) => StartCoroutine(IAttackWait(time)));
    }
    [PunRPC]
    public void RPCAttackQ()
    {
        if (isFreezing) return;
        if (canAttack == false) return;
        weapons[currentWeaponIndex].PerformQ(this,
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
    float lastTimeTakeDmg = 0;
    public void TakeDamage(int dmg)
    {
        if (Time.time - lastTimeTakeDmg >= 0.1f)
        {
            SettakeDamageAnim();
            if (photonView.IsMine)
            {
                Debug.Log("take damg" + dmg);
                int newHealth = NetworkController_PUN.Instance.GetPlayerProperties(photonView.Controller).playerHealth - (int)(dmg * defense / 2f / 100f);
                if (newHealth <= 0) newHealth = 0;
                NetworkController_PUN.Instance.UpdateMyProperty(PlayerPropertiesType.health, newHealth);
            }
            lastTimeTakeDmg = Time.time;
        }


    }

    private bool CanDash()
    {
        return NetworkController_PUN.Instance.GetPlayerProperties(photonView.Controller).playerPhysical >= dashPhysical;
    }
    private void DecreasePhysicalWhenDash()
    {
        if (photonView.IsMine)
        {
            int newP = NetworkController_PUN.Instance.GetPlayerProperties(photonView.Controller).playerPhysical - dashPhysical;
            if (newP <= 0) newP = 0;
            NetworkController_PUN.Instance.UpdateMyProperty(PlayerPropertiesType.physical, newP);
        }
    }
    private void IncreasePhysicalOverPriod()
    {
        if (photonView.IsMine)
        {
            int newP = NetworkController_PUN.Instance.GetPlayerProperties(photonView.Controller).playerPhysical + physicalIncreaseOverPeriod;
            if (newP >= PlayerProperties.MAX_PHYSICAL) newP = PlayerProperties.MAX_PHYSICAL;
            NetworkController_PUN.Instance.UpdateMyProperty(PlayerPropertiesType.physical, newP);
        }
    }
    #endregion
}
