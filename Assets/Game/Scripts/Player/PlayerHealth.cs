using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Diperlukan untuk berpindah scene

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
        float healthPercentage = currentHealth / maxHealth;

        // Diperbaiki dengan menambahkan tanda kali (*)
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

        // Pindah ke scene GameOverScene saat nyawa habis
        SceneManager.LoadScene("GameOverScene");
    }
}