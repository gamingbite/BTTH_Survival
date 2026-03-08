using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Script chạy một lần trong Editor để tạo 5 Enemy Prefab mới từ asset sprites.
/// Gắn vào bất kỳ GameObject và chạy qua context menu.
/// </summary>
public class EnemySetup : MonoBehaviour
{
    // Danh sách thông số từng loại Enemy
    [System.Serializable]
    public class EnemyConfig
    {
        public string enemyName;
        public int health = 30;
        public float speed = 2f;
        public int damage = 10;
        public Color tintColor = Color.white;
    }

    public EnemyConfig[] configs = new EnemyConfig[]
    {
        new EnemyConfig { enemyName = "Enemy02", health = 40, speed = 2.5f, damage = 12, tintColor = Color.white },
        new EnemyConfig { enemyName = "Enemy03", health = 60, speed = 2.0f, damage = 15, tintColor = new Color(1f, 0.8f, 0.8f) },
        new EnemyConfig { enemyName = "Enemy04", health = 30, speed = 3.5f, damage = 10, tintColor = new Color(0.8f, 1f, 0.8f) },
        new EnemyConfig { enemyName = "Enemy05", health = 80, speed = 1.5f, damage = 20, tintColor = new Color(0.8f, 0.8f, 1f) },
        new EnemyConfig { enemyName = "Enemy06", health = 100, speed = 1.2f, damage = 25, tintColor = new Color(1f, 0.7f, 0.3f) },
    };

#if UNITY_EDITOR
    [ContextMenu("Create Enemy Prefabs")]
    public void CreateEnemyPrefabs()
    {
        string[] enemyFolders = { "Enemy Character02", "Enemy Character03", "Enemy Character04", "Enemy Character05", "Enemy Character06" };
        string basePath = "Assets/Mad Doctor Assets/Sprites/Enemy";
        string prefabPath = "Assets/Mad Doctor Assets/Prefabs";

        for (int i = 0; i < 5; i++)
        {
            string folder = enemyFolders[i];
            var cfg = configs[i];

            // Tạo GameObject tạm thời
            GameObject enemy = new GameObject(cfg.enemyName);
            
            // SpriteRenderer
            SpriteRenderer sr = enemy.AddComponent<SpriteRenderer>();
            // Load Idle sprite đầu tiên
            Sprite idleSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>($"{basePath}/{folder}/Idle/Idle_00.png");
            if (idleSprite != null) sr.sprite = idleSprite;
            sr.color = cfg.tintColor;

            // Rigidbody2D
            Rigidbody2D rb = enemy.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            // BoxCollider2D
            BoxCollider2D col = enemy.AddComponent<BoxCollider2D>();

            // EnemyController
            EnemyController ec = enemy.AddComponent<EnemyController>();
            ec.maxHealth = cfg.health;
            ec.moveSpeed = cfg.speed;
            ec.damageToPlayer = cfg.damage;

            // PlayerAnimation để chạy Idle/Walk sprites
            PlayerAnimation pa = enemy.AddComponent<PlayerAnimation>();

            // Nạp Idle Sprites
            var idleList = new System.Collections.Generic.List<Sprite>();
            for (int f = 0; f <= 13; f++)
            {
                Sprite s = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>($"{basePath}/{folder}/Idle/Idle_{f:D2}.png");
                if (s != null) idleList.Add(s);
            }
            pa.idleSprites = idleList.ToArray();

            // Nạp Walk Sprites
            var walkList = new System.Collections.Generic.List<Sprite>();
            for (int f = 0; f <= 13; f++)
            {
                Sprite s = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>($"{basePath}/{folder}/Walk/Walk_{f:D2}.png");
                if (s != null) walkList.Add(s);
            }
            pa.walkSprites = walkList.ToArray();

            // Lưu Prefab
            string fullPrefabPath = $"{prefabPath}/{cfg.enemyName}.prefab";
            UnityEditor.PrefabUtility.SaveAsPrefabAsset(enemy, fullPrefabPath);
            DestroyImmediate(enemy);

            Debug.Log($"Created prefab: {fullPrefabPath}");
        }

        UnityEditor.AssetDatabase.Refresh();
        Debug.Log("All 5 enemy prefabs created!");
    }
#endif
}
