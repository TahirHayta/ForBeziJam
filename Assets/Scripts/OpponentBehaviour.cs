using UnityEngine;
using PlayerController;
using GameSession;
using System.Collections.Generic;
using NPC;
using UnityEngine.Tilemaps;


namespace AI
{
public class OpponentBehaviour : MonoBehaviour, IBotBrain
{
    [SerializeField] float moveSpeed = 6.0f;
    [SerializeField] float detectRadius = 10.0f;
    [SerializeField] float wallCheckDist = 0.3f;
    [SerializeField] float stairAlignThreshold = 0.25f;
    [SerializeField] float upperFloorPause = 1.0f;

    [SerializeField] float jumpCooldown = 0.35f;
    float jumpTimer;


    [SerializeField] LayerMask groundMask; // Set to "Ground" layer ONLY in Inspector
    [SerializeField] LayerMask npcMask;
    [SerializeField] teamEnum myTeam = teamEnum.Red;
    private List<GameObject> allNPCs=new List<GameObject>();
    private List<Vector2> stairWorldPositions = new List<Vector2>(); // 2D world coords

    [SerializeField] Transform myGiftPile;

    void Start()
    {
        groundMask = LayerMask.GetMask("Ground");
        npcMask = LayerMask.GetMask("NPC");
    }


public PlayerController.BotCommand GetNextCommand(PlayerController.PlayerController self)
{
    var cmd = new PlayerController.BotCommand();
    if (self == null) return cmd;

    jumpTimer -= Time.deltaTime;

    Transform target = GetTarget();
    if (target == null) return cmd;

    Floor myFloor = GetFloor(transform.position.y);
    Floor targetFloor = GetFloor(target.position.y);

    Vector2 myPos = transform.position;

    // ✅ SAME FLOOR → DIRECT CHASE
    if (myFloor == targetFloor)
    {
        float dir = Mathf.Sign(target.position.x - transform.position.x);
        cmd.move = dir;

        Vector2 origin = (Vector2)self.transform.position + new Vector2(0f, 0.1f);
        bool grounded = self.IsGrounded();
        bool wallAhead = Physics2D.Raycast(origin, new Vector2(dir, 0f), wallCheckDist, groundMask);

        if (grounded && wallAhead && jumpTimer <= 0f)
        {
            cmd.jump = true;
            jumpTimer = jumpCooldown;
        }

        return cmd;
    }

    // ✅ DIFFERENT FLOORS → PATHFIND WITH STAIRS
    Vector2 stairTarget = GetBestStair(myFloor, targetFloor);



    bool atStair = Mathf.Abs(transform.position.x - stairTarget.x) < 0.2f;
    if (!atStair) MoveToward(stairTarget, ref cmd);

    if (atStair)
    {
        bool goingUp = targetFloor > myFloor;
        HandleStairUse(goingUp, ref cmd);
    }

    return cmd;
}



Transform findNearestTarget()
{
    // ✅ 1. PRIORITY: If bot has NO gift → go to GiftPile
    if (!GetComponent<PlayerController.PlayerController>().HasGift && myGiftPile != null)
    {
        return myGiftPile;
    }

    // ✅ 2. Otherwise → find nearest enemy NPC
    Transform nearest = null;
    float nearestDistSqr = float.MaxValue;

    foreach (var hit in allNPCs)
    {
        var npc = hit.GetComponent<NPCBehaviour>();
        if (npc == null) continue;

        // ❌ Skip own team
        if (npc.team == myTeam) continue;

        float d = (hit.transform.position - transform.position).sqrMagnitude;

        if (d < nearestDistSqr)
        {
            nearest = hit.transform;
            nearestDistSqr = d;
        }
    }

    return nearest;
}   

enum Floor
{
    First,
    Second,
    Third
}

Floor GetFloor(float y)
{
    if (y > 1.08f) return Floor.Third;
    if (y < -1.82f) return Floor.First;
    return Floor.Second;
}
Transform GetTarget()
{
    return findNearestTarget();
}
Vector2 GetBestStair(Floor current, Floor target)
{
    float requiredY = 0f;

    if (target < current) // GOING DOWN
    {
        if (current == Floor.Third) requiredY = 1.08f;
        else if (current == Floor.Second) requiredY = -1.82f;
    }
    else if (target > current) // GOING UP
    {
        if (current == Floor.First) requiredY = -1.82f;
        else if (current == Floor.Second) requiredY = 1.08f;
    }

    Vector2 best = Vector2.zero;
    float bestDist = float.MaxValue;

    foreach (var stair in stairWorldPositions)
    {
        if (Mathf.Abs(stair.y - requiredY) > 0.2f) continue;

        float dist = Mathf.Abs(stair.x - transform.position.x);
        if (dist < bestDist)
        {
            bestDist = dist;
            best = stair;
        }
    }

    return best;
}
void MoveToward(Vector2 target, ref PlayerController.BotCommand cmd)
{
    float dir = Mathf.Sign(target.x - transform.position.x);
    cmd.move = dir;
}
float stairWaitTimer = 0f;

void HandleStairUse(bool goingUp, ref PlayerController.BotCommand cmd)
{
    if (stairWaitTimer > 0f)
    {
        stairWaitTimer -= Time.deltaTime;
        return;
    }

    if (goingUp)
    {
        cmd.jump = true;
        stairWaitTimer = 2f;
    }
    else
    {
        cmd.down = true;
        stairWaitTimer = 1f;
    }
}




private void updateNPCList(List<GameObject> npcList)
    {
        allNPCs = npcList;
    }
    private void createStairsTilemap(List<Vector2> comingstairWorldPositions)
    {
        stairWorldPositions = comingstairWorldPositions;
    }
    private void OnEnable()
    {
        GameSession.GameSession.OnNPCListChanged += updateNPCList;
        GameSession.GameSession.OnStairsTilemapAssigned+=createStairsTilemap;
    }
    private void OnDisable()
    {
        GameSession.GameSession.OnNPCListChanged -= updateNPCList;
        GameSession.GameSession.OnStairsTilemapAssigned-=createStairsTilemap;

    }
}

}