using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private Rigidbody2D playerBody;
    [SerializeField] private CapsuleCollider2D capsuleCollider2d;
    [SerializeField] private LayerMask platformLayerMask;

    private float moveSpeed = 5f;
    private float moveDirection = 0f;
    
    private float jumpVelocity = 30f;
    private bool playerJumpQued = false;
    private float jumpDownTime = 0f;
    private float jumpDownTimeMax = 0.5f;

    private void Start()
    {
    }

    private void Update()
    {

        moveDirection = PlayerMoveDirectionNormalized();
        playerJumpQued = PlayerJumpQued();

        if (jumpDownTime < jumpDownTimeMax) {
            jumpDownTime += PlayerJumpStrength();
        } 
        
    }

    private void FixedUpdate() {
        PlayerMove();
        PlayerJump();
    }

    private float PlayerMoveDirectionNormalized() {
        Vector2 inputVector = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.D)) {
            inputVector.x += 1;
        }
        
        if (Input.GetKey(KeyCode.A)) {
            inputVector.x -= 1;
        }

        return inputVector.x;
    }

    private void PlayerMove() {
        playerBody.linearVelocity = new Vector2(moveDirection * moveSpeed, playerBody.linearVelocityY);
    }

    private void PlayerJump() {
        if (playerJumpQued) {

            playerBody.linearVelocity = new Vector2(playerBody.linearVelocityX, jumpVelocity * (jumpDownTime * 2));

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
