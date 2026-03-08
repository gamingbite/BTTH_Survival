using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script tự động tạo và cấu hình toàn bộ HUD (HP Bar, Score Box) khi bắt đầu game.
/// Gắn script này vào GameManager hoặc một GameObject trống trên Scene.
/// </summary>
[DefaultExecutionOrder(-10)]
public class HUDSetup : MonoBehaviour
{
    void Awake()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        SetupHPBar(canvas);
        SetupScoreBox(canvas);
    }

    void SetupHPBar(Canvas canvas)
    {
        // Xóa HPContainer cũ nếu tồn tại
        var old = canvas.transform.Find("HPContainer");
        if (old != null) Destroy(old.gameObject);

        // Tạo HPContainer — neo góc trên trái
        GameObject container = new GameObject("HPContainer");
        container.transform.SetParent(canvas.transform, false);
        RectTransform rt = container.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(15, -15);
        rt.sizeDelta = new Vector2(280, 60);

        // Icon Tim
        GameObject iconObj = new GameObject("HPIcon");
        iconObj.transform.SetParent(container.transform, false);
        RectTransform iconRt = iconObj.AddComponent<RectTransform>();
        iconRt.anchorMin = new Vector2(0, 0.5f);
        iconRt.anchorMax = new Vector2(0, 0.5f);
        iconRt.pivot = new Vector2(0, 0.5f);
        iconRt.anchoredPosition = new Vector2(0, 0);
        iconRt.sizeDelta = new Vector2(45, 45);
        Image iconImg = iconObj.AddComponent<Image>();
        iconImg.sprite = Resources.Load<Sprite>("HPIcon") ?? LoadSprite("Assets/Mad Doctor Assets/Sprites/User Interfaces/HpICon.png");

        // BG Thanh máu
        GameObject barBg = new GameObject("HPBarBg");
        barBg.transform.SetParent(container.transform, false);
        RectTransform barBgRt = barBg.AddComponent<RectTransform>();
        barBgRt.anchorMin = new Vector2(0, 0.5f);
        barBgRt.anchorMax = new Vector2(1, 0.5f);
        barBgRt.pivot = new Vector2(0, 0.5f);
        barBgRt.anchoredPosition = new Vector2(50, 0);
        barBgRt.sizeDelta = new Vector2(-50, 20);
        Image barBgImg = barBg.AddComponent<Image>();
        barBgImg.color = new Color(0.2f, 0.2f, 0.2f, 0.7f);

        // Thanh màu xanh (Fill)
        GameObject barFill = new GameObject("HPBarFill");
        barFill.transform.SetParent(barBg.transform, false);
        RectTransform barFillRt = barFill.AddComponent<RectTransform>();
        barFillRt.anchorMin = Vector2.zero;
        barFillRt.anchorMax = Vector2.one;
        barFillRt.offsetMin = Vector2.zero;
        barFillRt.offsetMax = Vector2.zero;
        Image fillImg = barFill.AddComponent<Image>();
        fillImg.color = new Color(0.2f, 0.85f, 0.2f);
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Horizontal;
        fillImg.fillAmount = 1f;

        // Text số HP
        GameObject hpTextObj = new GameObject("HPText");
        hpTextObj.transform.SetParent(container.transform, false);
        RectTransform hpTextRt = hpTextObj.AddComponent<RectTransform>();
        hpTextRt.anchorMin = new Vector2(0, 0);
        hpTextRt.anchorMax = new Vector2(1, 0);
        hpTextRt.pivot = new Vector2(0.5f, 1);
        hpTextRt.anchoredPosition = new Vector2(0, -5);
        hpTextRt.sizeDelta = new Vector2(0, 20);
        TextMeshProUGUI hpTmp = hpTextObj.AddComponent<TextMeshProUGUI>();
        hpTmp.text = "100/100";
        hpTmp.fontSize = 14;
        hpTmp.alignment = TextAlignmentOptions.Center;
        hpTmp.color = Color.white;
    }

    void SetupScoreBox(Canvas canvas)
    {
        var oldKills = canvas.transform.Find("KillsText");
        // KillsText đã có → chỉ đảm bảo vị trí đúng
        if (oldKills != null)
        {
            RectTransform rt = oldKills.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(0, 1);
                rt.pivot = new Vector2(0, 1);
                rt.anchoredPosition = new Vector2(15, -80);
                rt.sizeDelta = new Vector2(200, 30);
            }
        }

        // ScoreBox
        var old = canvas.transform.Find("ScoreBox");
        if (old != null) Destroy(old.gameObject);

        GameObject scoreBox = new GameObject("ScoreBox");
        scoreBox.transform.SetParent(canvas.transform, false);
        RectTransform srt = scoreBox.AddComponent<RectTransform>();
        srt.anchorMin = new Vector2(1, 1);
        srt.anchorMax = new Vector2(1, 1);
        srt.pivot = new Vector2(1, 1);
        srt.anchoredPosition = new Vector2(-15, -15);
        srt.sizeDelta = new Vector2(200, 60);
        Image sImg = scoreBox.AddComponent<Image>();
        sImg.color = new Color(0, 0, 0, 0.5f);

        GameObject scoreText = new GameObject("ScoreText");
        scoreText.transform.SetParent(scoreBox.transform, false);
        RectTransform strt = scoreText.AddComponent<RectTransform>();
        strt.anchorMin = Vector2.zero;
        strt.anchorMax = Vector2.one;
        strt.offsetMin = new Vector2(5, 5);
        strt.offsetMax = new Vector2(-5, -5);
        TextMeshProUGUI sTmp = scoreText.AddComponent<TextMeshProUGUI>();
        sTmp.text = "SCORE: 0";
        sTmp.fontSize = 18;
        sTmp.alignment = TextAlignmentOptions.Center;
        sTmp.color = Color.yellow;
    }

    Sprite LoadSprite(string path)
    {
#if UNITY_EDITOR
        return UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
#else
        return null;
#endif
    }
}
