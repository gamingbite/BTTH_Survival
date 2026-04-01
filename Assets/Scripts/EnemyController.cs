using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public int maxHealth = 60;
    public int damageToPlayer = 10;
    
    [Header("Combat Options")]
    public float attackRange = 0.8f;
    public bool isRanged = false;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    
    [HideInInspector] public int currentHealth;
    private Transform player;
    private bool isFacingLeft = true;
    private Rigidbody2D rb;
    
    private Animator anim;
    private SpriteRenderer sr;
    private bool isDead = false;
    private EnemyHPBar hpBar;

    private float nextAttackTime = 0f;
    public float attackCooldown = 1.25f;
    private bool isAttacking = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.mass = 50f; // Trọng lượng lớn để Player khó đẩy
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = false;
        
        if (firePoint == null)
            firePoint = transform.Find("FirePoint");

        // Tìm player trong scene
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) playerObj = GameObject.Find("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        
        // Tạo HP Bar
        hpBar = EnemyHPBar.Create(transform, maxHealth);
    }

    void Update()
    {
        if (isDead) return;
        
        if (player == null)
        {
            if (anim != null) anim.SetBool("isWalking", false);
            if (rb != null) rb.linearVelocity = Vector2.zero;
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        Vector2 direction = (player.position - transform.position).normalized;

        if (distance > attackRange)
        {
            if (!isAttacking)
            {
                if (rb != null) rb.linearVelocity = direction * moveSpeed;
                if (anim != null) anim.SetBool("isWalking", true);
            }
            else
            {
                if (rb != null) rb.linearVelocity = Vector2.zero;
            }
        }
        else
        {
            if (rb != null) rb.linearVelocity = Vector2.zero;
            if (anim != null && !isAttacking) anim.SetBool("isWalking", false);
            
            if (!isAttacking && Time.time >= nextAttackTime)
            {
                StartCoroutine(AttackRoutine());
            }
        }

        // Sprite gốc (Y = 0) quay Trái.
        if (direction.x > 0 && isFacingLeft)
        {
            isFacingLeft = false;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (direction.x < 0 && !isFacingLeft)
        {
            isFacingLeft = true;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        if (anim != null) anim.SetTrigger("Attack");
        
        // Wait for the attack animation to reach the striking frame (e.g. 0.4s)
        yield return new WaitForSeconds(0.4f);
        
        if (isDead) 
        {
            isAttacking = false;
            yield break;
        }

        if (AudioManager.Instance != null) AudioManager.Instance.PlayPunch();
        
        // Check if player is still roughly in range, or just attack anyway
        float distance = player != null ? Vector2.Distance(transform.position, player.position) : 999f;
        if (distance <= attackRange + 1.5f || isRanged)
        {
            if (isRanged)
            {
                Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
                
                if (projectilePrefab != null)
                {
                    GameObject bullet = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
                    
                    Vector2 shootDir = isFacingLeft ? Vector2.left : Vector2.right;
                    
                    Bullet bulletScript = bullet.GetComponent<Bullet>();
                    EnemyBullet enemyBulletScript = bullet.GetComponent<EnemyBullet>();

                    if (enemyBulletScript != null)
                    {
                        enemyBulletScript.SetDirection(shootDir);
                    }
                    else if (bulletScript != null)
                    {
                        bulletScript.isEnemyBullet = true;
                        bulletScript.SetDirection(shootDir);
                    }
                    else
                    {
                        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                        if (rb == null) rb = bullet.AddComponent<Rigidbody2D>();
                        rb.gravityScale = 0;
                        rb.linearVelocity = shootDir * projectileSpeed;
                    }
                    
                    Destroy(bullet, 3f);
                }
                else
                {
                    if (player != null)
                    {
                        PlayerController playerScript = player.GetComponent<PlayerController>();
                        if (playerScript != null) playerScript.TakeDamage(damageToPlayer, transform);
                    }
                }
            }
            else
            {
                // CẬN CHIẾN
                if (player != null)
                {
                    PlayerController playerScript = player.GetComponent<PlayerController>();
                    if (playerScript != null)
                    {
                        playerScript.TakeDamage(damageToPlayer, transform);
                    }
                }
            }
        }

        // Wait a bit more for animation to finish
        yield return new WaitForSeconds(0.4f);
        
        nextAttackTime = Time.time + attackCooldown;
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        
        // Chỉ cập nhật HP Bar, TẮT hiển thị số sát thương
        if (hpBar != null) hpBar.UpdateHP(currentHealth);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayHit();
        
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Animation bị đánh
            if (anim != null) anim.SetTrigger("GetHit");
            // Flash đỏ khi trúng đạn
            StartCoroutine(HitFlash());
        }
    }

    IEnumerator HitFlash()
    {
        if (sr != null)
        {
            Color original = sr.color;
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            if (sr != null) sr.color = original;
        }
    }

    void Die()
    {
        isDead = true;
        
        // Vô hiệu hóa va chạm để không cản đường
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        
        // Tắt Rigidbody để không bị đẩy
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
        
        // Báo cho GameManager + WaveManager
        if (GameManager.Instance != null)
            GameManager.Instance.AddKill();
        if (WaveManager.Instance != null)
            WaveManager.Instance.OnEnemyDeath();
            
        if (anim != null)
        {
            anim.SetTrigger("Die");
            anim.SetBool("isWalking", false);
            Destroy(gameObject, 1.5f); // Chờ animation Death phát xong rồi destroy
        }
        else
        {
            // Fade out nếu không có animator
            StartCoroutine(DeathFade());
        }
    }

    IEnumerator DeathFade()
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Color startColor = sr != null ? sr.color : Color.white;
        Vector3 startScale = transform.localScale;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            if (sr != null)
                sr.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);
            transform.localScale = Vector3.Lerp(startScale, startScale * 0.3f, t);
            yield return null;
        }
        Destroy(gameObject);
    }

    void OnCollisionStay2D(Collision2D coll)
    {
        if (isDead) return;
        if (coll.gameObject.CompareTag("Player"))
        {
            if (!isAttacking && Time.time >= nextAttackTime)
            {
                if (anim != null) anim.SetBool("isWalking", false);
                StartCoroutine(AttackRoutine());
            }
        }
    }

    void OnTriggerStay2D(Collider2D coll)
    {
        if (isDead) return;
        if (coll.gameObject.CompareTag("Player"))
        {
            if (!isAttacking && Time.time >= nextAttackTime)
            {
                if (anim != null) anim.SetBool("isWalking", false);
                StartCoroutine(AttackRoutine());
            }
        }
    }
}