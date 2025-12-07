using UnityEngine;
using PlayerController;
using GameSession;
using System.Collections.Generic;

namespace AI
{
public class OpponentBehaviour : MonoBehaviour, IBotBrain
{
    [SerializeField] float moveSpeed = 6.0f;
    [SerializeField] float detectRadius = 10.0f;
    [SerializeField] float wallCheckDist = 0.6f;

    [SerializeField] float jumpCooldown = 0.35f;
    float jumpTimer;

    [SerializeField] LayerMask groundMask; // Set to "Ground" layer ONLY in Inspector
    [SerializeField] LayerMask npcMask;
    [SerializeField] teamEnum myTeam = teamEnum.Red;
    private List<GameObject> allNPCs=new List<GameObject>();

    [SerializeField] Transform myGiftPile;
    public BotCommand GetNextCommand(PlayerController.PlayerController self)
    {
        var cmd = new PlayerController.BotCommand();
        if (self == null) return cmd;

        jumpTimer -= Time.deltaTime;

        if(!self.HasGift && myGiftPile != null)
        {
            float dir = Mathf.Sign(myGiftPile.position.x - transform.position.x);
            cmd.move = dir;

            // Ray origin slightly above feet to avoid self-collision
            Vector2 origin = (Vector2)self.transform.position + new Vector2(0f, 0.1f);
            bool grounded = Physics2D.Raycast(origin, Vector2.down, 0.8f, groundMask);
            bool wallAhead = Physics2D.Raycast(origin, new Vector2(dir, 0f), wallCheckDist, groundMask);

            if (grounded && wallAhead && jumpTimer <= 0f)
            {
                cmd.jump = true;
                jumpTimer = jumpCooldown;
            }
            return cmd;
        } 

        var target = findNearestTarget();
        if (target == null) return cmd;

        float dir_npc = Mathf.Sign(target.position.x - transform.position.x);
        cmd.move = dir_npc;

        Vector2 origin_npc = (Vector2)self.transform.position + new Vector2(0f, 0.1f);
        bool grounded_npc = Physics2D.Raycast(origin_npc, Vector2.down, 0.8f, groundMask);
        bool wallAhead_npc = Physics2D.Raycast(origin_npc, new Vector2(dir_npc, 0f), wallCheckDist, groundMask);

        if (grounded_npc && wallAhead_npc && jumpTimer <= 0f)
        {
            cmd.jump = true;
            jumpTimer = jumpCooldown;
        }
        return cmd;
    }

    Transform findNearestTarget()
    {
        Transform nearest = null;
        float nearestDistSqr = float.MaxValue;
        foreach (var hit in allNPCs)
        {
            var npc = hit.GetComponent<NPCBehaviour>();
            if (npc == null) continue;
            if (npc.team == myTeam) continue;
            float d= (hit.transform.position - transform.position).sqrMagnitude;
            if (d < nearestDistSqr)
            {
                nearest = hit.transform;
                nearestDistSqr = d;
            }
        }
        return nearest;
    }
    private void updateNPCList(List<GameObject> npcList)
    {

        allNPCs = npcList;
    }
    private void OnEnable()
    {
        GameSession.GameSession.OnNPCListChanged += updateNPCList;
    }
    private void OnDisable()
    {
        GameSession.GameSession.OnNPCListChanged -= updateNPCList;
    }
}

}