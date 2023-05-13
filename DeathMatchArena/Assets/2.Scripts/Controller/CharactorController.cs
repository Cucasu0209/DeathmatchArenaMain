using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class CharactorController : MonoBehaviour
{
    private enum AnimationType
    {
        Run, Idle, JumpUp, JumpDown
    }
    public SkeletonAnimation skAnim;
    public Transform CharactorTransform;
    private Spine.TrackEntry skAnimTrack;
    [SpineAnimation] public string RunAnim;
    [SpineAnimation] public string IdleAnim;
    [SpineAnimation] public string JumpUpAnim;
    [SpineAnimation] public string JumpDownAnim;

    public Rigidbody2D body;
    public float speed = 15;
    public float jumpSpeed = 15;
    private bool grounded = false;
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

                break;
            }
        }
        if (grounded == false)
        {
            if (body.velocity.y > 0) SetAnimationLoop(AnimationType.JumpUp);
            else SetAnimationLoop(AnimationType.JumpDown);
        }

        Debug.Log(grounded);
    }
    #endregion

    #region Private Actions
    private void SetupDefault()
    {
        if (body == null) body = gameObject.GetComponent<Rigidbody2D>();
    }
    #endregion

    #region Public Actions
    public void Move(Vector2 direction)
    {
        if (direction.x > 0)
        {
            CharactorTransform.localScale = new Vector3(1, 1, 1);
            body.velocity = new Vector2(speed, body.velocity.y);
            SetAnimationLoop(AnimationType.Run);
        }
        else if (direction.x < 0)
        {
            CharactorTransform.localScale = new Vector3(-1, 1, 1);
            body.velocity = new Vector2(-speed, body.velocity.y);

            SetAnimationLoop(AnimationType.Run);
        }
        else
        {
            body.velocity = new Vector2(0, body.velocity.y);
            SetAnimationLoop(AnimationType.Idle);
        }
    }
    public void Jump()
    {
        if (grounded)
        {
            body.velocity = new Vector2(body.velocity.x, jumpSpeed);
        }
    }
    #endregion

    #region Animation
    private void SetAnimationLoop(AnimationType _type)
    {
        if (_type == AnimationType.Run && grounded)
        {
            if (skAnimTrack == null || skAnimTrack.Animation.Name != RunAnim)
            {
                skAnimTrack = skAnim.state.SetAnimation(0, RunAnim, true);
            }
        }
        else if (_type == AnimationType.Idle && grounded)
        {
            if (skAnimTrack == null || skAnimTrack.Animation.Name != IdleAnim)
            {
                skAnimTrack = skAnim.state.SetAnimation(0, IdleAnim, true);
            }
        }
        else if (_type == AnimationType.JumpUp)
        {
            if (skAnimTrack == null || skAnimTrack.Animation.Name != JumpUpAnim)
            {
                skAnimTrack = skAnim.state.SetAnimation(0, JumpUpAnim, true);
            }
        }
        else if (_type == AnimationType.JumpDown)
        {
            if (skAnimTrack == null || skAnimTrack.Animation.Name != JumpDownAnim)
            {
                skAnimTrack = skAnim.state.SetAnimation(0, JumpDownAnim, true);
            }
        }
    }
    #endregion
}
