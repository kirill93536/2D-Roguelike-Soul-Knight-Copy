using TMPro;
using UnityEngine;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI mpText;
    [SerializeField] private TextMeshProUGUI coinsText; // New UI element for coins

    private int previousHp;
    private int previousMp;
    private int previousCoins; // To track changes in coins

    private Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        previousHp = player.hp;
        previousMp = player.mp;
        previousCoins = player.GetCoins();

        hpText.text = "HP: " + previousHp.ToString();
        mpText.text = "MP: " + previousMp.ToString();
        UpdateCoinsUI();
    }

    private void Update()
    {
        if (player.hp != previousHp)
        {
            hpText.text = "HP: " + player.hp;
            previousHp = player.hp;
        }

        if (player.mp != previousMp)
        {
            mpText.text = "MP: " + player.mp;
            previousMp = player.mp;
        }

        if (player.GetCoins() != previousCoins)
        {
            UpdateCoinsUI();
            previousCoins = player.GetCoins();
        }
    }

    private void UpdateCoinsUI()
    {
        coinsText.text = "Coins: " + player.GetCoins();
    }
}
