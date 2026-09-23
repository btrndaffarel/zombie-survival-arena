using UnityEngine;

public class ZombieAnimation : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private string currentAnimation;

    private const string WALK_DOWN = "Zombie_Walk_Down";
    private const string WALK_SIDE = "Zombie_Walk_Side";
    private const string WALK_UP = "Zombie_Walk_Up";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Vector2 movement = rb.linearVelocity;

        if (movement.sqrMagnitude < 0.01f)
            return;

        if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
        {
            PlayAnimation(WALK_SIDE);

            spriteRenderer.flipX = movement.x > 0;
        }
        else
        {
            spriteRenderer.flipX = false;

            if (movement.y > 0)
            {
                PlayAnimation(WALK_UP);
            }
            else
            {
                PlayAnimation(WALK_DOWN);
            }
        }
    }

    private void PlayAnimation(string animationName)
    {
        if (currentAnimation == animationName)
            return;

        currentAnimation = animationName;
        animator.Play(animationName);
    }
}