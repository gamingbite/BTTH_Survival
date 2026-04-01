using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class GameUIGenerator : EditorWindow
{
    [MenuItem("Tools/Generate Game Over UI")]
    public static void GenerateGameOverUI()
    {
        // 1. Tạo hoặc lấy Canvas hiện có
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // 2. Load Assets
        Sprite bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Mad Doctor Assets/Sprites/User Interfaces/Scorebox.png");
        if (bgSprite == null) 
            bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Mad Doctor Assets/Sprites/User Interfaces/PopUPbox.png");
            
        Sprite btnSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Mad Doctor Assets/Sprites/User Interfaces/Btn1.png");

        // Tìm Font "SHOWG SDF" nếu đã được tạo từ Font Asset Creator
        string[] fontGuids = AssetDatabase.FindAssets("SHOWG t:TMP_FontAsset");
        TMP_FontAsset fontAsset = null;
        if (fontGuids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(fontGuids[0]);
            fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
        }

        // 3. Tạo Panel nền Game Over
        GameObject panelObj = new GameObject("GameOverPanel");
        panelObj.transform.SetParent(canvas.transform, false);
        Image panelImg = panelObj.AddComponent<Image>();
        if (bgSprite != null) panelImg.sprite = bgSprite;
        panelImg.type = Image.Type.Sliced;
        
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(1000, 600); // Kích thước tương tự trong ảnh
        panelRect.anchoredPosition = Vector2.zero;

        // 4. Tạo đoạn Text điểm
        GameObject textObj = new GameObject("GameOverText");
        textObj.transform.SetParent(panelObj.transform, false);
        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.text = "GAME OVER!\n\nSCORE: 0\n\nHIGHSCORE: 0";
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.color = Color.white;
        tmpText.fontSize = 60;
        if (fontAsset != null) tmpText.font = fontAsset;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 0);
        textRect.anchorMax = new Vector2(1, 1);
        textRect.sizeDelta = new Vector2(-40, -100); // Cách mép
        textRect.anchoredPosition = new Vector2(0, 50);

        // 5. Tạo Nút Play Again
        GameObject btnObj = new GameObject("PlayAgainButton");
        btnObj.transform.SetParent(panelObj.transform, false);
        Image btnImg = btnObj.AddComponent<Image>();
        if (btnSprite != null) btnImg.sprite = btnSprite;
        btnImg.type = Image.Type.Sliced;
        Button btn = btnObj.AddComponent<Button>();

        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0f);
        btnRect.anchorMax = new Vector2(0.5f, 0f);
        btnRect.sizeDelta = new Vector2(350, 100);
        btnRect.anchoredPosition = new Vector2(0, 120); // Cách lề dưới một chút

        // 6. Tạo Text cho nút
        GameObject btnTextObj = new GameObject("Text (TMP)");
        btnTextObj.transform.SetParent(btnObj.transform, false);
        TextMeshProUGUI btnTmpText = btnTextObj.AddComponent<TextMeshProUGUI>();
        btnTmpText.text = "PLAY AGAIN";
        btnTmpText.alignment = TextAlignmentOptions.Center;
        btnTmpText.color = Color.black;
        btnTmpText.fontSize = 45;
        if (fontAsset != null) btnTmpText.font = fontAsset;

        RectTransform btnTextRect = btnTextObj.GetComponent<RectTransform>();
        btnTextRect.anchorMin = new Vector2(0, 0);
        btnTextRect.anchorMax = new Vector2(1, 1);
        btnTextRect.sizeDelta = Vector2.zero;
        btnTextRect.anchoredPosition = Vector2.zero;

        // Đảm bảo EventSystem được tạo ra để nút có thể click
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject evtSystemObj = new GameObject("EventSystem");
            evtSystemObj.AddComponent<EventSystem>();
            evtSystemObj.AddComponent<StandaloneInputModule>();
        }

        // Tạo hành động Undo trong Editor để ctrl+Z được
        Undo.RegisterCreatedObjectUndo(panelObj, "Generate GameOver UI");
        Selection.activeGameObject = panelObj;
        
        Debug.Log("Đã tạo Game Over UI thành công! Nút và UI đã tự động nhận asset.");
        if (fontAsset == null)
        {
            Debug.LogWarning("Chưa tìm thấy Font Asset cho TextMeshPro. Cần phải tạo Font Asset từ tệp SHOWG.TTF sử dụng menu Window > TextMeshPro > Font Asset Creator.");
        }
    }
}
