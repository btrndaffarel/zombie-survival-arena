using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerAnimation : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private string currentAnimation = "";

    private FacingDirection lastDirection = FacingDirection.Down;

    private enum FacingDirection
    {
        Down,
        Side,
        Up
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Vector2 velocity = rb.linearVelocity;

        bool isMoving = velocity.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            PlayMovementAnimation(velocity);
        }
        else
        {
            PlayIdleAnimation();
        }
    }

    private void PlayMovementAnimation(Vector2 velocity)
    {
        // Gerakan horizontal lebih dominan
        if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
        {
            lastDirection = FacingDirection.Side;

            // Kalau orientasi sprite nanti terbalik,
            // tinggal dibalik true/false-nya.
            spriteRenderer.flipX = velocity.x < 0f;

            PlayAnimation("Player_Walk_Side");
        }
        // Bergerak ke atas
        else if (velocity.y > 0f)
        {
            lastDirection = FacingDirection.Up;

            spriteRenderer.flipX = false;

            PlayAnimation("Player_Walk_Up");
        }
        // Bergerak ke bawah
        else
        {
            lastDirection = FacingDirection.Down;

            spriteRenderer.flipX = false;

            PlayAnimation("Player_Walk_Down");
        }
    }

    private void PlayIdleAnimation()
    {
        switch (lastDirection)
        {
            case FacingDirection.Down:
                PlayAnimation("Player_Idle_Down");
                break;

            case FacingDirection.Side:
                PlayAnimation("Player_Idle_Side");
                break;

            case FacingDirection.Up:
                PlayAnimation("Player_Idle_Up");
                break;
        }
    }

    private void PlayAnimation(string animationName)
    {
        if (currentAnimation == animationName)
            return;

        animator.Play(animationName);

        currentAnimation = animationName;
    }
}