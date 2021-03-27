using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: How do you keep the momentum for in/out moon beams?
// TODO: Should transforming be instant or have a time component?
// TODO: Should inputs be locked during pending transform and/or transforming?
// TODO: Seprate horizontal speed for pirate/bird?
// TODO: Transition(s) animations
// TODO: Bird cannot interact with physical objects (platforms and barrels etc.)

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Settings to Tweak for Gameplay Feel")]
    public float moveSpeed;
    public float jumpForce;
    public float normalGravity;
    public float flightGravity;
    public float intoMoonDelaySeconds;
    public float outOfMoonDelaySeconds;

    [Space(10.0f)]
    [Header("Ground Detection Settings")]
    public Transform feet;
    public LayerMask groundLayer;

    // Components
    private Rigidbody2D _rigidBody2d;
    private SpriteRenderer _spriteRenderer;

    // Horizontal movement input
    private float _mx;

    // Keep track of whether the player is inside a moonbeam
    private bool _inMoonLight = false;

    // Counters for going in/out of moonbeams
    private float _intoMoonCounter;
    private float _outOfMoonCounter;

    // Keep track of Player transformations
    private enum PlayerTransform
    {
        Pirate,
        Bird
    }
    private PlayerTransform _curTransform = PlayerTransform.Pirate;
    private PlayerTransform _prevTransform = PlayerTransform.Pirate;

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
        // Update moonbeam counters
        if (_intoMoonCounter > 0)
        {
            _intoMoonCounter -= Time.deltaTime;
        }
        if (_outOfMoonCounter > 0)
        {
            _outOfMoonCounter -= Time.deltaTime;
        }

        // Maybe transform
        MaybeTransform();

        // Calculate horizontal velocity
        Vector2 movement = new Vector2(_mx * moveSpeed, _rigidBody2d.velocity.y);
        _rigidBody2d.velocity = movement;

        // Calculate jump
        if (Input.GetButtonDown("Jump") && _curTransform == PlayerTransform.Bird)
        {
            Jump();
        }

        // Update previous transform after everything else
        _prevTransform = _curTransform;
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

    void MaybeTransform()
    {
        // Stayed in moonbeam long enough to become a bird
        if (_intoMoonCounter < 0 && _prevTransform == PlayerTransform.Pirate && _inMoonLight)
        {
            _curTransform = PlayerTransform.Bird;
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 0.5f);
            _rigidBody2d.gravityScale = flightGravity;
        }

        // Stayed out of moonbeam long enough to become a pirate
        if (_outOfMoonCounter < 0 && _prevTransform == PlayerTransform.Bird && !_inMoonLight)
        {
            _curTransform = PlayerTransform.Pirate;
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 1.0f);
            _rigidBody2d.gravityScale = normalGravity;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "MoonBeam")
        {
            _inMoonLight = true;
            _intoMoonCounter = intoMoonDelaySeconds;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "MoonBeam")
        {
            _inMoonLight = false;
            _outOfMoonCounter = outOfMoonDelaySeconds;
        }
    }
}
