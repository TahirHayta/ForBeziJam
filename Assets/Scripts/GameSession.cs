using UnityEngine;
using UnityEngine.UI;
using TMPro; // Don't forget this for Text Mesh Pro
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
namespace GameSession
{
    
public class GameSession : MonoBehaviour
{

    [Header("Game Settings")]
    public float gameDuration = 60f; // 60 seconds
    private float currentTime=0f;
    private bool isGameActive = false;
    private bool isPaused = false;

    [Header("Tracked Objects")]
    // We will find these automatically at start
    public List<GameObject> allNPCs=new List<GameObject>(); 

    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public GameObject pauseMenuPanel; // The panel with Resume/Quit buttons
    public GameObject gameOverPanel;
    
    // UI Bar References (Using Layout Elements)
    public LayoutElement redBarLayout;
    public LayoutElement blueBarLayout;
    public LayoutElement greenBarLayout;
    public LayoutElement yellowBarLayout;
    public static event Action<List<GameObject>> OnNPCListChanged;
    private void Start()
    {
        // 1. Get the integer ID for the "NPC" layer // TODO this can be more efficient way find
        int npcLayerID = LayerMask.NameToLayer("NPC");

        // 2. Find ALL active objects in the scene
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None); 

        // 4. Loop through and check the layer
        foreach (GameObject go in allObjects)
        {
            if (go.layer == npcLayerID)
            {
                allNPCs.Add(go);
            }
        }
        OnNPCListChanged?.Invoke(allNPCs);

        // 2. Initialize Timer
        currentTime = gameDuration;
        isGameActive = true;
        
        // 3. Hide Pause/Game Over screens
        if(pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if(gameOverPanel) gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        if (!isGameActive || isPaused) return;

        HandleTimer();
        UpdateScoreBar();
    }

    // --- LOGIC ---

    void HandleTimer()
    {
        currentTime -= Time.deltaTime;
        
        // Format time to 00:00
        int minutes = Mathf.FloorToInt(currentTime / 60F);
        int seconds = Mathf.FloorToInt(currentTime % 60F);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (currentTime <= 0)
        {
            EndGame();
        }
    }

    void UpdateScoreBar()
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

    void EndGame()
    {
        isGameActive = false;
        currentTime = 0;
        timerText.text = "00:00";
        if(gameOverPanel) gameOverPanel.SetActive(true);
        Debug.Log("Game Over!");
    }

    // --- BUTTON FUNCTIONS ---

    public void TogglePause()
    {
        isPaused = !isPaused;
        
        if (isPaused)
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

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
}