using UnityEngine;

public class Player : Entity
{
    public float moveSpeed = 5f;
    public float diagonalMoveModifier = 0.75f;
    public float skinWidth = 0.1f;

    private Rigidbody2D rb;

    private int coins;

    private void Awake()
    {
        if(hp > 10)
        {
            maxHP = hp;
        }
        else
        {
            hp = 100;
            maxHP = hp;
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        LoadCoins();
    }

    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        RotatePlayer(moveX);
        MovePlayer(moveX, moveY);

        CheckCollision();
    }

    private void RotatePlayer(float moveX)
    {
        // Flip the player sprite depending on the direction of movement
        if (moveX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void MovePlayer(float moveX, float moveY)
    {
        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;

        if (moveX != 0 && moveY != 0)
        {
            moveDirection *= diagonalMoveModifier;
        }

        rb.linearVelocity = moveDirection * moveSpeed;
    }

    private void CheckCollision()
    {
        Vector2 position = transform.position;
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        Vector2 size = collider.size;
        Vector2 direction = rb.linearVelocity.normalized;
        float distance = skinWidth + 0.01f;

        RaycastHit2D hit = Physics2D.BoxCast(position, size, 0, direction, distance, LayerMask.GetMask("Wall"));
        if (hit)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // Method to add mana
    public void AddMana(int amount)
    {
        mp += amount;

        if (mp > maxMP)
        {
            mp = maxMP;
            return;
        }
    }

    // Method to add coins
    public void AddCoins(int amount)
    {
        coins += amount;
        SaveCoins();
    }

    // Method to get current coins
    public int GetCoins()
    {
        return coins;
    }

    // Save coins to PlayerPrefs
    private void SaveCoins()
    {
        PlayerPrefs.SetInt("PlayerCoins", coins);
        PlayerPrefs.Save();
    }

    // Load coins from PlayerPrefs
    private void LoadCoins()
    {
        coins = PlayerPrefs.GetInt("PlayerCoins", 0);
    }
}
