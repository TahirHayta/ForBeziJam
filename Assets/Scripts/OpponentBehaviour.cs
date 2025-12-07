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

    [SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask npcMask;
    [SerializeField] teamEnum myTeam = teamEnum.Red;
    private List<GameObject> allNPCs=new List<GameObject>();

    [SerializeField] Transform myGiftPile;
    public BotCommand GetNextCommand(PlayerController.PlayerController self)
    {
        var cmd = new PlayerController.BotCommand();
        if(self == null) return cmd;
        var target = findNearestTarget();
        if (target == null) return cmd;

        float dir = Mathf.Sign(target.position.x - transform.position.x);
        cmd.move = dir;

        bool grounded = self != null && self.gameObject != null &&
                        Physics2D.Raycast(self.transform.position,
                                             Vector2.down,
                                             0.7f,
                                             groundMask);
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