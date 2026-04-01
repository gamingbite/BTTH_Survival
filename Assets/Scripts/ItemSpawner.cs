using UnityEngine;
using System.Collections;

public class ItemSpawner : MonoBehaviour
{
    [Header("Item Settings")]
    public GameObject potionPrefab;
    public float spawnInterval = 15f;
    public int maxItemsAtOnce = 3;

    [Header("Map Boundaries")]
    public SpriteRenderer mapBackground;
    private float minX, maxX, minY, maxY;

    private int currentItemCount = 0;

    void Start()
    {
        // Tự động tìm Background nếu chưa gán
        if (mapBackground == null)
        {
            GameObject bgObj = GameObject.Find("Background");
            if (bgObj != null)
                mapBackground = bgObj.GetComponent<SpriteRenderer>();
        }

        if (mapBackground != null)
        {
            float margin = 1f; // Không spawn sát mép
            minX = mapBackground.bounds.min.x + margin;
            maxX = mapBackground.bounds.max.x - margin;
            minY = mapBackground.bounds.min.y + margin;
            maxY = mapBackground.bounds.max.y - margin;
            
            StartCoroutine(SpawnRoutine());
        }
        else
        {
            Debug.LogWarning("[ItemSpawner] Không tìm thấy Background. ItemSpawner sẽ không hoạt động.");
        }
    }

    IEnumerator SpawnRoutine()
    {
        // Chờ 5s ban đầu mới spawn cái đầu tiên
        yield return new WaitForSeconds(5f);

        while (true)
        {
            // Đếm số lượng potion hiện tại trên map
            currentItemCount = FindObjectsByType<HealthPotion>(FindObjectsSortMode.None).Length;

            if (currentItemCount < maxItemsAtOnce)
            {
                SpawnPotion();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnPotion()
    {
        if (potionPrefab == null)
        {
            Debug.LogWarning("[ItemSpawner] Chưa gán potionPrefab!");
            return;
        }

        // Random vị trí
        float randX = Random.Range(minX, maxX);
        float randY = Random.Range(minY, maxY);
        Vector3 spawnPos = new Vector3(randX, randY, 0f);

        Instantiate(potionPrefab, spawnPos, Quaternion.identity);
    }
}