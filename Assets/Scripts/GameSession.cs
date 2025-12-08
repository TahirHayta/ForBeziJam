using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using GameSessionUI;
using UnityEngine.Tilemaps;


namespace GameSession
{
    
public class GameSession : MonoBehaviour
{

    [Header("Game Settings")]
    public float gameDuration = 180f; // 60 seconds
    public float currentTime=0f;
    public bool isGameActive = false;
    public bool isPaused = false;

    [Header("Tracked Objects")]
    // We will find these automatically at start
    public List<GameObject> allNPCs=new List<GameObject>();
    public List<Tilemap> stairsTilemaps = new List<Tilemap>(); 
    public List<Vector2> stairWorldPositions = new List<Vector2>(); // 2D world coords

    
    [Header("UI References")]
    public GameSessionUI.GameSessionUI gameSessionUI;

    //[Header("Actions")]
    public static event Action<List<GameObject>> OnNPCListChanged;
    public static event Action<List<Vector2>> OnStairsTilemapAssigned; // isim böyle kaldı

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


        // assign stairs
        AssignStairsTilemap();
        DetectStairs();
        OnStairsTilemapAssigned?.Invoke(stairWorldPositions);



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
    
    // Automatically finds the Tilemap on the "Stairs" layer
    private void AssignStairsTilemap()
    {
        // Get mask for the Stairs layer
        LayerMask stairsMask = LayerMask.GetMask("Stairs");

        // Convert mask → actual layer number
        int stairsLayer = Mathf.RoundToInt(Mathf.Log(stairsMask, 2));

        // Find all tilemaps in the scene
        Tilemap[] allTilemaps = FindObjectsByType<Tilemap>(FindObjectsInactive.Include,FindObjectsSortMode.None);
        
        foreach (Tilemap tm in allTilemaps)
        {
            if (tm.gameObject.layer == stairsLayer)
            {
                Tilemap stairsTilemap = tm;
                stairsTilemaps.Add(tm);
            }
        }

        if (stairsTilemaps.Count > 0)
        {
            this.stairsTilemaps =stairsTilemaps;
        }
        else
        {
            Debug.LogWarning("No Tilemap found on layer 'Stairs'.");
        }
    }
    void DetectStairs()
    {
        List<Vector3Int> stairCells = new List<Vector3Int>();

        foreach (Tilemap tilemap in stairsTilemaps)
        {

            BoundsInt bounds = tilemap.cellBounds;

            foreach (var pos in bounds.allPositionsWithin)
            {
                TileBase tile = tilemap.GetTile(pos);
                if (tile == null) continue;

                stairCells.Add(pos);

                // Convert to 2D world coordinates
                Vector3 worldPos3D = tilemap.CellToWorld(pos);
                Vector2 worldPos2D = new Vector2(worldPos3D.x + 0.5f, worldPos3D.y + 0.5f); // center of tile
                Debug.Log("stair:"+worldPos2D);
                this.stairWorldPositions.Add(worldPos2D);
            }
        }
        List<Vector2> midPoints = new List<Vector2>();
            // Make sure count is even; if odd, ignore last one
            for (int i = 0; i < this.stairWorldPositions.Count - 1; i += 2)
            {
                Vector2 a = this.stairWorldPositions[i];
                Vector2 b = this.stairWorldPositions[i + 1];
                
                Vector2 mid = (a + b) / 2f;   // midpoint
                midPoints.Add(mid);
                Debug.Log("firt"+a+"second"+b+mid);
            }

            // Replace old list with midpoints
            this.stairWorldPositions = midPoints;
    }

}
}