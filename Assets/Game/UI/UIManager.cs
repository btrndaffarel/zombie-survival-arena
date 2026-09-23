using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Game References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerWallet playerWallet;
    [SerializeField] private WaveManager waveManager;

    [Header("HUD References")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text zombieText;
    [SerializeField] private TMP_Text coinText;

    private void Update()
    {
        UpdateHealthUI();
        UpdateWaveUI();
        UpdateCoinUI();
    }

    private void UpdateHealthUI()
    {
        if (playerHealth == null)
            return;

        healthText.text =
            "HEALTH: " +
            Mathf.CeilToInt(playerHealth.CurrentHealth) +
            " / " +
            Mathf.CeilToInt(playerHealth.MaxHealth);
    }

    private void UpdateWaveUI()
    {
        if (waveManager == null)
            return;

        waveText.text =
            "WAVE: " +
            waveManager.CurrentWave +
            " / " +
            waveManager.MaxWaves;

        zombieText.text =
            "ZOMBIES: " +
            waveManager.EnemiesRemaining;
    }

    private void UpdateCoinUI()
    {
        if (playerWallet == null)
            return;

        coinText.text =
            "COIN: " +
            playerWallet.CurrentCoin;
    }
}