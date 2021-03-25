using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Settings to Tweak for Gameplay Feel")]
    public float moveSpeed = 3;
    public float jumpForce = 20;
    public float normalGravity = 5f;
    public float flightGravity = 3f;

    [Space(10.0f)]
    [Header("Ground Detection Settings")]
    public Transform feet;
    public LayerMask groundLayer;

    private Rigidbody2D _rigidBody2d;
    private SpriteRenderer _spriteRenderer;
    private float _mx;
    private bool _inMoonLight = false;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody2d = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidBody2d.gravityScale = normalGravity;
    }

    private void FixedUpdate()
    {
        _mx = Input.GetAxisRaw("Horizontal");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movement = new Vector2(_mx * moveSpeed, _rigidBody2d.velocity.y);
        _rigidBody2d.velocity = movement;

        if (Input.GetButtonDown("Jump") &&  _inMoonLight)
        {
            Jump();
        }
    }

    bool IsGrounded()
    {
        Collider2D groundCheck = Physics2D.OverlapCircle(feet.position, 0.2f, groundLayer);
        return groundCheck != null;
    }

    void Jump()
    {
        Vector2 movement = new Vector2(_rigidBody2d.velocity.x, jumpForce);
        _rigidBody2d.velocity = movement;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "MoonBeam")
        {
            _inMoonLight = true;
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 0.5f);
            _rigidBody2d.gravityScale = flightGravity;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "MoonBeam")
        {
            _inMoonLight = false;
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 1.0f);
            _rigidBody2d.gravityScale = normalGravity;
        }
    }
}
