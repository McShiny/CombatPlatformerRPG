using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{

    private const string PLAYER_RUN_ANIMATION = "IsRunning";
    private const string PLAYER_WALK_ANIMATION = "IsWalking";

    [SerializeField] private Player player;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Animator animator;

    private float moveEpsilon = 0.01f;
    
    private float idleGraceSeconds = 0.1f;
    private float lastMoveTime;
    private float lastDir;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Update() {
        UpdateMovement();
    }

    private void UpdateMovement() { 
        float dir = player.GetPlayerMovingDirection();

        if (Mathf.Abs(dir) > moveEpsilon) {
            lastMoveTime = Time.time;
            lastDir = dir;
        }

        spriteRenderer.flipX = lastDir < 0f;

        bool isMoving = (Time.time - lastMoveTime) <= idleGraceSeconds;

        bool isWalking = GameInput.Instance.IsWalking();
        animator.SetBool(PLAYER_WALK_ANIMATION,
                isWalking && isMoving);
        animator.SetBool(PLAYER_RUN_ANIMATION, 
            !isWalking && isMoving);
    }

}
