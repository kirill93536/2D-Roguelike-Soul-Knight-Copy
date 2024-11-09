using TMPro;
using UnityEngine;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI mpText;

    private int previousHp;
    private int previousMp;

    private Player player;

    private void Start()
    {
        player = GetComponent<Player>();
        previousHp = player.hp;
        previousMp = player.mp;

        hpText.text = "HP: " + previousHp.ToString();
        mpText.text = "MP: " + previousMp.ToString();
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
    }
}
