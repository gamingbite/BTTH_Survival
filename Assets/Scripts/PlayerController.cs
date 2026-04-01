using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isFacingRight = true;

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

    [Header("Map Boundaries")]
    [Tooltip("Check this to manually input the map bounds below.")]
    public bool useCustomBounds = false;
    public float customMinX = -8.5f;
    public float customMaxX = 8.5f;
    public float customMinY = -4.5f;
    public float customMaxY = 4.5f;
    
    public SpriteRenderer mapBackground;
    private float minX, maxX, minY, maxY;

    private WeaponManager weaponManager;
    private PlayerAnimation playerAnim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.gravityScale = 0f;
        }
        weaponManager = GetComponent<WeaponManager>();
        playerAnim = GetComponent<PlayerAnimation>();
        currentHealth = maxHealth;
        
        // Update UI at start
        if (GameManager.Instance != null)
            GameManager.Instance.UpdateHP(currentHealth, maxHealth);

        if (useCustomBounds)
        {
            minX = customMinX;
            maxX = customMaxX;
            minY = customMinY;
            maxY = customMaxY;
        }
        else
        {
            if (mapBackground == null)
            {
                GameObject bgObj = GameObject.Find("Background");
                if (bgObj != null)
                    mapBackground = bgObj.GetComponent<SpriteRenderer>();
            }

            if (mapBackground != null)
            {
                float playerWidth = GetComponent<SpriteRenderer>().bounds.extents.x;
                float playerHeight = GetComponent<SpriteRenderer>().bounds.extents.y;

                minX = mapBackground.bounds.min.x + playerWidth;
                maxX = mapBackground.bounds.max.x - playerWidth;
                minY = mapBackground.bounds.min.y + playerHeight;
                maxY = mapBackground.bounds.max.y - playerHeight;
            }
            else
            {
                minX = -8.5f; maxX = 8.5f;
                minY = -4.5f; maxY = 4.5f;
            }
        }
    }

    void Update()
    {
        if (isDead) return;

        movement = Vector2.zero;
        bool shootPressed = false;
        bool switchWeaponPressed = false;

        if (UnityEngine.InputSystem.Keyboard.current != null)
        {
            if (UnityEngine.InputSystem.Keyboard.current.aKey.isPressed) movement.x -= 1f;
            if (UnityEngine.InputSystem.Keyboard.current.dKey.isPressed) movement.x += 1f;
            if (UnityEngine.InputSystem.Keyboard.current.sKey.isPressed) movement.y -= 1f;
            if (UnityEngine.InputSystem.Keyboard.current.wKey.isPressed) movement.y += 1f;

            shootPressed = UnityEngine.InputSystem.Keyboard.current.kKey.wasPressedThisFrame;
            switchWeaponPressed = UnityEngine.InputSystem.Keyboard.current.qKey.wasPressedThisFrame;
        }

        if (movement.x > 0 && !isFacingRight)
            Flip();
        else if (movement.x < 0 && isFacingRight)
            Flip();

        if (shootPressed)
        {
            if (weaponManager != null) weaponManager.TriggerShoot();
        }

        if (switchWeaponPressed)
        {
            if (weaponManager != null) weaponManager.SwitchWeapon();
        }
    }

    private bool isKnockedBack = false;

    void FixedUpdate()
    {
        if (isDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (!isKnockedBack)
        {
            rb.linearVelocity = movement.normalized * moveSpeed;
        }

        Vector2 clampedPosition = rb.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
        rb.position = clampedPosition;
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;

        if (weaponManager != null) weaponManager.UpdateWeaponDirection(isFacingRight);
    }

    public void TakeDamage(int damage, Transform attacker = null)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (GameManager.Instance != null)
            GameManager.Instance.UpdateHP(currentHealth, maxHealth);
            
        if (currentHealth <= 0) 
        {
            Die();
        }
        else
        {
            StartCoroutine(HitEffect(attacker));
        }
    }

    private System.Collections.IEnumerator HitEffect(Transform attacker)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color originalColor = Color.white;
        if (sr != null)
        {
            originalColor = sr.color;
            sr.color = new Color(1f, 0.3f, 0.3f);
        }

        if (attacker != null && rb != null)
        {
            isKnockedBack = true;
            Vector2 knockbackDir = (transform.position - attacker.position).normalized;
            if (knockbackDir == Vector2.zero) knockbackDir = Random.insideUnitCircle.normalized;
            
            // Giảm độ giật lùi xuống rất nhẹ (còn khoảng 3.5 thay vì 8)
            rb.linearVelocity = knockbackDir * 3.5f;
        }

        // Thời gian bị khựng ngắt quãng càng ngắn càng mượt
        yield return new WaitForSeconds(0.1f);

        if (sr != null) sr.color = originalColor;
        isKnockedBack = false;
    }


    public void Heal(int amount)
    {
        if (isDead) return;
        
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (GameManager.Instance != null)
            GameManager.Instance.UpdateHP(currentHealth, maxHealth);
    }


    void Die()
    {
        isDead = true;
        
        // Tắt input & movement
        rb.linearVelocity = Vector2.zero;
        
        // Tắt weapon
        if (weaponManager != null) weaponManager.enabled = false;
        
        // Tắt collider để enemy không còn tấn công
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        
        // ═══ Phát animation chết ═══
        if (playerAnim != null && playerAnim.deathSprites != null && playerAnim.deathSprites.Length > 0)
        {
            playerAnim.PlayDeath(() =>
            {
                // Callback khi animation chết xong
                OnDeathAnimationComplete();
            });
        }
        else
        {
            // Không có death sprites → xử lý ngay
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = false;
            OnDeathAnimationComplete();
        }
    }

    void OnDeathAnimationComplete()
    {
        // Hiện Game Over UI
        if (GameManager.Instance != null)
            GameManager.Instance.GameOver();
    }
}