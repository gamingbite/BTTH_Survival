using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public int maxHealth = 60;
    public int damageToPlayer = 10;
    
    private int currentHealth;
    private Transform player;
    private bool isFacingRight = true;

    void Start()
    {
        currentHealth = maxHealth;
        // Tìm player trong scene
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        // Di chuyển về phía player
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        // Xoay mặt
        if (direction.x > 0 && !isFacingRight) Flip();
        else if (direction.x < 0 && isFacingRight) Flip();
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Thêm tính năng báo cho GameManager số Kill ở đây
        if (GameManager.Instance != null)
            GameManager.Instance.AddKill();
            
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        // Gây sát thương khi chạm vào Player
        if (coll.gameObject.CompareTag("Player"))
        {
            PlayerController playerScript = coll.gameObject.GetComponent<PlayerController>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(damageToPlayer);
            }
        }
    }
}