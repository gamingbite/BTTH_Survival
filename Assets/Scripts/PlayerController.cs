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

    private WeaponManager weaponManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        weaponManager = GetComponent<WeaponManager>();
        currentHealth = maxHealth;
    }

    void Update()
    {
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

    void FixedUpdate()
    {
        rb.linearVelocity = movement.normalized * moveSpeed;
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;

        if (weaponManager != null) weaponManager.UpdateWeaponDirection(isFacingRight);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (GameManager.Instance != null)
            GameManager.Instance.UpdateHP(currentHealth, maxHealth);
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.GameOver();
        Destroy(gameObject);
    }
}