using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Stats")]
    public int score = 0;
    public int kills = 0;
    
    [Header("UI Dependencies")]
    public TextMeshProUGUI killsText;
    public Image hpBarFill;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public Button playAgainBtn;
    public Image weaponIcon;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI scoreText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Auto assign UI Elements if not manually set
        if (killsText == null)
        {
            GameObject txtObj = GameObject.Find("KillsText");
            if (txtObj != null) killsText = txtObj.GetComponent<TextMeshProUGUI>();
        }
        if (hpBarFill == null)
        {
            GameObject hpObj = GameObject.Find("HPBarFill");
            if (hpObj != null) hpBarFill = hpObj.GetComponent<Image>();
        }
        if (gameOverPanel == null)
        {
            gameOverPanel = GameObject.Find("GameOverPanel");
        }
        if (gameOverText == null)
        {
            GameObject goTxt = GameObject.Find("GameOverText");
            if (goTxt != null) gameOverText = goTxt.GetComponent<TextMeshProUGUI>();
        }
        if (playAgainBtn == null)
        {
            GameObject btnObj = GameObject.Find("PlayAgainButton");
            if (btnObj != null) playAgainBtn = btnObj.GetComponent<Button>();
        }
        if (weaponIcon == null)
        {
            GameObject iconObj = GameObject.Find("WeaponIcon");
            if (iconObj != null) weaponIcon = iconObj.GetComponent<Image>();
        }
        if (hpText == null)
        {
            GameObject hpTxtObj = GameObject.Find("HPText");
            if (hpTxtObj != null) hpText = hpTxtObj.GetComponent<TextMeshProUGUI>();
        }
        if (scoreText == null)
        {
            GameObject scoreTxtObj = GameObject.Find("ScoreText");
            if (scoreTxtObj != null) scoreText = scoreTxtObj.GetComponent<TextMeshProUGUI>();
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (playAgainBtn != null) playAgainBtn.onClick.AddListener(RestartGame);
        
        UpdateUI();
    }

    public void AddKill()
    {
        kills++;
        score += 10;
        UpdateUI();
    }

    public void UpdateHP(int currentHealth, int maxHealth)
    {
        if (hpBarFill != null)
        {
            hpBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
        if (hpText != null)
        {
            hpText.text = currentHealth + "/" + maxHealth;
        }
    }

    public void UpdateWeaponUI(Sprite icon)
    {
        if (weaponIcon != null && icon != null)
        {
            weaponIcon.sprite = icon;
        }
    }

    void UpdateUI()
    {
        if (killsText != null)
            killsText.text = "KILLS: " + kills;
        if (scoreText != null)
            scoreText.text = "SCORE: " + score;
    }

    public void GameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        int highscore = PlayerPrefs.GetInt("Highscore", 0);
        if (score > highscore)
        {
            highscore = score;
            PlayerPrefs.SetInt("Highscore", highscore);
        }

        if (gameOverText != null)
        {
            gameOverText.text = $"GAME OVER!\nScore: {score}\nHighscore: {highscore}";
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}