using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlatformController : MonoBehaviour
{
    [HideInInspector] public bool facingRight = true;
    [HideInInspector] public bool jump = false;


    public float runSpeed = 8;
    float xMove;

    public float maxSpeed = 5f;
    public float jumpForce = 1000f;


    public bool grounded = false;
    private Rigidbody2D rb;


    public Collider2D collider;
    public Vector2 topLeft;
    public Vector2 bottomRight;
    public LayerMask groundLayers;
    public Transform groundedCheck;
    const float groundedRadius = .2f; // Radius of the overlap circle to determine if grounded


    //mario jump
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    public SpriteRenderer spriteRen;

    // public SpriteRenderer deathSpriteRen;
    public Sprite idleSprite;
    public Sprite jumpSprite;
    public Sprite fallSprite;
    public Sprite runSprite;

    // public Sprite idleHurtSprite;
    // public Sprite jumpHurtSprite;
    // public Sprite fallHurtSprite;
    // public Sprite runHurtSprite;

    // public bool hurt = false;
    // float hurtTime = 0.2f;

    // public Vector2 shotForce = Vector2.zero;
    // public bool shot = false;

    // public Vector2 recoilForce = Vector2.zero;

    // Use this for initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump") && grounded )
        {
            jump = true;
        }

        if (Input.GetAxisRaw("Horizontal") == 1 && facingRight)
            Flip();
        else if (Input.GetAxisRaw("Horizontal") == -1 && !facingRight)
            Flip();

        //mario jump
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        spriteRen.sprite = idleSprite;
        // deathSpriteRen.sprite = idleSprite;

        // if (hurt)
        // {
        //     spriteRen.sprite = idleHurtSprite;
        //     deathSpriteRen.sprite = idleHurtSprite;
        // }

        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.01f)
        {
            spriteRen.sprite = runSprite;
            // deathSpriteRen.sprite = runSprite;
            // if (hurt)
            // {
            //     spriteRen.sprite = runHurtSprite;
            //     deathSpriteRen.sprite = runHurtSprite;
            // }
        }

        if (!grounded)
        {
            if (rb.velocity.y > 0) //and jumping
            {
                spriteRen.sprite = jumpSprite;
                // deathSpriteRen.sprite = jumpSprite;
                // if (hurt)
                // {
                //     spriteRen.sprite = jumpHurtSprite;
                //     deathSpriteRen.sprite = jumpHurtSprite;
                // }
            }
            else if (rb.velocity.y < 0)
            {
                spriteRen.sprite = fallSprite;
                // deathSpriteRen.sprite = fallSprite;
                // if (hurt)
                // {
                //     spriteRen.sprite = fallHurtSprite;
                //     deathSpriteRen.sprite = fallHurtSprite;
                // }
            }
            
        }
        
        print(collider.bounds.size);


        //check if grounded
        topLeft = new Vector2(transform.position.x - collider.bounds.extents.x, transform.position.y - collider.bounds.extents.y);
        bottomRight = new Vector2(transform.position.x + collider.bounds.extents.x, transform.position.y + collider.bounds.extents.y);
        
        // grounded = Physics2D.OverlapArea(topLeft, bottomRight, groundLayers);

        grounded = false;
        
        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundedCheck.position, groundedRadius, groundLayers);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                grounded = true;
                // if (!wasGrounded)
                //     OnLandEvent.Invoke();
            }
        }
        
        print(grounded);

        float h;

        //store Right Hor input
        h = Input.GetAxis("Horizontal");


        //store wish move Right Hor
        float xMove = h * runSpeed;

        //move Right Horly
        rb.velocity += new Vector2(xMove - rb.velocity.x, 0);

        //jump
        if (jump )
        {
            rb.AddForce(new Vector2(0f, jumpForce));
            jump = false;
        }

        // rb.velocity += shotForce;
        // rb.velocity += recoilForce;


        //if (shot)
        //{

        //    LeanTween.value(shotForce.x, 0, 0.2f).setEase(LeanTweenType.easeOutExpo).setOnUpdate((float val) =>
        //    {
        //        shotForce.x = val;
        //    });
        //    LeanTween.value(shotForce.y, 0, 0.2f).setOnUpdate((float val) =>
        //    {
        //        shotForce.y = val;
        //    });
        //    shot = false;
        //}

        // shotForce = Vector3.Lerp(shotForce, Vector3.zero, 0.35f);
        // recoilForce = Vector3.Lerp(recoilForce, Vector3.zero, 0.35f);

        //limit speed by maxSpeed
        if (Mathf.Abs(rb.velocity.x) > maxSpeed)
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
    }


    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    // public void Hurt()
    // {
    //     StartCoroutine(HurtCo());
    // }

    // IEnumerator HurtCo()
    // {
    //     hurt = true;
    //     yield return new WaitForSeconds(hurtTime);
    //     hurt = false;
    // }
}