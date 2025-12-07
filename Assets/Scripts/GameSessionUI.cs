using UnityEngine;
using GameSession;

using System.Collections.Generic;
using UnityEngine.UI;
using TMPro; // Don't forget this for Text Mesh Pro


namespace GameSessionUI
{
    public class GameSessionUI : MonoBehaviour
    {
    public GameSession.GameSession gameSession; // Initialized in GameSession
    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public GameObject pauseMenuPanel; // The panel with Resume/Quit buttons
    public GameObject gameOverPanel;

    public RectTransform scoreBarContainer;
    
    // UI Bar References (Using Layout Elements)
    public LayoutElement redBarLayout;
    public LayoutElement blueBarLayout;
    public LayoutElement greenBarLayout;
    public LayoutElement yellowBarLayout;

    [SerializeField] Button pauseButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 3. Hide Pause/Game Over screens
        if(pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if(gameOverPanel) gameOverPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.gameSession.isGameActive || this.gameSession.isPaused) return;

        HandleTimer();
        UpdateScoreBar(this.gameSession.allNPCs);

        if (this.gameSession.currentTime <= 0f)
        {
            this.gameSession.EndGame();
        }
    }

    void HandleTimer()
    {
        // Format time to 00:00
        int minutes = Mathf.FloorToInt(this.gameSession.currentTime / 60F);
        int seconds = Mathf.FloorToInt(this.gameSession.currentTime % 60F);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    teamEnum CalculateWinner(List<GameObject> allNPCs)
    {
        int redCount = 0;
        int blueCount = 0;
        int greenCount = 0;
        int yellowCount = 0;

        foreach (GameObject npcGameObject in allNPCs)
        {
            NPCBehaviour npc = npcGameObject.GetComponent<NPCBehaviour>();
            switch (npc.team) 
            {
                case teamEnum.Red: redCount++; break;
                case teamEnum.Blue: blueCount++; break;
                case teamEnum.Green: greenCount++; break;
                case teamEnum.Yellow: yellowCount++; break;
            }
        }

        int maxCount = Mathf.Max(redCount, Mathf.Max(blueCount, Mathf.Max(greenCount, yellowCount)));
        
        if (redCount == maxCount) return teamEnum.Red;
        if (blueCount == maxCount) return teamEnum.Blue;
        if (greenCount == maxCount) return teamEnum.Green;
        if (yellowCount == maxCount) return teamEnum.Yellow;
        return teamEnum.Nix;
    }

    void UpdateScoreBar(List<GameObject> allNPCs)
    {
        // Calculate counts
        int redCount = 0;
        int blueCount = 0;
        int greenCount = 0;
        int yellowCount = 0;

        foreach (GameObject npcGameObject in allNPCs)
        {
            NPCBehaviour npc = npcGameObject.GetComponent<NPCBehaviour>();
            switch (npc.team) 
            {
                case teamEnum.Red: redCount++; break;
                case teamEnum.Blue: blueCount++; break;
                case teamEnum.Green: greenCount++; break;
                case teamEnum.Yellow: yellowCount++; break;
            }
        }

        // Update UI Widths using Layout Elements (Weights)
        // If count is 0, we give it a tiny value so it doesn't disappear completely or break layout
        redBarLayout.flexibleWidth = redCount;
        blueBarLayout.flexibleWidth = blueCount;
        greenBarLayout.flexibleWidth = greenCount;
        yellowBarLayout.flexibleWidth = yellowCount;

        redBarLayout.preferredHeight = 50f;
        blueBarLayout.preferredHeight = 50f;
        greenBarLayout.preferredHeight = 50f;
        yellowBarLayout.preferredHeight = 50f;
    }
    public void EndGame() //Called from backend
    {
        timerText.text = "00:00";
        scoreBarContainer.gameObject.SetActive(false);
        
        teamEnum winner = CalculateWinner(this.gameSession.allNPCs);
        int winningScore = GetTeamScore(this.gameSession.allNPCs, winner);
        
        gameOverPanel.GetComponentInChildren<TextMeshProUGUI>().fontSize = 20;
        gameOverPanel.GetComponentInChildren<TextMeshProUGUI>().text = $"Game Over!\n{winner} Team Wins!\nScore: {winningScore}";
        
        gameSession.isGameActive = false;
        gameOverPanel.SetActive(true);
        Debug.Log($"Game Over! Winner: {winner}");
    }

    int GetTeamScore(List<GameObject> allNPCs, teamEnum team)
    {
        int count = 0;
        foreach (GameObject npcGameObject in allNPCs)
        {
            NPCBehaviour npc = npcGameObject.GetComponent<NPCBehaviour>();
            if (npc.team == team) count++;
        }
        return count;
    }

    public void TogglePause()
    {
        this.gameSession.isPaused = !this.gameSession.isPaused;
        
        if (this.gameSession.isPaused)
        {
            Time.timeScale = 0f; // Freezes the game
            if(pauseMenuPanel) pauseMenuPanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f; // Resumes the game
            if(pauseMenuPanel) pauseMenuPanel.SetActive(false);
        }
    }

    void Awake()
    {
        // Anchor pause button to top-right
        RectTransform pauseRect = pauseButton.GetComponent<RectTransform>();
        pauseRect.anchorMin = new Vector2(1f, 1f); // top-right
        pauseRect.anchorMax = new Vector2(1f, 1f);
        pauseRect.pivot = new Vector2(1f, 1f);
        pauseRect.anchoredPosition = new Vector2(-20f, -20f); // 20px offset from corner
    }

} 
}

