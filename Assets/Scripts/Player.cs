using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private Rigidbody2D playerBody;
    [SerializeField] private CapsuleCollider2D playerCapsuleCollider;
    [SerializeField] private CapsuleCollider2D floorCapsuleCollider;
    [SerializeField] private LayerMask platformLayerMask;

    private float moveSpeed = 7f;
    private float moveDirection = 0f;
    private float lastMoveDirection = 0f;
    private bool isPlayerWalk = false;
    
    private float jumpVelocity = 40f;
    private float jumpDownTime = 0f;
    private float jumpDownTimeMax = 0.3f;
    private bool playerJumpQued = false;

    private float dashVelocity = 25f;
    private float dashVelocityMax = 25f;
    private float dashDecrease = 50f;
    private float dashTime = 0f;
    private float dashCooldown = 3f;
    private bool isDashAvailable = true;
    private bool playerDashQued = false;

    private float slideSpeed = 3f;

    private void Start() {
        GameInput.Instance.OnDashPreformed += GameInput_OnDashPreformed;
    }

    private void GameInput_OnDashPreformed(object sender, System.EventArgs e) {
        playerDashQued = DashQued();
    }

    private void Update()
    {

        moveDirection = PlayerMoveDirectionNormalized();
        isPlayerWalk = PlayerWalk();

        jumpDownTime += PlayerJumpStrength();

    }

    private void FixedUpdate() {
        if (MovingIntoWall(new Vector2(moveDirection, 0f))) {
            playerBody.linearVelocityY = -1 * slideSpeed;
        } else if (IsGrounded()) {
            playerBody.linearVelocityY = 0f;
        }

        if (!playerDashQued) {
            PlayerMove();
        }

        PlayerJump();

    if (isDashAvailable) {
        PlayerDash();
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

        inputVector.x = GameInput.Instance.GetMovementVectorNormalized();
        lastMoveDirection = GameInput.Instance.GetMovementVectorNormalized();

        return inputVector.x;
    }

    private bool PlayerWalk() {
        if (GameInput.Instance.IsWalking()) {
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
            float offWallModifier = 50f;
            if (MovingIntoWall(new Vector2(moveDirection, 0f))) {
                playerBody.linearVelocity = new Vector2(moveDirection * -1 * offWallModifier, jumpVelocity * (jumpDownTime * jumpModifier));
            }
            else {
                playerBody.linearVelocity = new Vector2(playerBody.linearVelocityX, jumpVelocity * (jumpDownTime * jumpModifier));
            }
            playerJumpQued = false;
            jumpDownTime = 0f;
        }
    }

    private float PlayerJumpStrength() {
        if (GameInput.Instance.GetJumpDown() &&
            (IsGrounded() || MovingIntoWall(new Vector2(moveDirection, 0f)))) {
            if (jumpDownTime < jumpDownTimeMax) {
                return Time.deltaTime;
            }
            return 0f;
        }
        else if (jumpDownTime > 0) {
            playerJumpQued = true;
            return 0f;
        }
        return 0f;
    }

    private bool DashQued() {
        if (isDashAvailable) {
            return true;
        }
        else if (playerDashQued) {
            return true;
        } else {
            return false;
        }
    }

    private void PlayerDash() {
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
        RaycastHit2D raycastHit = Physics2D.CapsuleCast(playerCapsuleCollider.bounds.center, 
            playerCapsuleCollider.bounds.size, 
            playerCapsuleCollider.direction, 0f, Vector2.down, 
            extraHeight, platformLayerMask);

        return raycastHit.collider != null;
    }

    private bool MovingIntoWall(Vector3 dir) {
        float extraHeight = 0.03f;
        RaycastHit2D wallHit = Physics2D.CapsuleCast(playerCapsuleCollider.bounds.center,
            playerCapsuleCollider.bounds.size,
            playerCapsuleCollider.direction, 0f, dir,
            extraHeight, platformLayerMask);
        if (wallHit.collider != null) {
            return true;
        }
            return false;
    }
}

