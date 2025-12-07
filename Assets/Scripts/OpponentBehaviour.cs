using UnityEngine;
using PlayerController;
using GameSession;
using System.Collections.Generic;
using NPC;

namespace AI
{
public class OpponentBehaviour : MonoBehaviour, IBotBrain
{
    [SerializeField] float moveSpeed = 6.0f;
    [SerializeField] float detectRadius = 10.0f;
    [SerializeField] float wallCheckDist = 0.3f;

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

            Vector2 origin = (Vector2)self.transform.position + new Vector2(0f, 0.1f);
            bool grounded = self.IsGrounded();
            bool wallAhead = Physics2D.Raycast(origin, new Vector2(dir, 0f), wallCheckDist, groundMask);

            // Check if pile is above bot
            bool pileAbove = myGiftPile.position.y > self.transform.position.y + 0.5f;

            if (grounded && (wallAhead || pileAbove) && jumpTimer <= 0f)
            {
                cmd.jump = true;
                jumpTimer = jumpCooldown;
            }
            return cmd;
        } 

        var target = findNearestTarget();
        if (target == null) return cmd;
        bool emptyNPCExists = false;
        NPC.NPCBehaviour hit_component;
        foreach (var hit in allNPCs)
            {
                hit_component = hit.GetComponent<NPCBehaviour>();
                if (hit_component == null) continue; 
                if (hit_component.team == teamEnum.Nix)
                {
                    emptyNPCExists = true;
                    break;
                }
            }

        if(emptyNPCExists)  {
                float dir_npc = Mathf.Sign(target.position.x - transform.position.x);
                cmd.move = dir_npc;

                Vector2 origin_npc = (Vector2)self.transform.position + new Vector2(0f, 0.1f);
                bool grounded_npc = self.IsGrounded();
                bool wallAhead_npc = Physics2D.Raycast(origin_npc, new Vector2(dir_npc, 0f), wallCheckDist, groundMask);

                if (grounded_npc && wallAhead_npc && jumpTimer <= 0f)
                {
                    cmd.jump = true;
                    jumpTimer = jumpCooldown;
                }
                return cmd;
            }
            else
            {
                // Track the npc's with other players gifts
                float dir_npc_gift = Mathf.Sign(target.position.x - transform.position.x);
                cmd.move = dir_npc_gift;

                // Add jump logic here too
                Vector2 origin_gift = (Vector2)self.transform.position + new Vector2(0f, 0.1f);
                bool grounded_gift = self.GetComponent<PlayerController.PlayerController>().IsGrounded();
                bool wallAhead_gift = Physics2D.Raycast(origin_gift, new Vector2(dir_npc_gift, 0f), wallCheckDist, groundMask);

                if (grounded_gift && wallAhead_gift && jumpTimer <= 0f)
                {
                    cmd.jump = true;
                    jumpTimer = jumpCooldown;
                }
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