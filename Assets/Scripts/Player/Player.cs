using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.UI.Image;
using Debug = UnityEngine.Debug;

public class Player : MonoBehaviour
{

    public static Player Instance { get; private set; }

    public event EventHandler<OnPlayerDashedEventArgs> OnPlayerDashed;
    public class OnPlayerDashedEventArgs : EventArgs {
        public float progressNormalized;
    }

    [SerializeField] private Rigidbody2D playerBody;
    [SerializeField] private CapsuleCollider2D playerCapsuleCollider;
    [SerializeField] private LayerMask platformLayerMask;

    [SerializeField] private Transform playerTopPosition;
    [SerializeField] private Transform playerMidPosition;

    // Movement Variables
    private float moveSpeed = 7f;
    private float moveDirection = 0f;
    private float lastMoveDirection = 0f;
    private float playerMovingDirection = 0f;
    private bool isPlayerWalk = false;

    public float terminalVelocity = 50f;

    private float groundedGrace = 0.08f;
    private float groundedTimer;

    // Jump Variables
    private float jumpVelocity = 40f;
    private float jumpStrength;
    private float jumpStrengthMin = 0.50f;
    private float jumpStrengthMax = 1.25f;
    private bool playerJumpQued = false;

    // DoubleJump Variables
    private float doubleJumpVelocity = 30f;
    private bool doubleJumpAvailable = true;
    private bool doubleJumpQued = false;

    // Dash Variables
    private float dashVelocity = 30f;
    private float dashVelocityMax = 30f;
    private float dashDecrease = 50f;
    private float dashTime = 0f;
    private float dashCooldown = 3f;
    private bool isDashAvailable = true;
    private bool playerDashQued = false;

    private float slideSpeed = 3f;

    private void Awake() {
        Instance = this;
    }
    private void Start() {
        GameInput.Instance.OnDashPreformed += GameInput_OnDashPreformed;
        GameInput.Instance.OnJumpPreformed += GameInput_OnJumpPreformed;
    }

    private void GameInput_OnJumpPreformed(object sender, EventArgs e) {
        PlayerDoubleJump();
    }

    private void GameInput_OnDashPreformed(object sender, System.EventArgs e) {
        playerDashQued = DashQued();
    }

    private void Update()
    {

        PlayerMoveDirectionNormalized();
        PlayerWalk();
        QueJump();

    }

    private void FixedUpdate() {
        if (MovingIntoWall(new Vector2(moveDirection, 0f))) {
            playerBody.linearVelocityY = -1 * slideSpeed;
        } if (IsGrounded()) groundedTimer = groundedGrace;
            else groundedTimer -= Time.fixedDeltaTime;

        bool grounded = groundedTimer > 0f;

        if (grounded && Mathf.Abs(playerBody.linearVelocityY) < 0.5f) {
            playerBody.linearVelocityY = 0f;
            doubleJumpAvailable = true;
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
                OnPlayerDashed?.Invoke(this, new OnPlayerDashedEventArgs {
                    progressNormalized = 1f
                });
            }

            dashCooldown -= Time.deltaTime;

            if (isDashAvailable == false) { 
                OnPlayerDashed?.Invoke(this, new OnPlayerDashedEventArgs {
                    progressNormalized = 1 - dashCooldown / 3
                });
            }
        }

        if (playerBody.linearVelocityY > terminalVelocity) {
            playerBody.linearVelocityY = terminalVelocity;
        }
    }

    private void PlayerMoveDirectionNormalized() {
        Vector2 inputVector = new Vector2(0, 0);

        inputVector.x = GameInput.Instance.GetMovementVectorNormalized();
        playerMovingDirection = inputVector.x;
        lastMoveDirection = GameInput.Instance.GetMovementVectorNormalized();

        moveDirection = inputVector.x;
    }

    private void PlayerWalk() {
        if (GameInput.Instance.IsWalking()) {
            isPlayerWalk = true;
        }
        else {
            isPlayerWalk = false;
        }
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
            float offWallModifier = 360f;
            float jumpModifier = 0.3f;

            if (GameInput.Instance.GetJumpDown() && MovingIntoWall(new Vector2(moveDirection, 0f))) {
                playerBody.linearVelocity = new Vector2(moveDirection * -1 * offWallModifier * jumpStrength * jumpModifier,
                    jumpVelocity * jumpStrength * jumpModifier);
                jumpStrength -= Time.deltaTime * 2;
                if (jumpStrength <= jumpStrengthMin) {
                    playerJumpQued = false;
                    jumpStrength = 0f;
                }
            }
            else if (GameInput.Instance.GetJumpDown()) {
                playerBody.linearVelocity = new Vector2(playerBody.linearVelocityX,
                    jumpVelocity * jumpStrength * jumpModifier);
                jumpStrength -= Time.deltaTime * 2;
                if (jumpStrength <= jumpStrengthMin) {
                    playerJumpQued = false;
                    jumpStrength = 0f;
                }
            }
            else {
                playerJumpQued = false;
            }
        } else if(doubleJumpQued) {
            playerBody.linearVelocity = new Vector2(playerBody.linearVelocityX, doubleJumpVelocity);
            doubleJumpQued = false;
        }
    }

    private void QueJump() {
        if (GameInput.Instance.GetJumpDown() && (IsGrounded() || MovingIntoWall(new Vector3(moveDirection, 0f, 0f)))) {
            jumpStrength = jumpStrengthMax;
            playerJumpQued = true;
        }
    }

    private void PlayerDoubleJump() {
        if (doubleJumpAvailable && (!IsGrounded() && !MovingIntoWall(new Vector3(moveDirection, 0f, 0f)))) {
            doubleJumpQued = true;
            doubleJumpAvailable = false;
        }
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

                OnPlayerDashed?.Invoke(this, new OnPlayerDashedEventArgs {
                    progressNormalized = 0f
                });

            }  
        }
    }
    private bool IsGrounded()
    {
        float extraHeight = 0.1f;
        RaycastHit2D raycastHit = Physics2D.CapsuleCast(playerCapsuleCollider.bounds.center, 
            playerCapsuleCollider.bounds.size, 
            playerCapsuleCollider.direction, 0f, Vector2.down, 
            extraHeight, platformLayerMask);

        if (raycastHit.collider != null) {
            return true;
        }
        return false;
    }

    private bool MovingIntoWall(Vector3 dir) {
        float distance = 0.3f;

        if (dir.x < 0) {
            playerMidPosition.position = transform.position + new Vector3(-0.22f, 0.9f, 0f);
        } else {
            playerMidPosition.position = transform.position + new Vector3(0.3f, 0.9f, 0f);
        }

        Debug.DrawRay(playerMidPosition.position, dir * distance, Color.green);

        RaycastHit2D wallHit = Physics2D.Raycast(playerMidPosition.position, 
            dir, 
            distance, 
            platformLayerMask); 
        if (wallHit.collider != null) {
            return true;
        }
        return false;
    }

    public float GetPlayerMovingDirection() {
        return playerMovingDirection;
    }

    public bool GetPlayerGrounded() {
        return IsGrounded();
    }

    public bool GetPlayerOnWall() {
        return MovingIntoWall(new Vector3(moveDirection, 0f, 0f));
    }

}

