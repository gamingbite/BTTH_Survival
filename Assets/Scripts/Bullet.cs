using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 20;
    public float lifeTime = 3f; // Thời gian tối đa xóa đạn
    
    private Rigidbody2D rb;
    private Vector2 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime); // Xóa tự động sau duration
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * speed;
    }

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
        
        // Xoay đạn nếu bắn ngược lại
        if (moveDirection.x < 0)
        {
             Vector3 scaler = transform.localScale;
             scaler.x *= -1;
             transform.localScale = scaler;
        }
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        EnemyController enemy = hitInfo.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
        
        // Cứ chạm cái gì có rigid thì xóa
        Destroy(gameObject);
    }
}