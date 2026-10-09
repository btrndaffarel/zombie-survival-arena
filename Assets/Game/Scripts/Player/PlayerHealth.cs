using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private Image[] heartImages;       // Masukkan 6 objek Heart dari Hierarchy ke sini

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;

    private float currentHealth;
    private bool isDead = false;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
        UpdateHeartUI();
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log("Player terkena " + damage + " damage. HP: " + currentHealth + "/" + maxHealth);

        UpdateHeartUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHeartUI()
    {
        // Menghitung berapa hati yang harus aktif berdasarkan sisa darah
        // Karena ada 6 hati (indeks 0 sampai 5)
        float healthPercentage = currentHealth / maxHealth;
        int activeHearts = Mathf.CeilToInt(healthPercentage * heartImages.Length);

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < activeHearts)
            {
                heartImages[i].gameObject.SetActive(true);  // Hati ditampilkan
            }
            else
            {
                heartImages[i].gameObject.SetActive(false); // Hati disembunyikan/hilang
            }
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("PLAYER MATI!");
    }
}