using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    [Header("Settings")]
    public int healAmount = 25;
    public float lifetime = 20f; // Tự hủy sau 20 giây nếu không ai nhặt

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Heal(healAmount);
                // Có thể thêm particle effect hoặc âm thanh ở đây
                Destroy(gameObject);
            }
        }
    }
}