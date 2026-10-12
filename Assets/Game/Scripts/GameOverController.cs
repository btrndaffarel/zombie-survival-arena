using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip gameOverSound;
    private AudioSource audioSource;

    private void Start()
    {
        // Mengambil atau menambahkan AudioSource secara otomatis
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Memutar suara Game Over secara otomatis saat scene ini dimuat
        if (gameOverSound != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }
    }

    public void CobaLagi()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene");
    }

    public void KeluarKeMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScreen");
    }
}