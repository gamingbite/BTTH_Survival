using UnityEngine;

/// <summary>
/// Enemy AI dạng Ranged: đuổi player đến tầm stopRange, rồi đứng yên bắn đạn.
/// State Machine: Chase ↔ Shoot
/// </summary>
public class EnemyRangedController : MonoBehaviour
{
    // ─── Stats ─────────────────────────────────────────────────────
    [Header("Stats")]
    public int maxHealth = 40;
    public float moveSpeed = 2.5f;

    // ─── Ranged Settings ───────────────────────────────────────────
    [Header("Ranged Settings")]
    [Tooltip("Khoảng cách dừng lại để bắn (units)")]
    public float stopRange = 5f;
    [Tooltip("Thời gian giữa 2 lần bắn (giây)")]
    public float fireRate = 1.5f;
    [Tooltip("Prefab đạn EnemyBullet")]
    public GameObject bulletPrefab;
    [Tooltip("Điểm xuất phát đạn (child object)")]
    public Transform firePoint;

    // ─── Internal ──────────────────────────────────────────────────
    private int currentHealth;
    private Transform player;
    private bool isFacingRight = true;
    private float nextFireTime = 0f;
    private bool isDead = false;
    private Rigidbody2D rb;

    private Animator anim;
    private SpriteRenderer sr;
    private EnemyHPBar hpBar;

    private enum State { Chase, Shoot }
    private State currentState = State.Chase;

    // ───────────────────────────────────────────────────────────────

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.freezeRotation = true;
            rb.gravityScale = 0f;
            rb.mass = 50f; // Trọng lượng lớn để người chơi khó đẩy
        }
        
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = false;

        // Tìm player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) playerObj = GameObject.Find("Player");
        if (playerObj != null) player = playerObj.transform;

        // Tự tìm FirePoint nếu chưa gán
        if (firePoint == null)
            firePoint = transform.Find("FirePoint");

        // Nếu vẫn null → dùng chính transform làm firePoint
        if (firePoint == null)
            firePoint = transform;
        
        // Tạo HP Bar
        hpBar = EnemyHPBar.Create(transform, maxHealth);
    }

    void Update()
    {
        if (isDead || player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // Chuyển state theo khoảng cách
        currentState = (dist <= stopRange) ? State.Shoot : State.Chase;

        switch (currentState)
        {
            case State.Chase:
                ChasePlayer();
                if (anim != null) anim.SetBool("isWalking", true);
                break;
            case State.Shoot:
                ShootAtPlayer();
                if (anim != null) anim.SetBool("isWalking", false);
                break;
        }

        // Lật sprite theo hướng player
        FlipTowardPlayer();
    }

    // ─── Chase ─────────────────────────────────────────────────────
    void ChasePlayer()
    {
        if (rb != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
        }
    }

    // ─── Shoot ─────────────────────────────────────────────────────
    void ShootAtPlayer()
    {
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // Đứng yên, bắn theo fireRate
        if (Time.time >= nextFireTime)
        {
            FireBullet();
            nextFireTime = Time.time + fireRate;
        }
    }

    private bool isFacingLeft = true;

    void FlipTowardPlayer()
    {
        bool playerIsOnRight = player.position.x > transform.position.x;
        if (playerIsOnRight && isFacingLeft)
        {
            isFacingLeft = false;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (!playerIsOnRight && !isFacingLeft)
        {
            isFacingLeft = true;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }


    void FireBullet()
    {
        if (bulletPrefab == null || firePoint == null) return;

        if (anim != null) anim.SetTrigger("Attack");
        if (AudioManager.Instance != null) AudioManager.Instance.PlayShoot();
        
        // Bắn thẳng ngang theo hướng nhìn (trái/phải)
        Vector2 direction = isFacingLeft ? Vector2.left : Vector2.right;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        EnemyBullet enemyBulletScript = bullet.GetComponent<EnemyBullet>();
        Bullet commonBullet = bullet.GetComponent<Bullet>();
        
        if (enemyBulletScript != null)
        {
            enemyBulletScript.SetDirection(direction);
        }
        else if (commonBullet != null)
        {
            commonBullet.isEnemyBullet = true;
            commonBullet.SetDirection(direction);
        }
        else
        {
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb == null) bulletRb = bullet.AddComponent<Rigidbody2D>();
            bulletRb.gravityScale = 0;
            bulletRb.linearVelocity = direction * 10f;
        }

        Destroy(bullet, 3f);
    }

    // ─── Flip ──────────────────────────────────────────────────────


    // ─── Health ────────────────────────────────────────────────────
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
            // Flash đỏ khi trúng đạn
            if (anim != null) anim.SetTrigger("GetHit");
            StartCoroutine(HitFlash());
        }
    }

    System.Collections.IEnumerator HitFlash()
    {
        if (sr != null)
        {
            Color original = sr.color;
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = original;
        }
    }

    void Die()
    {
        isDead = true;
        
        // Vô hiệu hóa va chạm
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        
        // Tắt Rigidbody để không bị đẩy
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
        
        if (GameManager.Instance != null)
            GameManager.Instance.AddKill();
        if (WaveManager.Instance != null)
            WaveManager.Instance.OnEnemyDeath();

        if (anim != null)
        {
            anim.SetTrigger("Die");
            anim.SetBool("isWalking", false);
            Destroy(gameObject, 1.5f);
        }
        else
        {
            // Fade out nếu không có animator
            StartCoroutine(DeathFade());
        }
    }

    System.Collections.IEnumerator DeathFade()
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

    // ─── Melee fallback ────────────────────────────────────────────
    // Nếu enemy Ranged vô tình bị player chạm vào, vẫn gây dame nhỏ
    [Header("Melee Fallback (va chạm)")]
    public int contactDamage = 5;

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (isDead) return;
        if (coll.gameObject.CompareTag("Player"))
        {
            PlayerController p = coll.gameObject.GetComponent<PlayerController>();
            if (p != null) p.TakeDamage(contactDamage);
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (isDead) return;
        if (coll.gameObject.CompareTag("Player"))
        {
            PlayerController p = coll.gameObject.GetComponent<PlayerController>();
            if (p != null) p.TakeDamage(contactDamage);
        }
    }

    // ─── Gizmos (debug range) ──────────────────────────────────────
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopRange);
    }
}
