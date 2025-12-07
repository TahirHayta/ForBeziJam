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
    
    // UI Bar References (Using Layout Elements)
    public LayoutElement redBarLayout;
    public LayoutElement blueBarLayout;
    public LayoutElement greenBarLayout;
    public LayoutElement yellowBarLayout;

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
    }

    void HandleTimer()
    {
        // Format time to 00:00
        int minutes = Mathf.FloorToInt(this.gameSession.currentTime / 60F);
        int seconds = Mathf.FloorToInt(this.gameSession.currentTime % 60F);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
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
    }
    public void EndGame() //Called from backend
    {
        timerText.text = "00:00";
        if(gameOverPanel) gameOverPanel.SetActive(true);
        Debug.Log("Game Over!");
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

} 
}

