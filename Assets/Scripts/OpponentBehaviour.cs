using UnityEngine;
using PlayerController;

namespace AI
{
public class OpponentBehaviour : MonoBehaviour, IBotBrain
{
    [SerializeField] float moveSpeed = 6.0f;
    [SerializeField] float detectRadius = 10.0f;
    [SerializeField] float wallCheckDist = 0.6f;

    [SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask npcMask;
    [SerializeField] teamEnum myTeam = teamEnum.Red;

    public BotCommand GetNextCommand(PlayerController.PlayerController self)
    {
        var cmd = new PlayerController.BotCommand();
        var target = findNearestTarget();
        if (target == null) return cmd;

        float dir = Mathf.Sign(target.position.x - transform.position.x);
        cmd.move = dir;

        bool grounded = self != null && self.gameObject != null &&
                        Physics2D.Raycast(self.transform.position,
                                             Vector2.down,
                                             0.7f,
                                             groundMask);
        print("Grounded: " + grounded);
        return cmd;
    }

    Transform findNearestTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRadius, npcMask);
        Transform nearest = null;
        float nearestDistSqr = float.MaxValue;
        foreach (var hit in hits)
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
}
}