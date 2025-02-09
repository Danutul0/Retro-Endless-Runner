
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;

    [Header("Death Info")]
    bool isDead;
    [HideInInspector] public bool extraLife;

    [Header("Knockback Info")]
    [SerializeField] private Vector2 knockbackDir;
    bool isKnocked;
    bool canBeKnocked = true;

    [Header("Move Info")]
    [HideInInspector] public bool runStart;
    [SerializeField] public float moveSpeed;
    [SerializeField] float maxSpeed;
    [SerializeField] float speedMultiplier;
    float defaultSpeed;
    float speedToSurvive = 18;

    [Space]
    [SerializeField] float milestoneIncreaserDistance;
    float defaultMilestoneIncrease;
    float speedMilestone;

    [Header("Jump Info")]
    public float jumpForce;
    private bool canDoubleJump;
    [SerializeField] float doubleJumpForce;

    [Header("Slide Info")]
    [SerializeField] float slideCooldown;
    [HideInInspector] public float slideCooldownTimer;
    [SerializeField] float slideTimer;
    [SerializeField] float slideSpeed;
    float slideTimerCounter;
    bool isSliding;


    [Header("Collision Info")]
    [SerializeField] float groundCheckDistance;
    [SerializeField] float upWallCheckDistance;
    [SerializeField] LayerMask groundCheckLayer;
    [SerializeField] Transform wallCheck;
    [SerializeField] Transform upWallCheck;
    [SerializeField] Vector2 wallCheckSize;
    private bool isGrounded;
    private bool wallDetected;
    private bool upWallDetected;

    [Header("Climbing Info")]
    public bool ledgeDetected;
    [SerializeField] private Vector2 offset1;
    [SerializeField] private Vector2 offset2;
    Vector2 climbBegunPosition;
    Vector2 climbOverPosition;
    public bool canGrabLedge = true;
    bool canClimb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        speedMilestone = milestoneIncreaserDistance;
        defaultSpeed = moveSpeed;
        defaultMilestoneIncrease = milestoneIncreaserDistance;

    }

    void Update()
    {
        if (Mathf.Abs(rb.velocity.x) < 1e-5)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        if (Mathf.Abs(rb.velocity.y) < 1e-5)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        extraLife = moveSpeed >= speedToSurvive;

        AnimControllers();
        CheckCollision();

        slideTimerCounter -= Time.deltaTime;
        slideCooldownTimer -= Time.deltaTime;

        if (isDead)
            return;

        if (isKnocked)
            return;

        CheckInput();
        CheckForSlideCancel();
        CheckForLedge();
        SpeedController();

        if (isGrounded)
            canDoubleJump = true;
        
        if (runStart)
            Movement();

        
    }

    public void Damage()
    {
        if (extraLife)
            Knockback();
        else
            StartCoroutine(Dead());
    }
    private IEnumerator Dead()
    {
        AudioManager.instance.PlaySFX(3);
        isDead = true;
        canBeKnocked = false;
        rb.velocity = knockbackDir;
        anim.SetBool("isDead", true);

        yield return new WaitForSeconds(.5f);
        rb.velocity = new Vector2(0, 0);
        GameManager.instance.GameEnded();
    }

    private IEnumerator Invincibility()
    {
        Color originalColor = sr.color;
        Color darkColor = new Color(sr.color.r, sr.color.g, sr.color.b, .5f);

        canBeKnocked = false;
        sr.color = darkColor;
        yield return new WaitForSeconds(.1f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.1f);

        sr.color = darkColor;
        yield return new WaitForSeconds(.15f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.15f);

        sr.color = darkColor;
        yield return new WaitForSeconds(.25f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.25f);

        sr.color = darkColor;
        yield return new WaitForSeconds(.3f);

        sr.color = originalColor;
        canBeKnocked = true;
    }
    private void Knockback()
    {
        if (!canBeKnocked)
            return;

        StartCoroutine(Invincibility());
        isKnocked = true;
        rb.velocity = knockbackDir;
    }

    private void CancelKnockback() => isKnocked = false;


    #region SpeedControll
    private void ResetSpeed()
    {
        if (isSliding)
            return;

        moveSpeed = defaultSpeed;
        milestoneIncreaserDistance = defaultMilestoneIncrease;
    }
    private void SpeedController()
    {
        if (moveSpeed == maxSpeed)
            return;

        if (transform.position.x > speedMilestone)
        {
            speedMilestone = speedMilestone + milestoneIncreaserDistance;

            moveSpeed = moveSpeed * speedMultiplier;
            milestoneIncreaserDistance = milestoneIncreaserDistance * speedMultiplier;

            if(moveSpeed > maxSpeed)
                moveSpeed = maxSpeed;

        }
    }
    #endregion
    #region Edge Climb
    private void CheckForLedge()
    {
        if (ledgeDetected && canGrabLedge && !isSliding)
        {
            canGrabLedge = false;
            rb.gravityScale = 0;

            Vector2 legdePosition = GetComponentInChildren<LedgeDetection>().transform.position;

            climbBegunPosition = legdePosition + offset1;
            climbOverPosition = legdePosition + offset2;

            canClimb = true;
        }

        if (canClimb)
            transform.position = climbBegunPosition;
    }

    private IEnumerator LedgeClimbOver()
    {
        transform.position = climbOverPosition;
        canClimb = false;
        rb.gravityScale = 5;
        yield return new WaitForSeconds(0.5f);
        canGrabLedge = true;
    }

    private void AllowLedgeGrab() => StartCoroutine(LedgeClimbOver());


#endregion
    public void SlideButton()
    {
        if (rb.velocity.x != 0 && slideCooldownTimer <0 && rb.velocity.y == 0)
        {
            slideCooldownTimer = slideCooldown;
            isSliding = true;
            slideTimerCounter = slideTimer;
        }
    }

    private void CheckForSlideCancel()
    {
        if(slideTimerCounter < 0 && !upWallDetected)
            isSliding = false;
    }



    public void CheckInput()
    {

        if (Input.GetButtonDown("Jump"))
            JumpButton();

        if (Input.GetKeyDown(KeyCode.LeftShift))
            SlideButton();

    }

    private void Movement()
    {

        if (wallDetected && rb.velocity.x == 0)
        {
            ResetSpeed();
            return;
        }

        if (isSliding)
            rb.velocity = new Vector2(slideSpeed + moveSpeed, rb.velocity.y);      

        else
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }

    public void JumpButton()
    {
        if (isSliding)
            return;

        AnimRollFinish();

        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            AudioManager.instance.PlaySFX(Random.Range(1, 2));
        }

        else if (canDoubleJump && rb.velocity.y !=0)
        {
            AudioManager.instance.PlaySFX(Random.Range(1, 2));
            canDoubleJump = false;
            rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
        }

    }

    #region Animations
    private void AnimControllers()
    {
        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("canDoubleJump", canDoubleJump);
        anim.SetBool("isSliding", isSliding);
        anim.SetBool("canClimb", canClimb);
        anim.SetBool("isKnocked", isKnocked);

        if (rb.velocity.y < -40)
            anim.SetBool("canRoll", true);
    }

    private void AnimRollFinish() => anim.SetBool("canRoll", false);
    #endregion
    private void CheckCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundCheckLayer);
        wallDetected = Physics2D.BoxCast(wallCheck.position, wallCheckSize, 0, Vector2.zero, 0, groundCheckLayer);
        upWallDetected = Physics2D.Raycast(upWallCheck.position, Vector2.up, upWallCheckDistance, groundCheckLayer);

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(upWallCheck.position, new Vector2(upWallCheck.position.x, upWallCheck.position.y + upWallCheckDistance));
        Gizmos.DrawWireCube(wallCheck.position, wallCheckSize);
    }
}
