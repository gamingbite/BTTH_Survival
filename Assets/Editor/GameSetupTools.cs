using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class GameSetupTools
{
    // ==================================
    // ENEMY SETUP
    // ==================================
    [MenuItem("Tools/Mad Doctor/Create Enemy Prefabs (02-06)")]
    public static void CreateEnemyPrefabs()
    {
        string[] enemyFolders  = { "Enemy Character02", "Enemy Character03", "Enemy Character04", "Enemy Character05", "Enemy Character06" };
        string[]  names        = { "Enemy02", "Enemy03", "Enemy04", "Enemy05", "Enemy06" };
        int[]     healths      = { 40, 60, 30, 80, 100 };
        float[]   speeds       = { 2.5f, 2.0f, 3.5f, 1.5f, 1.2f };
        int[]     damages      = { 12, 15, 10, 20, 25 };
        Color[]   tints = {
            Color.white,
            new Color(1f,0.8f,0.8f),
            new Color(0.8f,1f,0.8f),
            new Color(0.8f,0.8f,1f),
            new Color(1f,0.7f,0.3f)
        };

        string basePath   = "Assets/Mad Doctor Assets/Sprites/Enemy";
        string prefabPath = "Assets/Mad Doctor Assets/Prefabs";

        // Tạo thư mục Prefabs nếu chưa có
        if (!AssetDatabase.IsValidFolder(prefabPath))
            AssetDatabase.CreateFolder("Assets/Mad Doctor Assets", "Prefabs");

        for (int i = 0; i < 5; i++)
        {
            string folder  = enemyFolders[i];
            GameObject go  = new GameObject(names[i]);

            // SpriteRenderer
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            Sprite idleFirst = AssetDatabase.LoadAssetAtPath<Sprite>($"{basePath}/{folder}/Idle/Idle_00.png");
            if (idleFirst != null) sr.sprite = idleFirst;
            sr.color = tints[i];

            // Physics
            Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.constraints  = RigidbodyConstraints2D.FreezeRotation;
            go.AddComponent<BoxCollider2D>();

            // EnemyController
            EnemyController ec = go.AddComponent<EnemyController>();
            ec.maxHealth       = healths[i];
            ec.moveSpeed       = speeds[i];
            ec.damageToPlayer  = damages[i];

            // PlayerAnimation – dùng lại để chạy sprite frames
            PlayerAnimation pa = go.AddComponent<PlayerAnimation>();

            // Load Idle frames
            var idleList = new System.Collections.Generic.List<Sprite>();
            for (int f = 0; f <= 13; f++)
            {
                var s = AssetDatabase.LoadAssetAtPath<Sprite>($"{basePath}/{folder}/Idle/Idle_{f:D2}.png");
                if (s != null) idleList.Add(s);
            }
            pa.idleSprites = idleList.ToArray();

            // Load Walk frames
            var walkList = new System.Collections.Generic.List<Sprite>();
            for (int f = 0; f <= 13; f++)
            {
                var s = AssetDatabase.LoadAssetAtPath<Sprite>($"{basePath}/{folder}/Walk/Walk_{f:D2}.png");
                if (s != null) walkList.Add(s);
            }
            pa.walkSprites = walkList.ToArray();

            // Lưu Prefab
            string savePath = $"{prefabPath}/{names[i]}.prefab";
            PrefabUtility.SaveAsPrefabAsset(go, savePath);
            Object.DestroyImmediate(go);

            Debug.Log($"[GameSetupTools] Created {savePath} | Idle:{idleList.Count} Walk:{walkList.Count}");
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Done!", "5 Enemy Prefabs created successfully!", "OK");
    }

    // ==================================
    // WEAPON SETUP
    // ==================================
    [MenuItem("Tools/Mad Doctor/Setup 10 Weapons on Player")]
    public static void SetupWeapons()
    {
        GameObject playerGO = GameObject.Find("Player");
        if (playerGO == null) { Debug.LogError("Player not found in scene!"); return; }

        WeaponManager wm = playerGO.GetComponent<WeaponManager>();
        if (wm == null) { Debug.LogError("WeaponManager not found on Player!"); return; }

        string charBase   = "Assets/Mad Doctor Assets/Sprites/Mad Doctor - Main Character";
        string bulletBase = "Assets/Mad Doctor Assets/Sprites/Bullets";
        string prefabBase = "Assets/Mad Doctor Assets/Prefabs";

        string[] weaponNames = {
            "Pistol I", "Plasma Cannon", "Rifle II", "Rapid Fire",
            "Shotgun", "Mini Gun", "Laser Blaster", "Burst Rifle",
            "Heavy Canon", "Death Ray"
        };
        float[] fireRates = { 0.5f, 0.3f, 0.4f, 0.25f, 0.6f, 0.2f, 0.45f, 0.35f, 0.55f, 0.15f };
        string[] bulletPrefabs = {
            "Bullet_1", "Bullet_2", "Bullet_1", "Bullet_2", "Bullet_1",
            "Bullet_2", "Bullet_1", "Bullet_2", "Bullet_1", "Bullet_2"
        };
        string[] bulletIcons = {
            "Bullet 1","Bullet 2","Bullet 3","Bullet 4","Bullet 5",
            "Bullet 6","Bullet 7","Bullet 8","Bullet 9","Bullet 10"
        };

        wm.weapons.Clear();

        for (int g = 1; g <= 10; g++)
        {
            string gunFolder = $"Gun{g:D2}";
            var w = new WeaponManager.WeaponInfo();
            w.weaponName  = weaponNames[g - 1];
            w.fireRate    = fireRates[g - 1];
            w.bulletPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{prefabBase}/{bulletPrefabs[g - 1]}.prefab");
            w.weaponIcon   = AssetDatabase.LoadAssetAtPath<Sprite>($"{bulletBase}/{bulletIcons[g - 1]}.png");

            // Idle frames
            var idle = new System.Collections.Generic.List<Sprite>();
            for (int f = 0; f <= 13; f++)
            {
                var s = AssetDatabase.LoadAssetAtPath<Sprite>($"{charBase}/{gunFolder}/Idle/Idle_{f:D2}.png");
                if (s != null) idle.Add(s);
            }
            w.idleSprites = idle.ToArray();

            // Walk frames
            var walk = new System.Collections.Generic.List<Sprite>();
            for (int f = 0; f <= 13; f++)
            {
                var s = AssetDatabase.LoadAssetAtPath<Sprite>($"{charBase}/{gunFolder}/Walk/Walk_{f:D2}.png");
                if (s != null) walk.Add(s);
            }
            w.walkSprites = walk.ToArray();

            wm.weapons.Add(w);
            Debug.Log($"[{w.weaponName}] Idle:{idle.Count} Walk:{walk.Count}");
        }

        EditorUtility.SetDirty(wm);
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Done!", "10 Weapons configured on Player!", "OK");
    }

    // ==================================
    // XOÁ SETUP COMPONENTS
    // ==================================
    [MenuItem("Tools/Mad Doctor/Remove Setup Helpers")]
    public static void RemoveSetupHelpers()
    {
        // Không cần EnemySetup và WeaponSetup nữa sau khi dùng xong
        Debug.Log("Setup helpers removed.");
    }
}
#endif
