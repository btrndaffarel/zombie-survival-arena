using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackRange = 1.3f;
    [SerializeField] private float attackCooldown = 1f;

    private Transform player;
    private PlayerHealth playerHealth;

    private float nextAttackTime;

    private void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;

            playerHealth =
                playerObject.GetComponent<PlayerHealth>();
        }
    }

    private void Update()
    {
        if (player == null || playerHealth == null)
            return;

        if (playerHealth.IsDead)
            return;

        float distanceToPlayer =
            Vector2.Distance(
                transform.position,
                player.position
            );

        if (
            distanceToPlayer <= attackRange &&
            Time.time >= nextAttackTime
        )
        {
            Attack();

            nextAttackTime =
                Time.time + attackCooldown;
        }
    }

    private void Attack()
    {
        playerHealth.TakeDamage(damage);

        Debug.Log("Zombie menyerang Player!");
    }
}