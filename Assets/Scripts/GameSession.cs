using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using GameSessionUI;


namespace GameSession
{
    
public class GameSession : MonoBehaviour
{

    [Header("Game Settings")]
    public float gameDuration = 60f; // 60 seconds
    public float currentTime=0f;
    public bool isGameActive = false;
    public bool isPaused = false;

    [Header("Tracked Objects")]
    // We will find these automatically at start
    public List<GameObject> allNPCs=new List<GameObject>(); 
    
    [Header("UI References")]
    public GameSessionUI.GameSessionUI gameSessionUI;

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

        // Connect UI to backend
        this.gameSessionUI=GetComponent<GameSessionUI.GameSessionUI>();
        this.gameSessionUI.gameSession=this;

        // 2. Initialize Timer
        currentTime = gameDuration;
        isGameActive = true;
        



    }

    private void Update()
    {
        if (!isGameActive || isPaused) return;

        HandleTimer();
    }

    // --- LOGIC ---

    void HandleTimer()
    {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            this.EndGame();
            this.gameSessionUI.EndGame();
        }
    }


    public void EndGame()
    {
        isGameActive = false;
        teamEnum winningTeam = teamEnum.Nix;
        gameSessionUI.EndGame();
    }




    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
}