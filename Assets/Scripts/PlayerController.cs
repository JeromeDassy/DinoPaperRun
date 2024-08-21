using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 4.6f;//default
    public float groundCheckRadius = 0.002f;

    private Rigidbody rb;
    private float jumpTime;
    private float playerStartPosY;

    private bool jumping = false;

    private void Start()
    {
        playerStartPosY = transform.position.y;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (GameManager.Instance.IsPlaying)
        {
            if(rb.isKinematic == GameManager.Instance.IsPlaying)
                rb.isKinematic = false;

            #region Input
            // Check if the player is grounded
            bool isGrounded = transform.position.y < playerStartPosY + groundCheckRadius;
            bool inputJumpTriggered = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow);
            bool inputCrawlTriggered = Input.GetKey(KeyCode.DownArrow);
            #endregion

            #region Jump
            // Jump when the player presses the jump button and is grounded
            if (inputJumpTriggered && isGrounded)
            {
                Jump(jumpForce);
                jumpTime = 0;
                jumping = true;
            }

            if (jumping)
            {
                jumpTime += Time.deltaTime;
            }

            if (inputJumpTriggered && jumping && jumpTime >= 0.15f)
            {
                Jump();
                jumping = false;
            }
            #endregion

            #region Crawl
            if (inputCrawlTriggered)
            {
                if(!isGrounded)
                    rb.velocity = new Vector2(0, -(jumpForce*2));//back to the ground
                else
                    Crawl();
            }
            #endregion
        }
        else
        {
            rb.isKinematic = true;
        }
    }

    private void Jump(float force = 0.25f)
    {
        rb.velocity = new Vector2(0, jumpForce);
    }

    private void Crawl()
    {
        //s'accroupir
    }
}