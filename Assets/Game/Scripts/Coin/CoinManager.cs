using UnityEngine;
using TMPro; 
public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TMP_Text coinCountText; 

    private int totalCoins = 0;

    public int TotalCoins => totalCoins;

    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateCoinUI();
    }

   
    public void AddCoins(int amount)
    {
        totalCoins += amount;
        UpdateCoinUI();
    }

    private void UpdateCoinUI()
    {
        if (coinCountText != null)
        {
            coinCountText.text = "Coin: " + totalCoins;
        }
    }
}