using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class EnemyReward : MonoBehaviour
{
    [Header("Reward Settings")]
    [SerializeField] private int coinReward = 10;

    private EnemyHealth enemyHealth;
    private PlayerWallet playerWallet;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();

        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerWallet =
                player.GetComponent<PlayerWallet>();
        }
    }

    private void OnEnable()
    {
        enemyHealth.OnDied += GiveReward;
    }

    private void OnDisable()
    {
        enemyHealth.OnDied -= GiveReward;
    }

    private void GiveReward(EnemyHealth enemy)
    {
        if (playerWallet == null)
            return;

        playerWallet.AddCoin(coinReward);
    }
}