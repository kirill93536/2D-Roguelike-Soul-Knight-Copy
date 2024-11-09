using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask playerLayer;

    private Vector2 direction;
    private float speed;
    private float range;
    private LayerMask enemyLayer;
    private int damage;

    private float distanceTravelled;

    public void Launch(Vector2 direction, float speed, float range, int damage, LayerMask enemyLayer = default)
    {
        this.direction = direction;
        this.speed = speed;
        this.range = range;
        this.enemyLayer = enemyLayer;
        this.damage = damage;

        distanceTravelled = 0f;
    }

    private void Update()
    {
        float distance = speed * Time.deltaTime;
        distanceTravelled += distance;

        if (distanceTravelled >= range)
        {
            gameObject.SetActive(false);
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, enemyLayer);
        if (hit.collider != null)
        {
            hit.collider.GetComponent<Entity>().TakeDamage(damage);
            gameObject.SetActive(false);
        }

        // Move the projectile based on its speed and direction
        transform.position = Vector3.MoveTowards(transform.position, transform.position + (Vector3)direction, speed * Time.deltaTime);
    }
}
