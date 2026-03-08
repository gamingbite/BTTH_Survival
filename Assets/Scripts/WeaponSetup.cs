using UnityEngine;

/// <summary>
/// Script chạy một lần trong Editor để gán 10 bộ Sprites súng vào WeaponManager.
/// Gắn vào Player và chọn "Setup All 10 Weapons" từ context menu.
/// </summary>
public class WeaponSetup : MonoBehaviour
{
#if UNITY_EDITOR
    [ContextMenu("Setup All 10 Weapons")]
    public void SetupAllWeapons()
    {
        WeaponManager wm = GetComponent<WeaponManager>();
        if (wm == null) { Debug.LogError("WeaponManager not found!"); return; }

        string basePath = "Assets/Mad Doctor Assets/Sprites/Mad Doctor - Main Character";
        string bulletBasePath = "Assets/Mad Doctor Assets/Sprites/Bullets";

        // Bullet prefabs
        var bulletPaths = new string[]
        {
            "Assets/Mad Doctor Assets/Prefabs/Bullet_1.prefab",
            "Assets/Mad Doctor Assets/Prefabs/Bullet_2.prefab",
            "Assets/Mad Doctor Assets/Prefabs/Bullet_1.prefab",
            "Assets/Mad Doctor Assets/Prefabs/Bullet_2.prefab",
            "Assets/Mad Doctor Assets/Prefabs/Bullet_1.prefab",
            "Assets/Mad Doctor Assets/Prefabs/Bullet_2.prefab",
            "Assets/Mad Doctor Assets/Prefabs/Bullet_1.prefab",
            "Assets/Mad Doctor Assets/Prefabs/Bullet_2.prefab",
            "Assets/Mad Doctor Assets/Prefabs/Bullet_1.prefab",
            "Assets/Mad Doctor Assets/Prefabs/Bullet_2.prefab",
        };

        float[] fireRates = { 0.5f, 0.3f, 0.4f, 0.25f, 0.6f, 0.2f, 0.45f, 0.35f, 0.55f, 0.15f };
        string[] weaponNames = {
            "Pistol Mark I", "Plasma Cannon", "Rifle Mark II", "Rapid Fire",
            "Shotgun", "Mini Gun", "Laser Blaster", "Burst Rifle",
            "Heavy Canon", "Death Ray"
        };

        string[] bulletIcons = {
            "Bullet 1.png", "Bullet 2.png", "Bullet 3.png", "Bullet 4.png", "Bullet 5.png",
            "Bullet 6.png", "Bullet 7.png", "Bullet 8.png", "Bullet 9.png", "Bullet 10.png"
        };

        wm.weapons.Clear();

        for (int gunIdx = 1; gunIdx <= 10; gunIdx++)
        {
            string gunFolder = $"Gun{gunIdx:D2}";
            var weapon = new WeaponManager.WeaponInfo();
            weapon.weaponName = weaponNames[gunIdx - 1];
            weapon.fireRate = fireRates[gunIdx - 1];

            // Bullet prefab
            weapon.bulletPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(bulletPaths[gunIdx - 1]);

            // Weapon icon (bullet sprite)
            weapon.weaponIcon = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>($"{bulletBasePath}/{bulletIcons[gunIdx - 1]}");

            // Idle sprites
            var idleList = new System.Collections.Generic.List<Sprite>();
            for (int f = 0; f <= 13; f++)
            {
                Sprite s = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>($"{basePath}/{gunFolder}/Idle/Idle_{f:D2}.png");
                if (s != null) idleList.Add(s);
            }
            weapon.idleSprites = idleList.ToArray();

            // Walk sprites
            var walkList = new System.Collections.Generic.List<Sprite>();
            for (int f = 0; f <= 13; f++)
            {
                Sprite s = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>($"{basePath}/{gunFolder}/Walk/Walk_{f:D2}.png");
                if (s != null) walkList.Add(s);
            }
            weapon.walkSprites = walkList.ToArray();

            wm.weapons.Add(weapon);
            Debug.Log($"[{weapon.weaponName}] Idle:{idleList.Count} Walk:{walkList.Count} sprites loaded.");
        }

        UnityEditor.EditorUtility.SetDirty(wm);
        Debug.Log("All 10 weapons configured on WeaponManager!");
    }
#endif
}
