using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    [Header("Coin")]
    [SerializeField] private int startingCoin = 0;

    private int currentCoin;
    private int totalCoinEarned;

    public int CurrentCoin => currentCoin;
    public int TotalCoinEarned => totalCoinEarned;

    private void Awake()
    {
        currentCoin = startingCoin;
    }

    public void AddCoin(int amount)
    {
        if (amount <= 0)
            return;

        currentCoin += amount;
        totalCoinEarned += amount;

        Debug.Log(
            "Mendapatkan " +
            amount +
            " Coin. Total Coin: " +
            currentCoin
        );
    }

    public bool SpendCoin(int amount)
    {
        if (amount <= 0)
            return false;

        if (currentCoin < amount)
        {
            Debug.Log("Coin tidak cukup!");
            return false;
        }

        currentCoin -= amount;

        return true;
    }
}