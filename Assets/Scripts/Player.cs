using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private Rigidbody2D playerBody;
    [SerializeField] private CapsuleCollider2D capsuleCollider2d;
    [SerializeField] private LayerMask platformLayerMask;

    private float moveSpeed = 7f;
    private float moveDirection = 0f;
    private float lastMoveDirection = 0f;
    private bool isPlayerWalk = false;
    
    private float jumpVelocity = 40f;
    private float jumpDownTime = 0f;
    private float jumpDownTimeMax = 0.4f;
    private bool playerJumpQued = false;

    private float dashVelocity = 25f;
    private float dashVelocityMax = 25f;
    private float dashDecrease = 50f;
    private float dashTime = 0f;
    private float dashCooldown = 3f;
    private bool isDashAvailable = true;
    private bool playerDashQued = false;

    private void Start()
    {
    }

    private void Update()
    {

        moveDirection = PlayerMoveDirectionNormalized();
        isPlayerWalk = PlayerWalk();
        playerJumpQued = PlayerJumpQued();
        playerDashQued = dashQued();

        if (jumpDownTime < jumpDownTimeMax) {
            jumpDownTime += PlayerJumpStrength();
        } 
        
    }

    private void FixedUpdate() {
        if (!playerDashQued) { 
        PlayerMove();
        }
        PlayerJump();
    if (isDashAvailable) {
        playerDash();
        }

    if (!isDashAvailable) {
            if (dashCooldown <= 0) {
                isDashAvailable = true;
                dashCooldown = 3f;
            }

            dashCooldown -= Time.deltaTime;
        }
    }

    private float PlayerMoveDirectionNormalized() {
        Vector2 inputVector = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.D)) {
            inputVector.x += 1;
            lastMoveDirection = 1;
        }
        
        if (Input.GetKey(KeyCode.A)) {
            inputVector.x -= 1;
            lastMoveDirection = -1;
        }

        return inputVector.x;
    }

    private bool PlayerWalk() {
        if (Input.GetKey(KeyCode.LeftShift)) {
            return true;
        }
        return false;
    }

    private void PlayerMove() {
        if (isPlayerWalk) {
            float walkModifier = 0.5f;
            playerBody.linearVelocity = new Vector2(moveDirection * moveSpeed * walkModifier, playerBody.linearVelocityY);
        } else {
            playerBody.linearVelocity = new Vector2(moveDirection * moveSpeed, playerBody.linearVelocityY);
        }    
    }

    private void PlayerJump() {
        if (playerJumpQued) {
            float jumpModifier = 2;
            playerBody.linearVelocity = new Vector2(playerBody.linearVelocityX, jumpVelocity * (jumpDownTime * jumpModifier));

            playerJumpQued = false;
            jumpDownTime = 0f;
        }
    }

    private bool PlayerJumpQued() {
        if (Input.GetKeyUp(KeyCode.Space) && IsGrounded()) {
            return true;
        } else if(playerJumpQued) {
            return true;
        } else {
            return false;
        }
        
    }   

    private float PlayerJumpStrength() {
        if (Input.GetKey(KeyCode.Space) && IsGrounded()) {
            return Time.deltaTime;
        }
        else {
            return 0f;
        }
    }

    private bool dashQued() {
        if (Input.GetKeyDown(KeyCode.W)) {
            return true;
        }
        else if (playerDashQued) {
            return true;
        } else {
            return false;
        }
    }

    private void playerDash() {
        if (playerDashQued) {
            playerBody.linearVelocity = new Vector2(lastMoveDirection * dashVelocity, playerBody.linearVelocityY);
            dashVelocity -= dashTime * dashDecrease;
            dashTime += Time.deltaTime;
            if (dashVelocity <= moveSpeed) {
                playerDashQued = false;
                dashVelocity = dashVelocityMax;
                dashTime = 0f;
                isDashAvailable = false;
            }  
        }
    }
    private bool IsGrounded()
    {
        float extraHeight = 0.05f;
        RaycastHit2D raycastHit = Physics2D.CapsuleCast(capsuleCollider2d.bounds.center, 
            capsuleCollider2d.bounds.size, 
            capsuleCollider2d.direction, 0f, Vector2.down, 
            extraHeight, platformLayerMask);

        return raycastHit.collider != null;
    }
        
    
}
