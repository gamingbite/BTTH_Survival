using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite[] idleSprites;
    public Sprite[] walkSprites;
    
    public float frameRate = 0.1f;
    private float timer;
    private int currentFrame;
    private Rigidbody2D rb;

    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (spriteRenderer == null || idleSprites.Length == 0 || walkSprites.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= frameRate)
        {
            timer -= frameRate;
            currentFrame++;

            bool isWalking = Mathf.Abs(rb.linearVelocity.x) > 0.1f || Mathf.Abs(rb.linearVelocity.y) > 0.1f;
            
            Sprite[] currentArray = isWalking ? walkSprites : idleSprites;
            if (currentFrame >= currentArray.Length) currentFrame = 0;
            
            spriteRenderer.sprite = currentArray[currentFrame];
        }
    }
}